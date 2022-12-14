using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;


public static class SeedEncoder
{
    //Create Seed
    public static string CreateSeed(int qCount, List<string> qList)
    {
        string seed = qCount.ToString("00") + String.Concat(qList);
        return seed;
    }

    //Decode Seed
    public static (int qC, string qS) DecodeSeed(string seed)
    {
        //return params as tuple
        string questionString = seed.Substring(2).Trim();
        int questionCount = int.Parse(seed.Remove(2));
        return (questionCount, questionString);
    }

    //Create level ID
    public static string CreateLevelID()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        string id = string.Empty;

        for (int i = 0; i < 6; i++)
        {
            id += chars[UnityEngine.Random.Range(0, chars.Length)];
        }

        return id;
    }
}
