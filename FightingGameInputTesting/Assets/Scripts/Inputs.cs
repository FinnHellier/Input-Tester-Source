using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UIElements;
using static InputActionsparams;



public class Inputs
{
    const int TimeoutMs = 500; // 1/60*5 83 //made longer
    public enum groupOutput {
        Success,
        TooSlow,
        WrongButton,
        WrongDirection,
        OverRotated,
        NotNeutral,
        Neutral,
        TooEarly
    }
    /*
    Dictionary<groupOutput, Action> inputActions = new()
    {}
    */

    //input sequence contains input groups
    //input groups can contain a single button, button and direction or button direction and rotation
    public class inputSequence {
        public IReadOnlyList<inputGroup> inputGroups { get; }
        int sequenceCounter;

        public inputSequence(IReadOnlyList<inputGroup> inputGroups) {
            this.inputGroups = inputGroups;

        }

        public void initialiseSequence() {
        
        
        
        }

        //public void generateSequence() { }

        public string sequenceToString() {
            string returnString = "";
            for (int x = 0; x < inputGroups.Count; x++) {
                returnString += inputGroups[x].convertToString() +", " ;
            }
            return returnString;
        }
        
    }
    public abstract class inputGroup {
        
        protected static readonly Dictionary<faceButton, string> buttonName = new Dictionary<faceButton, string>() {
            { faceButton.North, "2" },
            { faceButton.West, "1" },
            { faceButton.South, "3" },
            { faceButton.East, "4" }
        };

        protected static readonly Dictionary<direction, string> directionName = new Dictionary<direction, string>() {
            { direction.Forward, "→" },
            { direction.UpForward, "↗" },
            { direction.Up, "↑" },
            { direction.UpBack, "↖" },
            { direction.Back, "←" },
            { direction.DownBack, "↙" },
            { direction.Down, "↓" },
            { direction.DownForward, "↘" },
            { direction.Neutral, "☆" }
        };


        public abstract string convertToString();

        public abstract Task<groupOutput> inputTest(bool isFirstInput); //async
    }

    public class buttonInput : inputGroup
    {
        public faceButton button;

        public buttonInput(faceButton button)
        {
            this.button = button;
        }
        public override string convertToString()
        {
            /* 
            string returnString = "";
            returnString += buttonName[button];
            return returnString;
            */
            return buttonName[button];
        }

        public override async Task<groupOutput> inputTest(bool isFirstInput)
        {

            var tcs = new TaskCompletionSource<groupOutput>();

            void OnButtonPressed(faceButton pressed){
                if (pressed == button)
                    tcs.TrySetResult(groupOutput.Success);
                else
                    tcs.TrySetResult(groupOutput.WrongButton);
            }

            InputActionsparams.Instance.buttonPressedEvent += OnButtonPressed;
            groupOutput result;

            if (isFirstInput){ result = await tcs.Task;}
            else{
                var timeoutTask = InputActionsparams.Instance.NewDelay(TimeoutMs / 1000f);
                var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);
                if (completedTask == timeoutTask)
                {

                    InputActionsparams.Instance.buttonPressedEvent -= OnButtonPressed; //don't cause another crash
                    return groupOutput.TooSlow;
                }

                result = await tcs.Task;
            }
            if (InputActionsparams.Instance.lastDirection != direction.Neutral) {
                return groupOutput.NotNeutral;
            }

            InputActionsparams.Instance.buttonPressedEvent -= OnButtonPressed;//don't cause another crash
            return result;

        }
    }
    public class directionInput : inputGroup
    {
        public faceButton button;
        public direction buttonDirection;


        public directionInput(faceButton button, direction buttonDirection)
        {
            this.button = button;
            this.buttonDirection = buttonDirection;
        }
        public override string convertToString()
        {
            return buttonName[button] +"+"+ directionName[buttonDirection];
        }

        public async override Task<groupOutput> inputTest(bool isFirstInput)
        {
            var tcs = new TaskCompletionSource<groupOutput>();

            void OnButtonPressed(faceButton pressed)
            {
                if (pressed == button)
                    tcs.TrySetResult(groupOutput.Success);
                else
                    tcs.TrySetResult(groupOutput.WrongButton);
            }

            InputActionsparams.Instance.buttonPressedEvent += OnButtonPressed;
            groupOutput result;

            if (isFirstInput) { result = await tcs.Task; }
            else
            {
                var timeoutTask = InputActionsparams.Instance.NewDelay(TimeoutMs / 1000f);
                var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);
                if (completedTask == timeoutTask)
                {

                    InputActionsparams.Instance.buttonPressedEvent -= OnButtonPressed; //don't cause another crash
                    return groupOutput.TooSlow;
                }

                result = await tcs.Task;
            }
            if (InputActionsparams.Instance.lastDirection != buttonDirection)
            {
                return groupOutput.WrongDirection;
            }

            InputActionsparams.Instance.buttonPressedEvent -= OnButtonPressed;//don't cause another crash
            return result;
        }
    }

    public class motionInput : inputGroup
    {

        public faceButton button;
        public direction initialDirection;
        public int segmentsRotated;
        public bool clockwise;

        public motionInput(faceButton button, direction initialDirection, int segmentsRotated, bool clockwise)
        {
            this.button = button;
            this.initialDirection = initialDirection;
            this.segmentsRotated = segmentsRotated;
            this.clockwise = clockwise;
        }

        public override string convertToString()
        {
            string returnString = "";
            returnString += directionName[initialDirection];
            direction tempDirection = initialDirection;
            for (int x = 0; x < segmentsRotated; x++)
            {
                tempDirection = RotateDirection(tempDirection, 1, clockwise);
                returnString += directionName[tempDirection];
            }
            returnString += "+" + buttonName[button];
            return returnString;
        }
        public async override Task<groupOutput> inputTest(bool isFirstInput){

            var earlyPressTcs = new TaskCompletionSource<groupOutput>();
            void OnEarlyButton(faceButton pressed) => earlyPressTcs.TrySetResult(groupOutput.TooEarly);
            InputActionsparams.Instance.buttonPressedEvent += OnEarlyButton;
            {
                var tcs = new TaskCompletionSource<bool>();

                if (InputActionsparams.Instance.lastDirection == initialDirection)//checks if cur direction is already in position
                    tcs.TrySetResult(true);
                void OnDirectionChanged(direction current)
                {
                    if (current == initialDirection) tcs.TrySetResult(true);
                }
                InputActionsparams.Instance.motionEvent += OnDirectionChanged;
                if (isFirstInput) //doesn't use timer
                    await Task.WhenAny(tcs.Task, earlyPressTcs.Task);
                else
                {
                    var timeout = InputActionsparams.Instance.NewDelay(TimeoutMs / 1000f);
                    await Task.WhenAny(tcs.Task, earlyPressTcs.Task, timeout);
                    if (!tcs.Task.IsCompleted) { InputActionsparams.Instance.motionEvent -= OnDirectionChanged; InputActionsparams.Instance.buttonPressedEvent -= OnEarlyButton; return earlyPressTcs.Task.IsCompleted ? groupOutput.TooEarly : groupOutput.TooSlow; }
                }
                InputActionsparams.Instance.motionEvent -= OnDirectionChanged; //avoid overflow
                if (earlyPressTcs.Task.IsCompleted) { InputActionsparams.Instance.buttonPressedEvent -= OnEarlyButton; return groupOutput.TooEarly; }
            }

            if (segmentsRotated==1) {//avoids weird behavior on single input
                direction expectedDir = initialDirection;
                for (int i = 0; i < segmentsRotated; i++){
                    expectedDir = RotateDirection(expectedDir, 1, clockwise);

                    var tcs = new TaskCompletionSource<bool>();
                    if (InputActionsparams.Instance.lastDirection == expectedDir)
                        tcs.TrySetResult(true);
                        void OnDirectionChanged(direction current){
                        if (current == direction.Neutral) return;
                        if (current == expectedDir) tcs.TrySetResult(true);
                        else tcs.TrySetResult(false);
                    }
                    InputActionsparams.Instance.motionEvent += OnDirectionChanged;
                    var timeout = InputActionsparams.Instance.NewDelay(TimeoutMs / 1000f);
                    await Task.WhenAny(tcs.Task, earlyPressTcs.Task, timeout);
                    InputActionsparams.Instance.motionEvent -= OnDirectionChanged;

                    if (earlyPressTcs.Task.IsCompleted) { InputActionsparams.Instance.buttonPressedEvent -= OnEarlyButton; return groupOutput.TooEarly; }
                    if (!tcs.Task.IsCompleted) { InputActionsparams.Instance.buttonPressedEvent -= OnEarlyButton; return groupOutput.TooSlow; }
                    if (!tcs.Task.Result) { InputActionsparams.Instance.buttonPressedEvent -= OnEarlyButton; return groupOutput.OverRotated; }


                }
            }

            InputActionsparams.Instance.buttonPressedEvent -= OnEarlyButton;//final step and button
            var buttonTcs = new TaskCompletionSource<groupOutput>();
            void OnButtonPressed(faceButton pressed)
            {
                if (pressed == button) buttonTcs.TrySetResult(groupOutput.Success);
                else buttonTcs.TrySetResult(groupOutput.WrongButton);
            }
            InputActionsparams.Instance.buttonPressedEvent += OnButtonPressed;
            var buttonTimeout = InputActionsparams.Instance.NewDelay(TimeoutMs / 1000f);
            var completed = await Task.WhenAny(buttonTcs.Task, buttonTimeout);
            InputActionsparams.Instance.buttonPressedEvent -= OnButtonPressed;
            if (completed == buttonTimeout) return groupOutput.TooSlow;
            return await buttonTcs.Task;
        }

        }



        }
