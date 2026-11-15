using System;
using TMPro;
using UnityEngine;
using static AccountManagement;

public class ExperienceDropDown : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public bool isValid;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }

    void Start()
    {
        //dropdown.ClearOptions();
        string[] enumNames = Enum.GetNames(typeof(experience));
        for (int x = 0; x < enumNames.Length; x++) {
            enumNames[x] = EnumWithSpaces(enumNames[x]);
        }
        dropdown.AddOptions(new System.Collections.Generic.List<string>(enumNames));
        dropdown.value = 0;
        dropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    public experience GetSelected()
    {
        return (experience)dropdown.value;
    }

    void OnDropdownChanged(int index)
    {
        if (index != 0)
        {
            dropdown.onValueChanged.RemoveListener(OnDropdownChanged);

            AccountManagement.instance.AccExperience = (experience)(index - 1);

            dropdown.options.RemoveAt(0);
            dropdown.value = index - 1;
            dropdown.RefreshShownValue();

            dropdown.onValueChanged.AddListener(OnDropdownChangedAfterRemoval);

            isValid = true;
        }
    }
    void OnDropdownChangedAfterRemoval(int index)
    {
        string selectedOption = dropdown.options[index].text;

        AccountManagement.instance.AccExperience = (experience)index;
    }
}
