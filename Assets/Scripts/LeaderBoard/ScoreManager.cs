using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private UserScoreData scoreData;
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private RowUi rowUi;

    private List<RowUi> rowUiList;

    void Start()
    {
        rowUiList = new List<RowUi>();
        UpdateScoreDisplay();
        selectionManager.SelectionChanged += UpdateScoreDisplay;
    }

    void UpdateScoreDisplay()
    {
        Debug.Log("Selection changed. Update leaderboard table...");
        RetrieveUserScoreData();
        ClearRows();
        DisplayRows();
    }

    // get user score data from database
    private void RetrieveUserScoreData()
    {
        string worldSelected = selectionManager.GetWorldSelected();
        string zoneSelected = selectionManager.GetZoneSelected();
        string levelSelected = selectionManager.GetLevelSelected();

        scoreData = new UserScoreData();

        // retrieve user and total scores based on world, zone, level
        // currently add dummy values for testing
        if (worldSelected == "Addition")
        {
            scoreData.AddUserScore(new UserScore("Aug Add", 8));
            scoreData.AddUserScore(new UserScore("Mar Add", 3));
            scoreData.AddUserScore(new UserScore("Jan Add", 1));
            scoreData.AddUserScore(new UserScore("Feb Add", 2));
            scoreData.AddUserScore(new UserScore("FebTwin Add", 2));
            scoreData.AddUserScore(new UserScore("Apr Add", 4));
            scoreData.AddUserScore(new UserScore("May Add", 5));
            scoreData.AddUserScore(new UserScore("Jun Add", 6));
            scoreData.AddUserScore(new UserScore("Jul Add", 7));
            scoreData.AddUserScore(new UserScore("Sep Add", 9));
            scoreData.AddUserScore(new UserScore("Oct Add", 10));
            scoreData.AddUserScore(new UserScore("Dec Add", 12));
            scoreData.AddUserScore(new UserScore("Nov Add", 11));
        }
        else
        {
            scoreData.AddUserScore(new UserScore("Aug Others", 8));
            scoreData.AddUserScore(new UserScore("Mar Others", 3));
            scoreData.AddUserScore(new UserScore("Jan Others", 1));
            scoreData.AddUserScore(new UserScore("Feb Others", 2));
            scoreData.AddUserScore(new UserScore("FebTwin Others", 2));
            scoreData.AddUserScore(new UserScore("Apr Others", 4));
            scoreData.AddUserScore(new UserScore("May Others", 5));
            scoreData.AddUserScore(new UserScore("Jun Others", 6));
            scoreData.AddUserScore(new UserScore("Jul Others", 7));
            scoreData.AddUserScore(new UserScore("Sep Others", 9));
            scoreData.AddUserScore(new UserScore("Oct Others", 10));
            scoreData.AddUserScore(new UserScore("Dec Others", 12));
            scoreData.AddUserScore(new UserScore("Nov Others", 11));
        }
    }

    private void ClearRows()
    {
        foreach (RowUi row in rowUiList)
        {
            Destroy(row.gameObject);
        }
    }
    private void DisplayRows()
    {
        var arrangedScores = scoreData.GetScoresDescendingOrder();

        int previous_score = arrangedScores[0].score;
        int previous_rank = 1;
        for (int index = 0; index < arrangedScores.Length; index++)
        {
            RowUi row = Instantiate(rowUi, transform).GetComponent<RowUi>();
            int current_score = arrangedScores[index].score;
            int current_rank;

            // users with the same scores will have the same rank
            if (current_score == previous_score)
            {
                current_rank = previous_rank;
            }
            else
            {
                current_rank = index + 1;
            }
            previous_rank = current_rank;
            previous_score = current_score;

            row.displayRankUserScore(current_rank, arrangedScores[index]);
            rowUiList.Add(row);
        }
    }
}
