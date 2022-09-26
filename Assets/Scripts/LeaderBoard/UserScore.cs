using System;

[Serializable]
public class UserScore
{
    public String username;
    public float score;
    public UserScore(String username, float score)
    {
        this.username = username;
        this.score = score;
    }
}
