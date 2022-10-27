using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rank;
    [SerializeField] private TextMeshProUGUI username;
    [SerializeField] private TextMeshProUGUI score;

    // display the rank, username and score
    public void DisplayRankUserScore(int rank, UserScore userScore)
    {
        this.rank.SetText(rank.ToString());
        this.username.SetText(userScore.username);
        this.score.SetText(userScore.score.ToString());
    }
}
