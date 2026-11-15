using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static InputActionsparams;
using static Inputs;

public class GamePlay : MonoBehaviour
{
    int challenge;

    int previousAttempt = 0;
    public int getPreviousAttempt() {
        int temp = previousAttempt;
        previousAttempt = 0;
        return temp;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        updatePeviousProgress();
        _ = runInputChallenges();
    }


    void updatePeviousProgress() {
        if (AccountManagement.instance.GetStage() == null){
            int seed = RandomSeed.Instance.generateSeed();
            RandomSeed.Instance.setSpecificSeed(seed);
            AccountManagement.instance.SetSeed(seed);
            ServerCommunication.Instance.CreateRun(AccountManagement.instance.GetID(),(int)AccountManagement.instance.GetInputType().Value, seed);
            challenge = 0;
        }
        else {
            RandomSeed.Instance.setSpecificSeed((int)AccountManagement.instance.GetSeed());
            previousAttempt = AccountManagement.instance.GetAttempts() ?? 0;
            challenge = (int)AccountManagement.instance.GetStage();
            SkipToSequence(challenge);
        }
    }

    public async Task runInputChallenges(){
        while (challenge < 23){
            inputSequence sequence = GenerateInputGroup(challenge);
            sequenceHandler.Instance.buildSequence(sequence.inputGroups.Count);
            sequenceHandler.Instance.drawButtons(sequence);

            for (int x = getPreviousAttempt(); x < 63; x++){
                var (sequenceAttempt, failIndex) = await runSequence(sequence);
                byte attemptByte = encodeAttempt(sequenceAttempt, failIndex);
                AccountManagement.instance.SetAttempts(x + 1);
                ServerCommunication.Instance.SubmitAttempt(AccountManagement.instance.GetID(),attemptByte,
                    onSuccess: succeeded => {
                        if (!succeeded)
                            sequenceHandler.Instance.spawnFeedbackMessage(sequenceAttempt);
                    }

                );

                await Awaitable.WaitForSecondsAsync(0.2f);
                if (sequenceAttempt == groupOutput.Success)
                    break;
            }
            AccountManagement.instance.SetAttempts(0);
            
            challenge++;
            AccountManagement.instance.SetStage(challenge);//call after challenge++ to avoid 
        }
        AccountManagement.instance.DeleteStage();
        AccountManagement.instance.GoToEnd();
    }

    byte encodeAttempt(groupOutput result, int failStep)
    {
        byte b = 0;
        if (result == groupOutput.Success) b |= (1 << 7);
        if (result == groupOutput.TooSlow) b |= (1 << 6);
        b |= (byte)(failStep & 0x3F);
        return b;
    }
    public async Task<(groupOutput result, int failIndex)> runSequence(inputSequence sequence)
    {
        for (int x = 0; x < sequence.inputGroups.Count; x++)
        {
            
            groupOutput inputAttempt = await sequence.inputGroups[x].inputTest(x == 0);
            if (inputAttempt != groupOutput.Success) {
                sequenceHandler.Instance.failSequence(x);
                return (inputAttempt, x);
                //display failure message
            }
            sequenceHandler.Instance.progressSequence(x);
        }
        return (groupOutput.Success, 0);
    }

    /*
    public async Task<groupOutput> runInput()
    {

        return groupOutput.Success;
    }
    */





    
    /*
     public void unittest(){
        for (int x = 0; x < 10; x++)
        {
            RandomSeed.Instance.setSpecificSeed(x);
            for (int y = 0; y < 22; y++)
            {
                Debug.Log(x + "challenge "+ y);
                inputSequence testSeq = GenerateInputGroup(y);
                Debug.Log(testSeq.sequenceToString());
            }
        }
        for (int x = 0; x < 10; x++)
        {
            RandomSeed.Instance.setSpecificSeed(x);
            SkipToSequence(17);
            for (int y = 17; y < 22; y++)
            {
                Debug.Log("seed "+ x + " challenge " + y);
                inputSequence testSeq = GenerateInputGroup(y);
                Debug.Log(testSeq.sequenceToString());
            }
        }
    }
    */
        

    // Update is called once per frame
    void Update()
    {
        
    }

    public inputSequence GenerateInputGroup(int number)
    {
        switch (number / 3)
        {
            case 0: return tutorialGroup();
            case 1: return directionalGroup();
            case 2: return directionalButtonGroup();
            case 3: return directionalShuffleGroup();
            case 4: return motionGroup();
            case 5: return motionButtonGroup();
            case 6: return motionDirectionalButtonGroup();
            case 7: return motionShuffleGroup();
            default: return null;
        }
    }

    public void SkipInputGroup(int number)
    {
        switch (number / 3)
        {
            case 0: SkipTutorialGroup(); break;
            case 1: SkipDirectionalGroup(); break;
            case 2: SkipDirectionalButtonGroup(); break;
            case 3: SkipDirectionalShuffleGroup(); break;
            case 4: SkipMotionGroup(); break;
            case 5: SkipMotionButtonGroup(); break;
            case 6: SkipMotionDirectionalButtonGroup(); break;
            case 7: SkipMotionShuffleGroup(); break;
        }
    }

    private void SkipToSequence(int skipTo) {
        for (int x = 0; x < skipTo; x++) {
            SkipInputGroup(x);
        }
    }



    private void SkipMotionShuffleGroup()
    {
        List<inputGroup> groups = new List<inputGroup>();

        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new directionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection()));
        groups.Add(new directionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection()));
        groups.Add(new motionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection(), RandomSeed.Instance.GetRandomRotation(), RandomSeed.Instance.GetRandomBool()));
        groups.Add(new motionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection(), RandomSeed.Instance.GetRandomRotation(), RandomSeed.Instance.GetRandomBool()));

        RandomSeed.Instance.ShuffleList(groups);

        inputSequence Sequence = new inputSequence(groups);
    }

    private void SkipMotionDirectionalButtonGroup()
    {
        List<inputGroup> groups = new List<inputGroup>();
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new directionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection()));
        groups.Add(new directionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection()));
        RandomSeed.Instance.ShuffleList(groups);

        groups.Add(new motionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection(), RandomSeed.Instance.GetRandomRotation(), RandomSeed.Instance.GetRandomBool()));

        inputSequence Sequence = new inputSequence(groups);
    }

    private void SkipMotionButtonGroup()
    {
        List<inputGroup> groups = new List<inputGroup>();

        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        RandomSeed.Instance.ShuffleList(groups);
        groups.Add(new motionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection(), RandomSeed.Instance.GetRandomRotation(), RandomSeed.Instance.GetRandomBool()));

        inputSequence Sequence = new inputSequence(groups);
    }

    private void SkipMotionGroup()
    {
        List<inputGroup> groups = new List<inputGroup>();

        groups.Add(new motionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection(), RandomSeed.Instance.GetRandomRotation(), RandomSeed.Instance.GetRandomBool()));

        inputSequence Sequence = new inputSequence(groups);
    }

    private void SkipDirectionalShuffleGroup()
    {
        List<inputGroup> groups = new List<inputGroup>();

        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new directionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection()));
        groups.Add(new directionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection()));
        RandomSeed.Instance.ShuffleList(groups);

        inputSequence Sequence = new inputSequence(groups);
    }

    private void SkipDirectionalButtonGroup()
    {
        List<inputGroup> groups = new List<inputGroup>();

        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        RandomSeed.Instance.ShuffleList(groups);
        groups.Add(new directionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection()));

        inputSequence Sequence = new inputSequence(groups);
    }

    private void SkipDirectionalGroup()
    {
        List<inputGroup> groups = new List<inputGroup>();

        groups.Add(new directionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection()));

        inputSequence Sequence = new inputSequence(groups);
    }

    private void SkipTutorialGroup()
    {
        List<inputGroup> groups = new List<inputGroup>();

        groups.Add(new buttonInput(faceButton.North));
        groups.Add(new buttonInput(faceButton.West));
        groups.Add(new buttonInput(faceButton.South));
        groups.Add(new buttonInput(faceButton.East));
        RandomSeed.Instance.ShuffleList(groups);

        inputSequence Sequence = new inputSequence(groups);
    }

    public inputSequence tutorialGroup() {
        List<inputGroup> groups = new List<inputGroup>();

        groups.Add(new buttonInput(faceButton.North));
        groups.Add(new buttonInput(faceButton.West));
        groups.Add(new buttonInput(faceButton.South));
        groups.Add(new buttonInput(faceButton.East));
        RandomSeed.Instance.ShuffleList(groups);

        inputSequence Sequence = new inputSequence(groups);
        return Sequence;

    } // xyba random
    public inputSequence directionalGroup() {
        List<inputGroup> groups = new List<inputGroup>();

        groups.Add(new directionInput(RandomSeed.Instance.GetRandomButton(),RandomSeed.Instance.GetRandomDirection()));

        inputSequence Sequence = new inputSequence(groups);
        return Sequence;
    } //single button+direction
    public inputSequence directionalButtonGroup() {
        List<inputGroup> groups = new List<inputGroup>();

        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        RandomSeed.Instance.ShuffleList(groups);
        groups.Add(new directionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection()));

        inputSequence Sequence = new inputSequence(groups);
        return Sequence;
    } // 4 buttons + direction with button
    public inputSequence directionalShuffleGroup() {
        List<inputGroup> groups = new List<inputGroup>();

        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new directionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection()));
        groups.Add(new directionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection()));
        RandomSeed.Instance.ShuffleList(groups);

        inputSequence Sequence = new inputSequence(groups);
        return Sequence;
    } // 3 buttons with 3 directions + button
    public inputSequence motionGroup() {
        List<inputGroup> groups = new List<inputGroup>();

        groups.Add(new motionInput(RandomSeed.Instance.GetRandomButton(),RandomSeed.Instance.GetRandomDirection(),RandomSeed.Instance.GetRandomRotation(),RandomSeed.Instance.GetRandomBool()));

        inputSequence Sequence = new inputSequence(groups);
        return Sequence;
    } //single motion+button
    public inputSequence motionButtonGroup() {
        List<inputGroup> groups = new List<inputGroup>();

        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        RandomSeed.Instance.ShuffleList(groups);
        groups.Add(new motionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection(),RandomSeed.Instance.GetRandomRotation(),RandomSeed.Instance.GetRandomBool()));

        inputSequence Sequence = new inputSequence(groups);
        return Sequence;
    } //  4 buttons + motion with button
    public inputSequence motionDirectionalButtonGroup() {
        List<inputGroup> groups = new List<inputGroup>();
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new directionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection()));
        groups.Add(new directionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection()));
        RandomSeed.Instance.ShuffleList(groups);

        groups.Add(new motionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection(), RandomSeed.Instance.GetRandomRotation(), RandomSeed.Instance.GetRandomBool()));

        inputSequence Sequence = new inputSequence(groups);
        return Sequence;
    }//2 buttons with direction 2 buttons + motion with button at the end

    public inputSequence motionShuffleGroup() {
        List<inputGroup> groups = new List<inputGroup>();

        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new directionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection()));
        groups.Add(new directionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection()));
        groups.Add(new motionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection(), RandomSeed.Instance.GetRandomRotation(), RandomSeed.Instance.GetRandomBool()));
        groups.Add(new motionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection(), RandomSeed.Instance.GetRandomRotation(), RandomSeed.Instance.GetRandomBool()));

        RandomSeed.Instance.ShuffleList(groups);

        inputSequence Sequence = new inputSequence(groups);
        return Sequence;
    } //3 random buttons, directional buttons and motion buttons

    public inputSequence testShuffleGroup() //testing only
    {
        List<inputGroup> groups = new List<inputGroup>();

        groups.Add(new motionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection(), RandomSeed.Instance.GetRandomRotation(), RandomSeed.Instance.GetRandomBool()));
        groups.Add(new motionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection(), RandomSeed.Instance.GetRandomRotation(), RandomSeed.Instance.GetRandomBool()));
        groups.Add(new motionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection(), RandomSeed.Instance.GetRandomRotation(), RandomSeed.Instance.GetRandomBool()));
        groups.Add(new motionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection(), RandomSeed.Instance.GetRandomRotation(), RandomSeed.Instance.GetRandomBool()));
        groups.Add(new motionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection(), RandomSeed.Instance.GetRandomRotation(), RandomSeed.Instance.GetRandomBool()));
        groups.Add(new motionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection(), RandomSeed.Instance.GetRandomRotation(), RandomSeed.Instance.GetRandomBool()));
        groups.Add(new motionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection(), RandomSeed.Instance.GetRandomRotation(), RandomSeed.Instance.GetRandomBool()));
        groups.Add(new motionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection(), RandomSeed.Instance.GetRandomRotation(), RandomSeed.Instance.GetRandomBool()));
        groups.Add(new motionInput(RandomSeed.Instance.GetRandomButton(), RandomSeed.Instance.GetRandomDirection(), RandomSeed.Instance.GetRandomRotation(), RandomSeed.Instance.GetRandomBool()));

        RandomSeed.Instance.ShuffleList(groups);

        inputSequence Sequence = new inputSequence(groups);
        return Sequence;
    } //3 random buttons, directional buttons and motion buttons


}
