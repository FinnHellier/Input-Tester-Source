using System;
using System.Collections.Generic;
using UnityEngine;
using static InputActionsparams;

public class RandomSeed : MonoBehaviour
{
    public static RandomSeed Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private System.Random randomFactory;
    private int seed;
    public void setSeed() {
        seed = generateSeed();
        randomFactory = new System.Random(seed);
    }
    public int getSeed() { return seed; }
    public void setSpecificSeed(int specificSeed) {
        seed = specificSeed;
        randomFactory = new System.Random(seed);
    }

    public int generateSeed() {
        return System.Environment.TickCount;
    }

    void loadSeed() {
        /*
         * 
        if (PlayerPrefs.HasKey("eed"))
        {
            seed = PlayerPrefs.GetInt("seed");
        }
        PlayerPrefs.SetInt("seed", seed);
        PlayerPrefs.Save();
        */
    }

    private static direction[] motionDirections =
        Array.FindAll(
            (direction[])Enum.GetValues(typeof(direction))
            , d => d != direction.Neutral)//filters out neutral
        ;

    private static faceButton[] buttons =
    (faceButton[])Enum.GetValues(typeof(faceButton));

    public direction GetRandomDirection()
    {
        return motionDirections[randomFactory.Next(motionDirections.Length)];
    }

    public faceButton GetRandomButton()
    {
        return buttons[randomFactory.Next(buttons.Length)];
    }

    public void ShuffleList<T>(List<T> list) {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = randomFactory.Next(i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    public bool GetRandomBool()
    {
        return randomFactory.Next(0, 2) == 1;
    }

    public int GetRandomRotation()
    {
        return randomFactory.Next(1, 8);
    }


    //void GenerateButton() { }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
