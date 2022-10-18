using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;

[Serializable]
public class UserAttemptSummary
{
    public int numLevelsAttempted;
    public int sumCorrectAnswers;
    public int totalFailsBeforePassing;
    public UserAttemptSummary()
    {
        numLevelsAttempted = 0;
        sumCorrectAnswers = 0;
        totalFailsBeforePassing = 0;
    }
}


public class AnalyticsManager : MonoBehaviour
{
    private UserScoreData scoreData;
    private Dictionary<string, UserAttemptSummary> attemptsSelectedLevels;
    private string current_username;
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private TextMeshProUGUI numUniqueUsers;
    [SerializeField] private TextMeshProUGUI avgFailsBeforeFirstPass;
    [SerializeField] private TextMeshProUGUI avgCorrectAnswersBestAttempt;

    private List<RowUi> rowUiList;

    void Start()
    {
        rowUiList = new List<RowUi>();
        selectionManager.SelectionChanged += UpdateScoreDisplay;
    }

    void UpdateScoreDisplay()
    {
        Debug.Log("Selection changed. Update analytics report...");
        StartCoroutine(RetrieveUserAttempts());
    }

    // get user attempts data from database
    public IEnumerator RetrieveUserAttempts()
    {
        List<(string, string)> selectedWorldsLevels = selectionManager.getSelectedWorldsLevels();
        scoreData = new UserScoreData();

        attemptsSelectedLevels = new Dictionary<string, UserAttemptSummary>();

        foreach ((string worldSelected, string levelSelected) in selectedWorldsLevels)
        {
            yield return StartCoroutine(RetrieveUserScoreDataSingleLevel(worldSelected, levelSelected));
        }

        Debug.Log("Done getting user scores from selected world and level");

        int numUsers = attemptsSelectedLevels.Count();
        int numLevelsAttemptedAllUsers = 0;
        int sumCorrectAnswersAllUsers = 0;
        int totalFailsBeforePassingAllUsers = 0;
        foreach (KeyValuePair<string, UserAttemptSummary> userScorePair in attemptsSelectedLevels)
        {
            UserAttemptSummary summary = userScorePair.Value;

            sumCorrectAnswersAllUsers += summary.sumCorrectAnswers;
            numLevelsAttemptedAllUsers += summary.numLevelsAttempted;
            totalFailsBeforePassingAllUsers += summary.totalFailsBeforePassing;
        }
        DisplayAnalytics(numUsers, sumCorrectAnswersAllUsers, totalFailsBeforePassingAllUsers, numLevelsAttemptedAllUsers);
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
            if (!attemptsSelectedLevels.ContainsKey(uid))
            {
                attemptsSelectedLevels[uid] = new UserAttemptSummary();
            }
            attemptsSelectedLevels[uid].numLevelsAttempted++;
            attemptsSelectedLevels[uid].sumCorrectAnswers += int.Parse(userAttempt["correct"].ToString());
            attemptsSelectedLevels[uid].totalFailsBeforePassing += int.Parse(userAttempt["fail"].ToString());
        }
    }

    private void DisplayAnalytics(int numUsers, int sumCorrectAnswersAllUsers, int totalFailsBeforePassingAllUsers, int numLevelsAttemptedAllUsers)
    {
        this.numUniqueUsers.SetText(numUsers.ToString());

        if (numLevelsAttemptedAllUsers > 0)
        {
            float avgCorrectAnswers = (float)sumCorrectAnswersAllUsers / numLevelsAttemptedAllUsers;
            float avgFails = (float)totalFailsBeforePassingAllUsers / numLevelsAttemptedAllUsers;

            this.avgFailsBeforeFirstPass.SetText(avgFails.ToString());
            this.avgCorrectAnswersBestAttempt.SetText(avgCorrectAnswers.ToString());
        }
        else
        {
            this.avgFailsBeforeFirstPass.SetText("No data available");
            this.avgCorrectAnswersBestAttempt.SetText("No data available");
        }

    }
}
