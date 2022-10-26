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

    public void AddUser(string uid)
    {
        uniqueUsersAttempted.Add(uid);
    }
}


public class AnalyticsManager : MonoBehaviour
{
    // map a tuple of (world, level) to attempt summary
    private Dictionary<(string, string), UserAttemptSummary> allSummariesWorldLevel;
    [SerializeField] private AnalyticsRowUI analyticsRowUi;
    private List<AnalyticsRowUI> rowUIList;

    // keep tracks of the list of levels each world contains
    private Dictionary<string, List<string>> worldLevelList;

    private string worldAll = "All";
    private string levelAll = "All";
    public enum ContentType
    {
        DefaultContent,
        AssignmentContent,
    }
    private ContentType? currentContentType;
    public const string assignmentWorldKey = "assignments";

    void Awake()
    {
        worldLevelList = new Dictionary<string, List<string>>();
        rowUIList = new List<AnalyticsRowUI>();
        currentContentType = null;
    }

    public void DisplayAnalyticsOnContent(ContentType contentType)
    {
        Debug.Log($"content type analytics = {contentType}");
        if (currentContentType != null && currentContentType == contentType) return;
        currentContentType = contentType;
        Debug.Log($"current content type = {currentContentType}");
        StartCoroutine(GetAnalyticsData());
    }

    public IEnumerator GetAnalyticsData()
    {
        worldLevelList.Clear();
        if (currentContentType == ContentType.DefaultContent)
        {
            yield return StartCoroutine(GetContentStructureDefaultTask());
            yield return StartCoroutine(RetrieveUserAttemptsDefaultContent());
        }
        else
        {
            yield return StartCoroutine(GetContentStructureAssignment());
            yield return StartCoroutine(RetrieveUserAttemptsAssignmentContent());
        }
        ClearRows();
        DisplayRows();
    }

    // Get the list of worlds and levels
    public IEnumerator GetContentStructureDefaultTask()
    {
        var getContentHierarchyTask = FirestoreManager.Instance.GetWorldsLevels(
            res =>
            {
                worldLevelList = res;
            }
        );
        yield return new WaitUntil(predicate: () => getContentHierarchyTask.IsCompleted);
    }

    // Get the list of assignments
    public IEnumerator GetContentStructureAssignment()
    {
        var getAssignmentHierarchyTask = FirestoreManager.Instance.GetAssignments(
            res =>
            {
                worldLevelList.Add(assignmentWorldKey, res);
            }
        );
        yield return new WaitUntil(predicate: () => getAssignmentHierarchyTask.IsCompleted);
    }

    // get user attempts on default content from database
    public IEnumerator RetrieveUserAttemptsDefaultContent()
    {
        allSummariesWorldLevel = new Dictionary<(string, string), UserAttemptSummary>();

        UserAttemptSummary allWorldsAllLevelsAggAttempt = new UserAttemptSummary();
        foreach (KeyValuePair<string, List<string>> levelsInWorld in worldLevelList)
        {
            string currentWorld = levelsInWorld.Key;
            UserAttemptSummary currentWorldAllLevelsAggAttempt = new UserAttemptSummary();

            List<string> levels = levelsInWorld.Value;
            foreach (string currentLevel in levels)
            {
                UserAttemptSummary currentWorldCurrentLevelAttempt = new UserAttemptSummary();
                string levelId = WorldLevelParser.formatIdFromWorldLevel(currentWorld, currentLevel);
                var getScoresForSelectedContent = FirestoreManager.Instance.GetLevelAttemptsbyID(levelId,
                    attempts =>
                    {
                        AddAttemptToSummary(attempts, currentWorldCurrentLevelAttempt);
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

    // get user attempts on assignment content from database
    public IEnumerator RetrieveUserAttemptsAssignmentContent()
    {
        allSummariesWorldLevel = new Dictionary<(string, string), UserAttemptSummary>();

        string currentWorld = assignmentWorldKey;
        List<string> levels = worldLevelList[currentWorld];
        UserAttemptSummary currentWorldAllLevelsAggAttempt = new UserAttemptSummary();
        foreach (string currentLevel in levels)
        {
            UserAttemptSummary currentWorldCurrentLevelAttempt = new UserAttemptSummary();
            string assignId = currentLevel;
            var getScoresForSelectedContent = FirestoreManager.Instance.GetAssignmentAttemptsbyID(assignId,
                attempts =>
                {
                    AddAttemptToSummary(attempts, currentWorldCurrentLevelAttempt);
                });
            yield return new WaitUntil(predicate: () => getScoresForSelectedContent.IsCompleted);
            allSummariesWorldLevel.Add((currentWorld, currentLevel), currentWorldCurrentLevelAttempt);
            currentWorldAllLevelsAggAttempt.AggregateAttempt(currentWorldCurrentLevelAttempt);
        }
        allSummariesWorldLevel.Add((currentWorld, levelAll), currentWorldAllLevelsAggAttempt);
    }

    private void AddAttemptToSummary(List<Dictionary<string, object>> attempts, UserAttemptSummary summary)
    {
        foreach (Dictionary<string, object> userAttempt in attempts)
        {
            string uid = userAttempt["uid"].ToString();
            summary.AddUser(uid);
            summary.numLevelsAttempted++;
            summary.sumCorrectAnswers += int.Parse(userAttempt["correct"].ToString());
            summary.totalFailsBeforePassing += int.Parse(userAttempt["fail"].ToString());
        }
    }

    private void ClearRows()
    {
        foreach (AnalyticsRowUI row in rowUIList)
        {
            Destroy(row.gameObject);
        }
        rowUIList.Clear();
    }

    private void DisplayRows()
    {
        if (currentContentType == ContentType.DefaultContent) AddRow(worldAll, levelAll);
        foreach (KeyValuePair<string, List<string>> levelsInWorld in worldLevelList)
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
        AnalyticsRowUI row = Instantiate(analyticsRowUi, transform).GetComponent<AnalyticsRowUI>();
        rowUIList.Add(row);
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
