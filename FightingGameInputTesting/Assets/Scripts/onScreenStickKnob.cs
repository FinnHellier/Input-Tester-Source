using UnityEngine;
using static InputActionsparams;

public class OnScreenStickKnob : MonoBehaviour
{
    [SerializeField] float range = 60f;
    private RectTransform rectTransform;
    private Vector2 currentInput;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        InputActionsparams.Instance.controls.GameplayMap.Movement.performed += ctx => currentInput = ctx.ReadValue<Vector2>();
        InputActionsparams.Instance.controls.GameplayMap.Movement.canceled += ctx => currentInput = Vector2.zero;
    }

    void Update()
    {
        rectTransform.anchoredPosition = currentInput * range;
    }


    void UpdatePosition(Vector2 input){
        rectTransform.anchoredPosition = input * range;
    }
}