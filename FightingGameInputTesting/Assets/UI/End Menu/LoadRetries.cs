using TMPro;
using UnityEngine;


public class LoadRetries : MonoBehaviour{
    private TextMeshProUGUI scoreText;
    void Start()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        ServerCommunication.Instance.GetScore(AccountManagement.instance.GetID(),onSuccess: score => scoreText.text = score.ToString()
        );

    }


}
