using System.Collections.Generic;
using UnityEngine;
using static Inputs;

public class InternalTestScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RandomSeed.Instance.setSpecificSeed(1231241);

        // Create list first
        List<inputGroup> groups = new List<inputGroup>();

        groups.Add(new directionInput(
            RandomSeed.Instance.GetRandomButton(),
            RandomSeed.Instance.GetRandomDirection()
        ));

        // Add some test inputs
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));
        groups.Add(new buttonInput(RandomSeed.Instance.GetRandomButton()));

        groups.Add(new directionInput(
            RandomSeed.Instance.GetRandomButton(),
            RandomSeed.Instance.GetRandomDirection()
        ));

        groups.Add(new motionInput(
            RandomSeed.Instance.GetRandomButton(),
            RandomSeed.Instance.GetRandomDirection(),
            2,
            RandomSeed.Instance.GetRandomBool()
        ));

        //RandomSeed.Instance.ShuffleList(groups);
        // Now create the sequence
        inputSequence testSequence = new inputSequence(groups);
        
        
        // Print result
        
        Debug.Log(testSequence.sequenceToString());
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
