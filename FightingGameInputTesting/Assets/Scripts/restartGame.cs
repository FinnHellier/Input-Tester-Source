using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class restartGame : MonoBehaviour
{
    [SerializeField] private Toggle theToggleState;
    [SerializeField] private InputDropDown inputDropDown;
    [SerializeField] private RadioButtons radioButtons;

    private Button attachedButton;

    private void Awake()
    {
        attachedButton = GetComponent<Button>();
    }

    void Start()
    {
        attachedButton.interactable = false;

        // Listen for changes
        theToggleState.onValueChanged.AddListener(delegate { ValidateForm(); });
        inputDropDown.dropdown.onValueChanged.AddListener(delegate { ValidateForm(); });
        radioButtons.OnSelectionChanged.AddListener(ValidateForm);

        attachedButton.onClick.AddListener(MoveToInitialise);
    }

    void ValidateForm()
    {
        attachedButton.interactable =
            theToggleState.isOn &&
            inputDropDown.isValid &&
            radioButtons.isValid;
    }

    public void MoveToInitialise()
    {
        AccountManagement.instance.GoBackToGameplay(AccountManagement.instance.AccInputType);
    }
}