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
    public HashSet<string> uniqueUsersAttempted;

    public UserAttemptSummary()
    {
        this.numLevelsAttempted = 0;
        this.sumCorrectAnswers = 0;
        this.totalFailsBeforePassing = 0;
        this.uniqueUsersAttempted = new HashSet<string>();
    }

    public bool IsAttempted()
    {
        return numLevelsAttempted > 0;
    }

    // Get average, note that need to check empty first
    public float GetAverageFails()
    {
        return (float)totalFailsBeforePassing / numLevelsAttempted;
    }

    // Get average, note that need to check empty first
    public float GetAverageCorrect()
    {
        return (float)sumCorrectAnswers / numLevelsAttempted;
    }

    public int GetNumUniqueUsersAttempted()
    {
        return uniqueUsersAttempted.Count;
    }

    public void AggregateAttempt(UserAttemptSummary anotherSummary)
    {
        this.numLevelsAttempted += anotherSummary.numLevelsAttempted;
        this.sumCorrectAnswers += anotherSummary.sumCorrectAnswers;
        this.totalFailsBeforePassing += anotherSummary.totalFailsBeforePassing;
        this.uniqueUsersAttempted.UnionWith(anotherSummary.uniqueUsersAttempted);
    }
}


public class AnalyticsManager : MonoBehaviour
{
    // map a tuple of (world, level) to attempt summary
    private Dictionary<(string, string), UserAttemptSummary> allSummariesWorldLevel;
    [SerializeField] private AnalyticsRowUi analyticsRowUi;

    // keep tracks of the list of levels each world contains
    private Dictionary<string, List<string>> worldsLevels;

    private string worldAll = "All";
    private string levelAll = "All";

    void Start()
    {
        StartCoroutine(GetAnalyticsData());
    }

    public IEnumerator GetAnalyticsData()
    {
        yield return StartCoroutine(GetWorldLevelData());
        yield return StartCoroutine(RetrieveUserAttempts());
        DisplayRows();
    }

    // Get the list of worlds and levels
    public IEnumerator GetWorldLevelData()
    {
        var getContentHierarchyTask = FirestoreManager.Instance.getWorldsLevels(
            res =>
            {
                worldsLevels = res;
            }
        );
        yield return new WaitUntil(predicate: () => getContentHierarchyTask.IsCompleted);
    }

    // get user attempts data from database
    public IEnumerator RetrieveUserAttempts()
    {
        allSummariesWorldLevel = new Dictionary<(string, string), UserAttemptSummary>();

        UserAttemptSummary allWorldsAllLevelsAggAttempt = new UserAttemptSummary();
        foreach (KeyValuePair<string, List<string>> levelsInWorld in worldsLevels)
        {
            string currentWorld = levelsInWorld.Key;
            UserAttemptSummary currentWorldAllLevelsAggAttempt = new UserAttemptSummary();

            List<string> levels = levelsInWorld.Value;
            foreach (string currentLevel in levels)
            {
                UserAttemptSummary currentWorldCurrentLevelAttempt = new UserAttemptSummary();
                string levelId = WorldLevelParser.formatIdFromWorldLevel(currentWorld, currentLevel);
                var getScoresForSelectedContent = FirestoreManager.Instance.getLevelAttemptsbyID(levelId,
                    attempts =>
                    {
                        foreach (Dictionary<string, object> userAttempt in attempts)
                        {
                            string uid = userAttempt["uid"].ToString();
                            currentWorldCurrentLevelAttempt.uniqueUsersAttempted.Add(uid);
                            currentWorldCurrentLevelAttempt.numLevelsAttempted++;
                            currentWorldCurrentLevelAttempt.sumCorrectAnswers += int.Parse(userAttempt["correct"].ToString());
                            currentWorldCurrentLevelAttempt.totalFailsBeforePassing += int.Parse(userAttempt["fail"].ToString());
                        }
                    });
                yield return new WaitUntil(predicate: () => getScoresForSelectedContent.IsCompleted);
                allSummariesWorldLevel.Add((currentWorld, currentLevel), currentWorldCurrentLevelAttempt);
                currentWorldAllLevelsAggAttempt.AggregateAttempt(currentWorldCurrentLevelAttempt);
            }
            allWorldsAllLevelsAggAttempt.AggregateAttempt(currentWorldAllLevelsAggAttempt);
            allSummariesWorldLevel.Add((currentWorld, levelAll), currentWorldAllLevelsAggAttempt);
        }
        allSummariesWorldLevel.Add((worldAll, levelAll), allWorldsAllLevelsAggAttempt);
    }

    private void DisplayRows()
    {
        AddRow(worldAll, levelAll);
        foreach (KeyValuePair<string, List<string>> levelsInWorld in worldsLevels)
        {
            string currentWorld = levelsInWorld.Key;
            List<string> levels = levelsInWorld.Value;
            AddRow(currentWorld, levelAll);
            foreach (string currentLevel in levels)
            {
                AddRow(currentWorld, currentLevel);
            }
        }
    }

    private void AddRow(string world, string level)
    {
        UserAttemptSummary summary = allSummariesWorldLevel[(world, level)];
        AnalyticsRowUi row = Instantiate(analyticsRowUi, transform).GetComponent<AnalyticsRowUi>();
        if (!summary.IsAttempted())
        {
            row.Display(world, level, "0", "NA", "NA");
        }
        else
        {
            row.Display(
                world, level,
                summary.GetNumUniqueUsersAttempted().ToString(),
                summary.GetAverageFails().ToString("F2"),
                summary.GetAverageCorrect().ToString("F2")
            );
        }

    }
}
