using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static InputActionsparams;

public class ButtonVisualController : MonoBehaviour
{


    public static ButtonVisualController Instance;
    private buttonVisuals currentVisuals;


    public List<Sprite> northSprites;
    public List<Sprite> southSprites;
    public List<Sprite> eastSprites;
    public List<Sprite> westSprites;

    public Sprite motionPad;

    public Sprite diagDirection;
    public Sprite straightDirection;

    public Sprite diagMotionStart;
    public Sprite straightMotionStart;

    public Sprite motionLine;
    public Sprite motionEnd;

    private void Awake()
    {
        Instance = this;
    }

    public enum buttonVisuals
    {
        Default,
        Pc,
        Invert,
        Shape,
        Standard
    }

    public void setNewVisuals(buttonVisuals newVisuals)
    {
        currentVisuals = newVisuals;
    }

    public buttonVisuals GetVisuals()
    {
        return currentVisuals;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var inputType = AccountManagement.instance.GetInputType();
        if (inputType.HasValue)
        {
            setNewVisuals((buttonVisuals)(int)inputType.Value);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Sprite GetButtonImage(faceButton button)
    {
        switch (button)
        {
            case faceButton.North:
                return northSprites[(int)currentVisuals];

            case faceButton.South:
                return southSprites[(int)currentVisuals];

            case faceButton.East:
                return eastSprites[(int)currentVisuals];

            case faceButton.West:
                return westSprites[(int)currentVisuals];
        }
        return null;

    }
}
