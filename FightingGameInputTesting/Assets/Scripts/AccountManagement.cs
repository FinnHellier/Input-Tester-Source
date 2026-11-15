using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AccountManagement : MonoBehaviour
{
    public static AccountManagement instance;
    public string ID;
    public inputType AccInputType;
    public experience AccExperience;

    const string stateKey = "state"; //links to account state (enum)
    const string PlayerIdKey = "PlayerId"; //account id, request on changing screen to initialising (string) 
    const string ExperienceKey = "Experience";//experience entered on initialisation (enum)
    //const string RunKey = "Run"; //run number, starts at 1 (int)
    const string InputDeviceKey = "InputDevice"; //input device, corresponds to run entered on initialisation and end (enum)
    const string StageKey = "Stage"; //current stage in the sequence starts at 0 (int)
    const string AttemptsKey = "Attempts"; //current attempts on the stage (int)
    const string SeedKey = "Seed"; //current seed (int)
    public string GetID() => PlayerPrefs.HasKey(PlayerIdKey) ? PlayerPrefs.GetString(PlayerIdKey) : null;
    //public int? GetRun() => PlayerPrefs.HasKey(RunKey) ? PlayerPrefs.GetInt(RunKey) : null;
    public int? GetStage() => PlayerPrefs.HasKey(StageKey) ? PlayerPrefs.GetInt(StageKey) : null;
    public int? GetAttempts() => PlayerPrefs.HasKey(AttemptsKey) ? PlayerPrefs.GetInt(AttemptsKey) : null;
    public int? GetSeed() => PlayerPrefs.HasKey(SeedKey) ? PlayerPrefs.GetInt(SeedKey) : null;
    public experience? GetExperience() => PlayerPrefs.HasKey(ExperienceKey) ? (experience)PlayerPrefs.GetInt(ExperienceKey) : null;
    public inputType? GetInputType() => PlayerPrefs.HasKey(InputDeviceKey) ? (inputType)PlayerPrefs.GetInt(InputDeviceKey) : null;

    public void SetID(string id) { PlayerPrefs.SetString(PlayerIdKey, id); PlayerPrefs.Save(); }
   // public void SetRun(int run) { PlayerPrefs.SetInt(RunKey, run); PlayerPrefs.Save(); }
    public void SetStage(int stage) { PlayerPrefs.SetInt(StageKey, stage); PlayerPrefs.Save(); }
    public void setState(int state) { PlayerPrefs.SetInt(stateKey,state); PlayerPrefs.Save(); }
    public void SetAttempts(int attempts) { PlayerPrefs.SetInt(AttemptsKey, attempts); PlayerPrefs.Save(); }
    public void SetExperience(experience exp) { PlayerPrefs.SetInt(ExperienceKey, (int)exp); PlayerPrefs.Save(); }
    public void SetInputType(inputType input) { PlayerPrefs.SetInt(InputDeviceKey, (int)input); PlayerPrefs.Save(); }
    public void SetSeed(int seed) { PlayerPrefs.SetInt(SeedKey, seed); PlayerPrefs.Save(); }
    public void DeleteStage() { PlayerPrefs.DeleteKey(StageKey); PlayerPrefs.Save(); }
    

    public void GoToInitialising(){
        SceneManager.LoadScene("InitialiseScreen");
        ServerCommunication.Instance.GetNewID(onSuccess: id => {
                SetID(id);
                PlayerPrefs.SetInt(stateKey, (int)accountState.initialising);
                },
            onFail: () => Debug.LogError("Failed to get ID"));


    }


    public void GoToGameplay(experience exp, inputType input)
    {
        SetExperience(exp);
        SetInputType(input);
        //SetStage(0);
        SceneManager.LoadScene("GameplayScreen");
        setState((int)accountState.gameplay);
        ServerCommunication.Instance.InitialisePlayer(
            GetID(),
            (int)exp
        );
    }

    public void GoBackToGameplay(inputType input){
        SetInputType(input);
        SceneManager.LoadScene("GameplayScreen");

        setState((int)accountState.gameplay);
    }

    public void GoToEnd()
    {
        SceneManager.LoadScene("FinalScreen");


        setState((int)accountState.end);
        //PlayerPrefs.Save();
    }
    /*
    public void GoToGameplay(){
        //get values
        //generate seed

        //update scene

        //send data to server
        //set experience
        //set input
        //setstage to 0
        //increment run
        //change state


    }

    public void GoToEnd() {
        //update scene
        //change state
    }
    public void GoBackToGameplay() {
        //get values
        //generate seed

        //update scene

        //send data to server
        //set input
        //setstage to 0
        //increment run
        //change state
    }
    */



    enum accountState{
        reading,
        initialising,
        gameplay,
        end
    }


    public enum experience {
        None,
        PlayedSomeGames,
        PlaysGamesRegularly,
        PlayedFightingGame,
        PlaysFightingGamesRegularly,
        CompetesInFightingGames,
        CompetesInInvitationals

    }

    public enum inputType {
        Controller,
        Touch,
        Keyboard,
        ArcadeStick,
        Leverless
    }

    public static string EnumWithSpaces(string value)
    {
        return Regex.Replace(value, "(\\B[A-Z])", " $1");
    }
    private void Awake(){
        instance = this;
        launchBehaviour();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void launchBehaviour() {
        if (PlayerPrefs.HasKey(stateKey))
        {
            ID = PlayerPrefs.GetString(PlayerIdKey); //assumes ID is made if state is made, may lead to empty IDs but pretty safe
            if (GetExperience() != null) AccExperience = GetExperience().Value;
            if (GetInputType() != null) AccInputType = GetInputType().Value;


            accountState state = (accountState)PlayerPrefs.GetInt(stateKey);
            

            switch (state)
            {
                case accountState.reading:
                    break;
                case accountState.initialising:
                    SceneManager.LoadScene("InitialiseScreen");
                    break;
                case accountState.gameplay:
                    SceneManager.LoadScene("GameplayScreen");
                    break;
                case accountState.end:
                    SceneManager.LoadScene("FinalScreen");
                    break;
            }
        }
    }
}
/*
 * 
if (PlayerPrefs.HasKey("eed"))
{
    seed = PlayerPrefs.GetInt("seed");
}
PlayerPrefs.SetInt("seed", seed);
PlayerPrefs.Save();
*/