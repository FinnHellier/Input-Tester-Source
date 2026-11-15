using UnityEngine;
using UnityEngine.UI;

public class SpinnerVisibiltyScript : MonoBehaviour
{
    public static SpinnerVisibiltyScript Instance;
    [SerializeField] GameObject loadingIcon;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        loadingIcon.SetActive(false);
    }

    public void makeActive() {
        loadingIcon.SetActive(true);
    }

    public void makeInactive() {
        loadingIcon.SetActive(false);
    }
}
