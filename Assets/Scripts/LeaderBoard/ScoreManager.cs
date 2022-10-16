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
    [SerializeField] private RowUi rowUi;

    private List<RowUi> rowUiList;

    void Start()
    {
        rowUiList = new List<RowUi>();
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
        List<(string, string)> selectedWorldsLevels = selectionManager.getSelectedWorldsLevels();
        scoreData = new UserScoreData();

        scoreSelectedLevels = new Dictionary<string, int>();

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
        var getUsernameFromUid = FirestoreManager.Instance.getUsernamebyID(uid,
            res =>
            {
                current_username = res;
            }
        );
        yield return new WaitUntil(predicate: () => getUsernameFromUid.IsCompleted);
    }

    private IEnumerator RetrieveUserScoreDataSingleLevel(string world, string level)
    {
        string levelId = WorldLevelParser.formatIdFromWorldLevel(world, level);
        var getScoresForSelectedContent = FirestoreManager.Instance.getLevelAttemptsbyID(levelId,
            res =>
            {
                addUserAttemptsSelectedLevel(res);
            });
        yield return new WaitUntil(predicate: () => getScoresForSelectedContent.IsCompleted);
    }

    private void addUserAttemptsSelectedLevel(List<Dictionary<string, object>> attempts)
    {
        foreach (Dictionary<string, object> userAttempt in attempts)
        {
            string uid = userAttempt["uid"].ToString();
            if (!scoreSelectedLevels.ContainsKey(uid))
            {
                scoreSelectedLevels[uid] = 0;
            }
            scoreSelectedLevels[uid] += int.Parse(userAttempt["score"].ToString());
        }
    }

    private void ClearRows()
    {
        foreach (RowUi row in rowUiList)
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
