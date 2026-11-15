using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;



public class Inputs
{

    public enum groupOutput
    {
        Success,
        TooSlow,
        WrongButton,
        WrongDirection,
        OverRotated,
        NotNeutral,
        Neutral,
        TooEarly
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
    public static direction RotateDirection(direction start, int steps, bool clockwise)
    {
        if (start == direction.Neutral) return start;
        int dir = (int)start;
        if (clockwise)
        {
            dir = ((dir - 1 - steps) % 8 + 8) % 8 + 1;

        }
        else
        {
            dir = ((dir - 1 + steps) % 8 + 8) % 8 + 1;

        }
        return (direction)dir;
    }
    public class inputSequence
    {
        public IReadOnlyList<inputGroup> inputGroups { get; }
        int sequenceCounter;

        public inputSequence(IReadOnlyList<inputGroup> inputGroups)
        {
            this.inputGroups = inputGroups;

        }

        public void initialiseSequence()
        {



        }

        //public void generateSequence() { }

        public string sequenceToString()
        {
            string returnString = "";
            for (int x = 0; x < inputGroups.Count; x++)
            {
                returnString += inputGroups[x].convertToString() + ", ";
            }
            return returnString;
        }

    }
    public abstract class inputGroup
    {

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
            return buttonName[button] + "+" + directionName[buttonDirection];
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
        

    }



}
