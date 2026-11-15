using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class InputActionsparams : MonoBehaviour
{

    public static InputActionsparams Instance;
    public event Action<faceButton> buttonPressedEvent;
    public event Action<direction> motionEvent;
    
    public direction lastDirection;
    public TesterInputsScript controls;
    public Task NewDelay(float seconds)
    {
        var tcs = new TaskCompletionSource<bool>();
        StartCoroutine(DelayCoroutine(seconds, tcs));
        return tcs.Task;
    }

    IEnumerator DelayCoroutine(float seconds, TaskCompletionSource<bool> tcs)
    {
        yield return new WaitForSeconds(seconds);
        tcs.SetResult(true);
    }
    public enum faceButton
    {
        North,
        West,
        South,
        East
    }

    public enum direction
    {
        Neutral,
        Forward,
        UpForward,
        Up,
        UpBack,
        Back,
        DownBack,
        Down,
        DownForward
    }

    public static direction RotateDirection(direction start,int steps, bool clockwise){
        if (start == direction.Neutral) return start;
        int dir = (int)start;
        if (clockwise){//worth optimising with mod but over full circles aren't needed
            dir = ((dir - 1 - steps) % 8 + 8) % 8 + 1;
            /*
            dir = dir - steps;
            if (dir < 1) {
                dir += 8;
            }
            */
        }
        else {
            dir = ((dir - 1 + steps) % 8 + 8) % 8 + 1;
            /*
            dir = dir + steps;
            if (dir > 8)
            {
                dir -= 8;
            }
            */
        }
        return (direction)dir;
    }

    private void Awake(){
        if (Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        
        
        Instance = this;
        controls = new TesterInputsScript();
        /*
        controls.Enable();
        */
        initialiseInputs();
        
        controls.UI.Enable();
        controls.GameplayMap.Enable();

    }
    private direction getUpdatedDirection(Vector2 inputVector) {
        const float deadzone = 0.6f;
        if (inputVector.magnitude < deadzone) { return direction.Neutral; }
        float angle = Mathf.Atan2(inputVector.y, inputVector.x) * Mathf.Rad2Deg;
        if (angle < 0) { angle += 360; }
        int sector = Mathf.RoundToInt(angle / 45f) % 8;
        switch (sector) {
            case 0: return direction.Forward;
            case 1: return direction.UpForward;
            case 2: return direction.Up;
            case 3: return direction.UpBack;
            case 4: return direction.Back;
            case 5: return direction.DownBack;
            case 6: return direction.Down;
            case 7: return direction.DownForward;
        }
        return direction.Neutral;
    }

    private void BroadcastDirection(direction newDirection) {
        if (newDirection != lastDirection) {
            motionEvent?.Invoke(newDirection);
        }
    }
    private void handleMotionInput(Vector2 inputVector) {
        //Debug.Log(inputVector);
        direction tempDirection = getUpdatedDirection(inputVector);
        BroadcastDirection(tempDirection);
        lastDirection = tempDirection;
    }

    void initialiseInputs() {
        //Debug.Log(controls.GameplayMap.LeftKick.triggered);
        controls.GameplayMap.LeftPunch.started += ctx =>{buttonPressedEvent?.Invoke(faceButton.West); };
        controls.GameplayMap.RightPunch.started += ctx => { buttonPressedEvent?.Invoke(faceButton.North); };
        controls.GameplayMap.LeftKick.started += ctx => { buttonPressedEvent?.Invoke(faceButton.South); };
        controls.GameplayMap.RightKick.started += ctx => { buttonPressedEvent?.Invoke(faceButton.East); };
        controls.GameplayMap.Movement.performed += ctx => { handleMotionInput(ctx.ReadValue<Vector2>()); };
        controls.GameplayMap.Movement.canceled += ctx => { handleMotionInput(ctx.ReadValue<Vector2>()); };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
