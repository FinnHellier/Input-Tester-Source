using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGamePlay : MonoBehaviour
{
    [SerializeField] private Toggle theToggleState;
    [SerializeField] private ExperienceDropDown expDropDown;
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
        expDropDown.dropdown.onValueChanged.AddListener(delegate { ValidateForm(); });
        inputDropDown.dropdown.onValueChanged.AddListener(delegate { ValidateForm(); });
        radioButtons.OnSelectionChanged.AddListener(ValidateForm);

        attachedButton.onClick.AddListener(MoveToInitialise);
    }

    void ValidateForm()
    {
        attachedButton.interactable =
            theToggleState.isOn &&
            expDropDown.isValid &&
            inputDropDown.isValid &&
            radioButtons.isValid;
    }

    public void MoveToInitialise()
    {
        AccountManagement.instance.GoToGameplay(AccountManagement.instance.AccExperience, AccountManagement.instance.AccInputType);
    }
}