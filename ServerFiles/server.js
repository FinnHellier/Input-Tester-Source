const express = require("express"); //needed for simple communication
const mysql = require("mysql2/promise");    //needed for async
const cors = require("cors");    //needed for http/https communication to be trusted
const crypto = require("crypto");
//CREATE DATABASE unity_db; // unity_db is the name of the aws instance and the internal database


const app = express();
const PORT = 3000; //80 http port 443 for https redirected with nginx
let lastShutdownTime = 0;
const CooldownMs = 5 * 60000; //cooldown in minutes
const ShutdownPassword = process.env.DB_SHUTDOWN;

app.use(cors());
app.use(express.json());

const pool = mysql.createPool({
    host: process.env.DB_HOST,
    user: process.env.DB_USER,
    password: process.env.DB_PASS,
    database: process.env.DB_NAME,
    waitForConnections: true,
    connectionLimit: 100,
    queueLimit: 0
});

app.get("/", (req, res) => {
    res.send("Running api server");
});

app.get("/initialiseTestDB",async (req,res) => {
    try{
        await pool.query(
            `CREATE TABLE IF NOT EXISTS test_tb (
            SessionID INT AUTO_INCREMENT PRIMARY KEY,
            LoginTime DATETIME NOT NULL
            )`   
        );
        res.send("Table 'test_tb' initialised");

    }
    catch(err){
        console.error(err);
        res.status(500).send("table creation failed");

    }

});

app.get("/loginTestDB",async (req,res) =>{
    try{
        await pool.query('INSERT INTO test_tb(LoginTime) VALUES(NOW())'
        );
        res.send("Login time recorded");
    }
    catch(err){
        console.error(err);
            res.status(500).send("login failed");
    }
});

const server = app.listen(PORT, '0.0.0.0',() => {
  console.log(`Opened port: ${PORT}`);
});


app.get("/initialiseDB", async (req, res) => {
    try {
        await pool.query(`
            CREATE TABLE IF NOT EXISTS users_tb (
                UserID VARCHAR(36) PRIMARY KEY,
                Experience INT,
                CurrentRun INT DEFAULT 0,
                CurrentLevel INT DEFAULT 0,
                CurrentAttempt INT DEFAULT 0,
                StartTime DATETIME NOT NULL
            )
        `);
        await pool.query(`
            CREATE TABLE IF NOT EXISTS runs_tb (
                UserID VARCHAR(36) NOT NULL,
                RunNumber INT NOT NULL,
                InputDevice INT,
                Seed INT,
                PRIMARY KEY (UserID, RunNumber),
                FOREIGN KEY (UserID) REFERENCES users_tb(UserID)
            )
        `);
        await pool.query(`
            CREATE TABLE IF NOT EXISTS levels_tb (
                UserID VARCHAR(36) NOT NULL,
                RunNumber INT NOT NULL,
                LevelNumber INT NOT NULL,
                Attempts BINARY(64) NOT NULL,
                CurrentAttempt TINYINT UNSIGNED NOT NULL DEFAULT 0,
                PRIMARY KEY (UserID, RunNumber, LevelNumber),
                FOREIGN KEY (UserID, RunNumber) REFERENCES runs_tb(UserID, RunNumber)
            )
        `);
        res.send("Database initialised");
    }
    catch (err) {
        console.error(err);
        res.status(500).send("Database initialisation failed");
    }
});

app.get("/getScore", async (req, res) => {
    try {
        const { id } = req.query;
        
        const [rows] = await pool.query(
            `SELECT Attempts FROM levels_tb WHERE UserID = ? AND RunNumber = (
                SELECT CurrentRun FROM users_tb WHERE UserID = ?
            )`,
            [id, id]
        );

        let totalScore = 0;
        for (const row of rows) {
            const attempts = row.Attempts;
            let attemptsUsed = 0;
            for (let i = 0; i < 64; i++) {
                if (attempts[i] === 0) break;
                attemptsUsed++;
            }
            totalScore += 64 - attemptsUsed;
        }

        res.json({ score: totalScore });//out of 512
    }
    catch (err) {
        console.error(err);
        res.status(500).send("Failed to calculate score");
    }
});

app.get("/newID", async (req, res) => {
    try {
        const id = crypto.randomUUID();
        await pool.query(
            `INSERT INTO users_tb (UserID, StartTime) VALUES (?, NOW())`,
            [id]
        );
        res.json({ id });
    }
    catch (err) {
        console.error(err);
        res.status(500).send("Failed to create user");
    }
});

app.post("/initialisePlayer", async (req, res) => {//id and experience
    try {
        const { id, experience } = req.body;
        await pool.query(
            `UPDATE users_tb SET Experience = ? WHERE UserID = ?`,
            [experience, id]
        );
        res.send("Player initialised");
    }
    catch (err) {
        console.error(err);
        res.status(500).send("Failed to initialise player");
    }
});

app.post("/createRun", async (req, res) => {
    try {
        const { id, inputDevice, seed } = req.body;
        
        await pool.query(
            `UPDATE users_tb SET CurrentRun = CurrentRun + 1, CurrentLevel = 0, CurrentAttempt = 0 WHERE UserID = ?`,
            [id]
        );

        const [rows] = await pool.query(
            `SELECT CurrentRun FROM users_tb WHERE UserID = ?`,
            [id]
        );
        const runNumber = rows[0].CurrentRun;

        await pool.query(
            `INSERT INTO runs_tb (UserID, RunNumber, InputDevice, Seed) VALUES (?, ?, ?, ?)`,
            [id, runNumber, inputDevice, seed]
        );

        res.json({ runNumber });
    }
    catch (err) {
        console.error(err);
        res.status(500).send("Failed to create run");
    }
});

app.post("/submitAttempt", async (req, res) => {//id and attempt data bytes
    try {
        const { id, attemptData } = req.body; //json data
        const [rows] = await pool.query(
            `SELECT CurrentRun, CurrentLevel, CurrentAttempt FROM users_tb WHERE UserID = ?`,
            [id]
        );
        const { CurrentRun, CurrentLevel, CurrentAttempt } = rows[0];

        await pool.query(
            `INSERT INTO levels_tb (UserID, RunNumber, LevelNumber, Attempts, CurrentAttempt)
             VALUES (?, ?, ?, REPEAT(CHAR(0), 64), 0)
             ON DUPLICATE KEY UPDATE UserID = UserID`,
            [id, CurrentRun, CurrentLevel]
        );

        await pool.query(
            `UPDATE levels_tb 
             SET Attempts = INSERT(Attempts, ? + 1, 1, CHAR(?))
             WHERE UserID = ? AND RunNumber = ? AND LevelNumber = ?`,
            [CurrentAttempt, attemptData, id, CurrentRun, CurrentLevel]
        );

        const succeeded = (attemptData & 0b10000000) !== 0;

        if (succeeded) {
            await pool.query(
                `UPDATE users_tb SET 
                    CurrentLevel = CurrentLevel + 1,
                    CurrentAttempt = 0
                 WHERE UserID = ?`,
                [id]
            );
        } else {
            await pool.query(
                `UPDATE users_tb SET CurrentAttempt = CurrentAttempt + 1 WHERE UserID = ?`,
                [id]
            );
        }

        res.json({ succeeded });
    }
    catch (err) {
        console.error(err);
        res.status(500).send("Failed to submit attempt");
    }
});

const shutdownServer = () =>{
    console.log("shutting down");
    server.close(()=>{
        console.log("Sucessful shutdown");
        process.exit(0);
    });

};

app.post("/shutdown", (req,res)=>{
    const {password} = req.body;
    const now = Date.now();
    if(now-lastShutdownTime <CooldownMs){
        return res.status(429).send("On Cooldown");
    }
    lastShutdownTime = now;
    if(password !== ShutdownPassword){
        console.log(`Wrong password at: ${new Date().toISOString()}`)
        return res.status(401).send("Wrong password");
    }
    res.send("Shutting down");
    console.log(`Successful shutdown at: ${new Date().toISOString()}`)
    shutdownServer();
    }
)

process.on("SIGINT",shutdownServer);
process.on("SIGTERM",shutdownServer);

process.on("uncaughtException", (err) => {
  console.error("Uncaught exception:", err);
  shutdownServer();
});

process.on("unhandledRejection", (reason, promise) => {
  console.error("Unhandled rejection at:", promise, "reason:", reason);
  shutdownServer();
});