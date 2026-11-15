using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ServerCommunication : MonoBehaviour 
{
    public static ServerCommunication Instance { get; private set; }
    
    private static string apiUrl = "https://inputtester.finnhellier.com/";
    private int activeRequests = 0;

    void ShowSpinner(){
        activeRequests++;
        SpinnerVisibiltyScript.Instance.makeActive();
    }
    void HideSpinner(){
        if (--activeRequests <= 0){
            activeRequests = 0;
            SpinnerVisibiltyScript.Instance.makeInactive();
        }
    }



    public void Awake()
    {
        Instance = this;
    }
    /*
    public void testServer() {
        Debug.Log("Z");
        StartCoroutine(SendRequest());
    }
    static IEnumerator SendRequest(){
        Debug.Log("A");
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest(); // Send request and wait for response

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {// Check for errors
            Debug.LogError("Error: " + request.error);
        }
        else{
            Debug.Log("Server Response: " + request.downloadHandler.text);  // Print server response
        }
    }
    */
    IEnumerator GetHelper(string endpoint, Action<string> onSuccess, Action onFail = null){
        ShowSpinner();
        //Debug.Log("Get");
        using var request = UnityWebRequest.Get(apiUrl + endpoint);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            HideSpinner();
            Debug.LogError(endpoint + "failed: " + request.error);
            onFail?.Invoke();
        }
        else
        {
            //Debug.Log("Get succeeded");
            HideSpinner();
            onSuccess?.Invoke(request.downloadHandler.text);
        }
    }

    
    IEnumerator PostHelper(string endpoint, string json, Action<string> onSuccess, Action onFail = null){
        ShowSpinner();
        Debug.Log(json);
        //Debug.Log("Post");
        using var request = new UnityWebRequest(apiUrl + endpoint, "POST");
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            //Debug.Log("Post failed");
            HideSpinner();
            Debug.LogError(endpoint + "failed: " + request.error);
            onFail?.Invoke();
        }
        else {
            //Debug.Log("Post succeeded");
            HideSpinner();
            onSuccess?.Invoke(request.downloadHandler.text);
        }
            
    }

    public void GetNewID(Action<string> onSuccess, Action onFail = null){
        StartCoroutine(GetHelper("newID", response =>{
            var data = JsonUtility.FromJson<IDResponse>(response);
            onSuccess?.Invoke(data.id);
        }, onFail));
    }

    public void InitialisePlayer(string id, int experience, Action onSuccess = null, Action onFail = null){
        string json = JsonUtility.ToJson(new InitialisePlayerRequest { id = id, experience = experience });
        StartCoroutine(PostHelper("initialisePlayer", json, _ => onSuccess?.Invoke(), onFail));
    }

    public void CreateRun(string id, int inputDevice, int seed, Action<int> onSuccess = null, Action onFail = null){
        string json = JsonUtility.ToJson(new CreateRunRequest { id = id, inputDevice = inputDevice, seed = seed });
        StartCoroutine(PostHelper("createRun", json, response =>{
            var data = JsonUtility.FromJson<RunResponse>(response);
            onSuccess?.Invoke(data.runNumber);
        }, onFail));
    }

    public void SubmitAttempt(string id, byte attemptData, Action<bool> onSuccess = null, Action onFail = null){
        string json = JsonUtility.ToJson(new SubmitAttemptRequest { id = id, attemptData = attemptData });
        StartCoroutine(PostHelper("submitAttempt", json, response =>{
            var data = JsonUtility.FromJson<AttemptResponse>(response);
            onSuccess?.Invoke(data.succeeded);
        }, onFail));
    }

    public void GetScore(string id, Action<int> onSuccess = null, Action onFail = null){
        StartCoroutine(GetHelper($"getScore?id={id}", response =>{
            var data = JsonUtility.FromJson<ScoreResponse>(response);
            onSuccess?.Invoke(data.score);
        }, onFail));
    }

    // json structure
    [Serializable] class IDResponse { public string id; }
    [Serializable] class RunResponse { public int runNumber; }
    [Serializable] class AttemptResponse { public bool succeeded; }
    [Serializable] class ScoreResponse { public int score; }
    [Serializable] class InitialisePlayerRequest { public string id; public int experience; }
    [Serializable] class CreateRunRequest { public string id; public int inputDevice; public int seed; }
    [Serializable] class SubmitAttemptRequest { public string id; public int attemptData; }
    /*
    public void RequestID()
    {
        StartCoroutine(RequestIDCoroutine());
    }
    IEnumerator RequestIDCoroutine()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl + "newIDrequest");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            string id = request.downloadHandler.text;

            Debug.Log("Received ID: " + id);

            // Save locally
            PlayerPrefs.SetString("playerID", id);
            PlayerPrefs.Save();
        }
    }
    */
}
