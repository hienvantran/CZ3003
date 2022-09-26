using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class UserScoreData
{
    private List<UserScore> userScores;

    public UserScoreData()
    {
        userScores = new List<UserScore>();
    }

    public void AddUserScore(UserScore userScore)
    {
        userScores.Add(userScore);
    }

    public IEnumerable<UserScore> GetHighScores()
    {
        return userScores.OrderByDescending(userScore => userScore.score);
    }
}

