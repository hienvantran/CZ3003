using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;


public static class SeedEncoder
{
    //class to hold the variables
    public class JsonObj
    {
        public int questionCount;
        public List<string> questionList;
    }

    //Create Seed
    public static string CreateSeed(int qCount, List<string> qList)
    {
        //get json obj
        JsonObj seedThis = new JsonObj();
        seedThis.questionCount = qCount;
        seedThis.questionList = qList;
        string json = JsonUtility.ToJson(seedThis);

        //encode seed
        byte[] encodeBytes = Encoding.UTF8.GetBytes(json);
        string encodedString = Convert.ToBase64String(encodeBytes);
        return encodedString;
    }

    //Decode Seed
    public static (int qC, List<string> qL) DecodeSeed(string seed)
    {
        ///decode seed
        byte[] decodeBytes = Convert.FromBase64String(seed);
        string decodedString = Encoding.UTF8.GetString(decodeBytes);

        //get json obj
        JsonObj unseedThis = JsonUtility.FromJson<JsonObj>(decodedString);

        //return params as tuple
        return  (unseedThis.questionCount, unseedThis.questionList);
    }
}
