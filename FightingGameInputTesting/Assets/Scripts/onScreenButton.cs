using UnityEngine;
using UnityEngine.UI;
using static InputActionsparams;

public class onScreenButton : MonoBehaviour
{
    [SerializeField] Image buttonImage;
    [SerializeField] faceButton buttonType;
    static readonly Color normal = Color.white;
    static readonly Color pressed = new Color(0.6f, 0.6f, 0.6f);

    void Start()
    {
        buttonImage.sprite = ButtonVisualController.Instance.GetButtonImage(buttonType);
        buttonImage =GetComponent<Image>();
        initialiseButtons(buttonType);
    }
    /*controls.GameplayMap.LeftPunch.started += ctx =>{buttonPressedEvent?.Invoke(faceButton.West); };
    controls.GameplayMap.RightPunch.started += ctx => { buttonPressedEvent?.Invoke(faceButton.North); };
    controls.GameplayMap.LeftKick.started += ctx => { buttonPressedEvent?.Invoke(faceButton.South); };
    controls.GameplayMap.RightKick.started += ctx => { buttonPressedEvent?.Invoke(faceButton.East); };*/

    void initialiseButtons(faceButton button)
    {
        switch (button)
        {
            case faceButton.North:
                InputActionsparams.Instance.controls.GameplayMap.RightPunch.started += ctx => { buttonImage.color = pressed; };
                InputActionsparams.Instance.controls.GameplayMap.RightPunch.canceled += ctx => { buttonImage.color = normal; };
                break;
            case faceButton.South:
                InputActionsparams.Instance.controls.GameplayMap.LeftKick.started += ctx => { buttonImage.color = pressed; };
                InputActionsparams.Instance.controls.GameplayMap.LeftKick.canceled += ctx => { buttonImage.color = normal; };
                break;
            case faceButton.West:
                InputActionsparams.Instance.controls.GameplayMap.LeftPunch.started += ctx => { buttonImage.color = pressed; };
                InputActionsparams.Instance.controls.GameplayMap.LeftPunch.canceled += ctx => { buttonImage.color = normal; };
                break;
            case faceButton.East:
                InputActionsparams.Instance.controls.GameplayMap.RightKick.started += ctx => { buttonImage.color = pressed; };
                InputActionsparams.Instance.controls.GameplayMap.RightKick.canceled += ctx => { buttonImage.color = normal; };
                break;
        }
    }
}
