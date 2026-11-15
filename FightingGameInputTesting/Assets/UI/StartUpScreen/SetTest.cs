using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SetTest : MonoBehaviour
{
    private TextMeshProUGUI textField;
    [SerializeField] private TextAsset disclaimerFile;
    private void Awake()
    {
        textField = GetComponent<TextMeshProUGUI>();
        textField.text = disclaimerFile.text;
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            textField.GetComponent<RectTransform>());
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

}
