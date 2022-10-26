using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class UserScoreData
{
    private List<UserScore> userScoreList;

    public UserScoreData()
    {
        userScoreList = new List<UserScore>();
    }

    public void AddUserScore(UserScore userScore)
    {
        userScoreList.Add(userScore);
    }

    public UserScore[] GetScoresDescendingOrder()
    {
        return userScoreList.OrderByDescending(userScore => userScore.score).ToArray();
    }

    public void Clear()
    {
        userScoreList.Clear();
    }
}

