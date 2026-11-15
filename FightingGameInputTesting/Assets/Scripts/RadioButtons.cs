using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RadioButtons : MonoBehaviour
{
    public List<RadioButton> buttons = new List<RadioButton>();

    public UnityEvent OnSelectionChanged;

    public bool isValid;

    private void Awake(){
        for (int x = 0; x < buttons.Count; x++){buttons[x].setController(this);}
    }

    public void SelectButton(RadioButton selectedButton){
        for (int x = 0; x < buttons.Count; x++){buttons[x].SetSelected(buttons[x] == selectedButton);}
        isValid = true;
        OnSelectionChanged?.Invoke();
    }
}