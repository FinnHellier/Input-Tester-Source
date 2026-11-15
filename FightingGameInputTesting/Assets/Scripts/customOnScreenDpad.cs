using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

public class customOnScreenDpad : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private float movementRange = 50f;

    [InputControl(layout = "Vector2")]
    [SerializeField] private string m_ControlPath;

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MoveStick(eventData.position, eventData.pressEventCamera);
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveStick(eventData.position, eventData.pressEventCamera);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SendValueToControl(Vector2.zero);
    }

    void MoveStick(Vector2 pointerPosition, Camera uiCamera)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)transform, pointerPosition, uiCamera, out var localPoint);

        var delta = Vector2.ClampMagnitude(localPoint, movementRange);
        SendValueToControl(delta / movementRange);
    }
}
