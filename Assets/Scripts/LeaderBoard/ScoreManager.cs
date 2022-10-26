using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class ScoreManager : MonoBehaviour
{
    private UserScoreData scoreData;
    private Dictionary<string, int> scoreSelectedLevels;
    private string current_username;
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private RowUI rowUi;

    private List<RowUI> rowUiList;

    void Start()
    {
        scoreData = new UserScoreData();
        rowUiList = new List<RowUI>();
        scoreSelectedLevels = new Dictionary<string, int>();
        selectionManager.SelectionChanged += UpdateScoreDisplay;
    }

    void UpdateScoreDisplay()
    {
        Debug.Log("Selection changed. Update leaderboard table...");
        StartCoroutine(RetrieveUserScoreData());
    }

    // get user score data from database
    public IEnumerator RetrieveUserScoreData()
    {
        List<(string, string)> selectedWorldsLevels = selectionManager.GetSelectedWorldsLevels();
        scoreData.Clear();
        scoreSelectedLevels.Clear();


        foreach ((string worldSelected, string levelSelected) in selectedWorldsLevels)
        {
            yield return StartCoroutine(RetrieveUserScoreDataSingleLevel(worldSelected, levelSelected));
        }

        Debug.Log("Done getting user scores from selected world and level");

        foreach (KeyValuePair<string, int> userScorePair in scoreSelectedLevels)
        {
            string uid = userScorePair.Key;
            yield return StartCoroutine(GetUserNameFromUid(uid));
            scoreData.AddUserScore(new UserScore(current_username, userScorePair.Value));
        }
        ClearRows();
        DisplayRows();
    }

    private IEnumerator GetUserNameFromUid(string uid)
    {
        var getUsernameFromUid = FirestoreManager.Instance.GetUsernamebyID(uid,
            res =>
            {
                current_username = res;
            }
        );
        yield return new WaitUntil(predicate: () => getUsernameFromUid.IsCompleted);
    }

    private IEnumerator RetrieveUserScoreDataSingleLevel(string world, string level)
    {
        if (world != SelectionManager.assignmentWorldKey)
        {
            string levelId = WorldLevelParser.formatIdFromWorldLevel(world, level);
            var getScoresForSelectedContent = FirestoreManager.Instance.GetLevelAttemptsbyID(levelId,
                res =>
                {
                    AddUserAttemptsSelectedLevel(res);
                });
            yield return new WaitUntil(predicate: () => getScoresForSelectedContent.IsCompleted);
        }
        else
        {
            string assignId = level;
            var getScoresForSelectedContent = FirestoreManager.Instance.GetAssignmentAttemptsbyID(assignId,
                res =>
                {
                    AddUserAttemptsSelectedLevel(res);
                });
            yield return new WaitUntil(predicate: () => getScoresForSelectedContent.IsCompleted);
        }
    }

    private void AddUserAttemptsSelectedLevel(List<Dictionary<string, object>> attempts)
    {
        foreach (Dictionary<string, object> userAttempt in attempts)
        {
            string uid = userAttempt["uid"].ToString();
            if (!scoreSelectedLevels.ContainsKey(uid))
            {
                scoreSelectedLevels.Add(uid, 0);
            }
            scoreSelectedLevels[uid] += int.Parse(userAttempt["score"].ToString());
        }
    }

    private void ClearRows()
    {
        foreach (RowUI row in rowUiList)
        {
            Destroy(row.gameObject);
        }
        rowUiList.Clear();
    }
    private void DisplayRows()
    {
        var arrangedScores = scoreData.GetScoresDescendingOrder();
        if (arrangedScores.Length == 0) return;

        int previous_score = arrangedScores[0].score;
        int previous_rank = 1;
        for (int index = 0; index < arrangedScores.Length; index++)
        {
            RowUI row = Instantiate(rowUi, transform).GetComponent<RowUI>();
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

            row.DisplayRankUserScore(current_rank, arrangedScores[index]);
            rowUiList.Add(row);
        }
    }
}
