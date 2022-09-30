using System;

[Serializable]
public class UserScore
{
    public String username;
    public int score;
    public UserScore(String username, int score)
    {
        this.username = username;
        this.score = score;
    }
}
