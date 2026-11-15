using UnityEngine;
using UnityEngine.UI;
using static ButtonVisualController;

public class RadioButton : MonoBehaviour
{
    private Button button;                // The Button component
    private RadioButtons controller;
    private Image img;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.green;
    public buttonVisuals buttonvisuals;


    public void setController(RadioButtons newController) {
        controller= newController;
    }
    private void Start()
    {
        if (button == null)
            button = GetComponent<Button>();
        img = button.GetComponent<Image>();
        button.onClick.AddListener(OnClick);
        SetSelected(false);
    }

    public void OnClick()
    {
        ButtonVisualController.Instance.setNewVisuals(buttonvisuals);
        controller.SelectButton(this);
    }

    public void SetSelected(bool selected)
    {
        
        if (selected)
        {
            img.color = selectedColor;
        }
        else {
            img.color = normalColor;
        }
    }
}
