using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class MovementQueue : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class Shuffle
{
    static System.Random rand = new System.Random();
    static void FYShuff<T>(ref T[] input)
    {
        int n = input.Length;
        for(int i = 0; i < (n-1); i++)
        {
            int r = i + rand.Next(n - i);
            T t = input[r];
            input[r] = input[i];
            input[i] = t;
        }
    }
}