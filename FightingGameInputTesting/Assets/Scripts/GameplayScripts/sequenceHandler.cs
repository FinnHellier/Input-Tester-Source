using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static InputActionsparams;
using static Inputs;
using Image = UnityEngine.UI.Image;

public class sequenceHandler : MonoBehaviour
{
    public static sequenceHandler Instance;
    [SerializeField] private GameObject backplatePrefab;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private Transform contentParent;

    [SerializeField] private Sprite notDone;
    [SerializeField] private Sprite doing;
    [SerializeField] private Sprite complete;
    [SerializeField] private Sprite failure;


    private List<Image> backplates = new List<Image>();
    private List<Image> buttonIcons = new List<Image>();

    private void Awake()
    {
        Instance = this;
    }

    public void buildSequence(int length)
    {
        for(int x = 0; x < backplates.Count;x++)
            Destroy(backplates[x].gameObject);

        backplates.Clear();

        for (int i = 0; i < length; i++)
        {
            GameObject obj = Instantiate(backplatePrefab, contentParent);
            Image img = obj.GetComponent<Image>();

            img.sprite = notDone;

            RectTransform rect = obj.GetComponent<RectTransform>();

            float startX = -200f;
            float startY = 75f;
            float spacingX = 60f;

            rect.anchoredPosition = new Vector2(startX + (i * spacingX), startY);

            backplates.Add(img);
        }
    }

    public void updateProgress(int index)
    {
        for (int i = 0; i < backplates.Count; i++)
        {
            if (i < index)
                backplates[i].sprite = complete;
            else if (i == index)
                backplates[i].sprite = doing;
            else
                backplates[i].sprite = notDone;
        }
    }

    public void failSequence(int failPoint)
    {
        for (int i = 0; i < backplates.Count; i++)
        {
            backplates[i].sprite = notDone;
        }
        backplates[failPoint].sprite = failure;
    }

    public void progressSequence(int index)
    {
        for (int i = 0; i < backplates.Count; i++)
        {
            if (i < index)
                backplates[i].sprite = complete;
            else if (i == index)
                backplates[i].sprite = doing;
        }
    }

    public void drawButtons(inputSequence sequence) {
        for (int x = 0; x < buttonIcons.Count; x++)
            Destroy(buttonIcons[x].gameObject);

        buttonIcons.Clear();

        for (int i = 0; i < sequence.inputGroups.Count; i++)
        {
            switch (sequence.inputGroups[i]) {
                case buttonInput b:
                    drawButton(b,i);
                    break;
                case directionInput d:
                    drawDirection(d,i);
                    break;
                case motionInput m:
                    drawMotion(m,i);
                    break;
            }
        }
    }

    private float GetAngleFromDirection(direction dir)
    {
        switch (dir)
        {
            case direction.Forward: return 0f;        // →
            case direction.UpForward: return 45f;     // ↗
            case direction.Up: return 90f;            // ↑
            case direction.UpBack: return 135f;       // ↖
            case direction.Back: return 180f;         // ←
            case direction.DownBack: return -135f;    // ↙
            case direction.Down: return -90f;         // ↓
            case direction.DownForward: return -45f;  // ↘
            default: return 0f;
        }
    }

    private string feedbackMessage(groupOutput input)
    {
        switch (input)
        {
            case groupOutput.Success:
                return "Shouldn't see this";
            case groupOutput.NotNeutral:
                return "Don't input direction";
            case groupOutput.TooSlow:
                return "Too long between presses";
            case groupOutput.WrongButton:
                return "Wrong button pressed";
            case groupOutput.WrongDirection:
                return "Wrong direction";
            case groupOutput.Neutral:
                return "Input direction";
            case groupOutput.OverRotated:
                return "Pressed button past ending";
            default:
                return "Not implemented";

        }
    }

    public void spawnFeedbackMessage(groupOutput failure) {
        GameObject obj = Instantiate(messagePrefab, contentParent);
        obj.GetComponent<TMPro.TextMeshProUGUI>().text = feedbackMessage(failure);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition += new Vector2(0, -80f); // move down
    }
    private void drawMotion(motionInput m, int indexPos)
    {
        GameObject obj = spawnRight(indexPos);
        Image img = obj.GetComponent<Image>();
        img.sprite = ButtonVisualController.Instance.GetButtonImage(m.button);
        buttonIcons.Add(img);

        GameObject obj2 = spawnLeft(indexPos);
        Image img2 = obj2.GetComponent<Image>();
        img2.sprite = ButtonVisualController.Instance.motionPad;
        buttonIcons.Add(img2);


        int directionIndex = (int)m.initialDirection;
        if (directionIndex == 0) { return; }
        else if ((directionIndex - 1) % 2 == 0) {
            directionIndex = (directionIndex / 2) - 1;
            GameObject obj3 = spawnLeft(indexPos);
            Image img3 = obj3.GetComponent<Image>();
            img3.sprite = ButtonVisualController.Instance.straightMotionStart;
            obj3.transform.Rotate(0, 0, 90 * directionIndex + 90);
            buttonIcons.Add(img3);
            Color c = Color.HSVToRGB(0f, 0.6f, 0.6f);
            img3.color = c;
        }
        else {
            directionIndex = (directionIndex / 2);
            GameObject obj3 = spawnLeft(indexPos);
            Image img3 = obj3.GetComponent<Image>();
            img3.sprite = ButtonVisualController.Instance.diagMotionStart;
            
            obj3.transform.Rotate(0, 0, 90 * directionIndex - 90);
            buttonIcons.Add(img3);

            Color c = Color.HSVToRGB(0f, 0.6f,0.6f);
            img3.color = c;
        }

        /*
        int curDirection = (int)m.initialDirection; // 0-8
        int step = m.clockwise ? 1 : -1;

        */
        direction curDirection = m.initialDirection;
        if (!m.clockwise)
        {
            curDirection--;
        }

        for (int i = 0; i < m.segmentsRotated; i++)
        {
            curDirection = RotateDirection(curDirection, 1, m.clockwise);

            GameObject objLoop = spawnLeft(indexPos);
            Image imgLoop = objLoop.GetComponent<Image>();
            imgLoop.sprite = ButtonVisualController.Instance.motionLine;

            float t = (float)i / m.segmentsRotated;

            float hue = t; // rainbow
            float brightness = Mathf.Lerp(0.6f, 1f, t);
                
            Color c = Color.HSVToRGB(hue, 0.7f, brightness);
            imgLoop.color = c;

            float angle = GetAngleFromDirection(curDirection);
            objLoop.transform.localRotation = Quaternion.Euler(0, 0, angle);

            buttonIcons.Add(imgLoop);

            /*
            // Move to next direction
            curDirection += step;

            // Wrap around 1-8
            if (curDirection > 8) curDirection = 1;
            if (curDirection < 1) curDirection = 8;

            // Skip Neutral (0)
            if (curDirection == 0)
                curDirection += step > 0 ? 1 : -1;

            GameObject objLoop = spawnLeft(indexPos);
            Image imgLoop = objLoop.GetComponent<Image>();
            imgLoop.sprite = ButtonVisualController.Instance.motionLine;

            // Convert direction to rotation
            // Unity: 0 = right, positive = clockwise
            float rotationZ = (curDirection - 1) * 45f;

            objLoop.transform.localRotation = Quaternion.Euler(0, 0, rotationZ);

            buttonIcons.Add(imgLoop);
            */

            //if (m.clockwise) { objLoop.transform.scale }
        }
        GameObject objEnd = spawnLeft(indexPos);
        Image imgEnd = objEnd.GetComponent<Image>();
        imgEnd.sprite = ButtonVisualController.Instance.motionEnd;
        
        


        float angle2 = GetAngleFromDirection(curDirection);
        

        if (m.clockwise)
        {
            objEnd.transform.localRotation = Quaternion.Euler(0, 0, angle2+180);
            Vector3 scale = objEnd.transform.localScale;
            scale.x = m.clockwise ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            objEnd.transform.localScale = scale;
        }
        else {
            objEnd.transform.localRotation = Quaternion.Euler(0, 0, angle2);
        }
        Color c2 = Color.HSVToRGB(0.9f, 0.6f, 1);
        imgEnd.color = c2;

        buttonIcons.Add(imgEnd);
        /*
        GameObject obj4 = spawnLeft(indexPos);
        Image img4 = obj4.GetComponent<Image>();
        img4.sprite = ButtonVisualController.Instance.motionEnd;
        buttonIcons.Add(img4);
        */
    }

    private void drawDirection(directionInput d, int indexPos)
    {
        GameObject obj = spawnRight(indexPos);
        Image img = obj.GetComponent<Image>();
        img.sprite = ButtonVisualController.Instance.GetButtonImage(d.button);
        buttonIcons.Add(img);

        GameObject obj2 = spawnLeft(indexPos);
        Image img2 = obj2.GetComponent<Image>();
        img2.sprite = ButtonVisualController.Instance.motionPad;
        buttonIcons.Add(img2);

        int directionIndex = (int)d.buttonDirection;
        if (directionIndex == 0) { return; }
        else if ((directionIndex - 1) % 2 == 0) {
            directionIndex = (directionIndex / 2) - 1;
            GameObject obj3 = spawnLeft(indexPos);
            Image img3 = obj3.GetComponent<Image>();
            img3.sprite = ButtonVisualController.Instance.straightDirection;
            obj3.transform.Rotate(0, 0, 90 * directionIndex + 90);
            buttonIcons.Add(img3);
            Color c = Color.HSVToRGB(0f, 0.6f, 1f);
            img3.color = c;
        }
        else {
            directionIndex = (directionIndex / 2);
            GameObject obj3 = spawnLeft(indexPos);
            Image img3 = obj3.GetComponent<Image>();
            img3.sprite = ButtonVisualController.Instance.diagDirection;
            
            obj3.transform.Rotate(0, 0, 90 * directionIndex - 90);
            buttonIcons.Add(img3);
            Color c = Color.HSVToRGB(0f, 0.6f, 1f);
            img3.color = c;
        }
    }

    private void drawButton(buttonInput inputGroup,int indexPos)
    {
        GameObject obj = spawnButtonCenter(indexPos);
        Image img = obj.GetComponent<Image>();
        img.sprite = ButtonVisualController.Instance.GetButtonImage(inputGroup.button);
        buttonIcons.Add(img);
    }

    private GameObject spawnBackplateCenter(int indexPosition) {
        GameObject obj = Instantiate(backplatePrefab, contentParent);

        RectTransform rect = obj.GetComponent<RectTransform>();

        float startX = -200f;
        float startY = 75f;
        float spacingX = 60f;

        rect.anchoredPosition = new Vector2(startX + (indexPosition * spacingX), startY);

        return obj;
    }
    private GameObject spawnButtonCenter(int indexPosition)
    {
        GameObject obj = Instantiate(buttonPrefab, contentParent);

        RectTransform rect = obj.GetComponent<RectTransform>();

        float startX = -200f;
        float startY = 75f;
        float spacingX = 60f;

        rect.anchoredPosition = new Vector2(startX + (indexPosition * spacingX), startY);

        return obj;
    }
    private GameObject spawnLeft(int indexPosition)
    {
        GameObject obj = Instantiate(buttonPrefab, contentParent);

        RectTransform rect = obj.GetComponent<RectTransform>();

        float startX = -215f;
        float startY = 75f;
        float spacingX = 60f;

        rect.anchoredPosition = new Vector2(startX + (indexPosition * spacingX), startY);

        return obj;
    }
    private GameObject spawnRight(int indexPosition)
    {
        GameObject obj = Instantiate(buttonPrefab, contentParent);

        RectTransform rect = obj.GetComponent<RectTransform>();

        float startX = -185f;
        float startY = 75f;
        float spacingX = 60f;

        rect.anchoredPosition = new Vector2(startX + (indexPosition * spacingX), startY);

        return obj;
    }
}
