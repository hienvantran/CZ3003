using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private UserScoreData scoreData;

    public static ScoreManager instance;

    // get user score data from database
    private void getUserScoreData()
    {
        scoreData = new UserScoreData();

        // currently add dummy values for testing
        scoreData.AddUserScore(new UserScore("Aug", 8));
        scoreData.AddUserScore(new UserScore("Mar", 3));
        scoreData.AddUserScore(new UserScore("Jan", 1));
        scoreData.AddUserScore(new UserScore("Feb", 2));
        scoreData.AddUserScore(new UserScore("Apr", 4));
        scoreData.AddUserScore(new UserScore("May", 5));
        scoreData.AddUserScore(new UserScore("Jun", 6));
        scoreData.AddUserScore(new UserScore("Jul", 7));
        scoreData.AddUserScore(new UserScore("Sep", 9));
        scoreData.AddUserScore(new UserScore("Oct", 10));
        scoreData.AddUserScore(new UserScore("Dec", 12));
        scoreData.AddUserScore(new UserScore("Nov", 11));
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        getUserScoreData();

        // Add some dummy data for testing
    }

    public UserScore[] GetHighScores()
    {
        return scoreData.GetHighScores().ToArray();
    }
}
