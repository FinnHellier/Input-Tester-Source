using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartInitialisation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Toggle theToggleState;
    private Button attachedButton;
    private void Awake()
    {
       attachedButton = GetComponent<Button>();
    }

    void Start()
    {
        attachedButton.interactable = false;
        theToggleState.onValueChanged.AddListener(OnToggleChanged);
        attachedButton.onClick.AddListener(MoveToInitialise);
    }

    void OnToggleChanged(bool value)
    {
        attachedButton.interactable = value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveToInitialise(){
        AccountManagement.instance.GoToInitialising();
    }

    
}
