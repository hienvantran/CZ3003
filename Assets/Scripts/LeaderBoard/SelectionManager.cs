using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using TMPro;

public class SelectionManager : MonoBehaviour
{
    private string defaultOptionAll = "All";
    public const string assignmentWorldKey = "assignments";
    [SerializeField] private TMP_Dropdown worldDropdown;
    [SerializeField] private TMP_Dropdown levelDropdown;

    // keep tracks of the list of levels each world or assignment contains
    private Dictionary<string, List<string>> worldLevelList;

    public delegate void OnSelectionChangedDelegate();
    public event OnSelectionChangedDelegate SelectionChanged;

    // map world key to a UNIQUE value to display on the UI
    private static Dictionary<string, string> keyToWorld = new Dictionary<string, string>()
    {
        {"add", "Addition"},
        {"sub", "Subtraction"},
        {"mul", "Multiplication"},
        {"div", "Division"},
        {"All", "All" },
        {assignmentWorldKey, "Assignments"},
    };

    private static Dictionary<string, string> worldToKey = keyToWorld.ToDictionary(x => x.Value, x => x.Key);

    void Start()
    {
        worldLevelList = new Dictionary<string, List<string>>();
        Debug.Log("Start trying to retrieve content hierarchy");
        StartCoroutine(GetContentStructure());
    }

    // Get the list of worlds and levels
    public IEnumerator GetContentStructure()
    {
        var getDefaultContentHierarchyTask = FirestoreManager.Instance.GetWorldsLevels(
            res =>
            {
                worldLevelList = res;
            }
        );
        yield return new WaitUntil(predicate: () => getDefaultContentHierarchyTask.IsCompleted);
        var getAssignmentHierarchyTask = FirestoreManager.Instance.GetAssignments(
            res =>
            {
                worldLevelList[assignmentWorldKey] = res;
            }
        );
        yield return new WaitUntil(predicate: () => getAssignmentHierarchyTask.IsCompleted);
        Debug.Log("Update world and level dropdowns");
        UpdateWorldOptions();
        UpdateLevelOptions();
        SetAutoUpdateOnValueChanged();
        OnSelectionChanged();
    }
    void UpdateWorldOptions()
    {
        ClearOptionsAndSetDefault(worldDropdown);
        Debug.Log("Updating world dropdown");
        worldDropdown.AddOptions(WorldKeyNaming(worldLevelList.Keys.ToList()));
    }

    void UpdateLevelOptions()
    {
        ClearOptionsAndSetDefault(levelDropdown);
        string worldSelected = GetWorldSelected();
        Debug.Log("Updating level dropdown");
        if (worldSelected != defaultOptionAll)
        {
            levelDropdown.AddOptions(worldLevelList[worldSelected]);
        }
    }

    private List<string> WorldKeyNaming(List<string> keys)
    {
        List<string> worlds = new List<string>();
        foreach (string key in keys)
            worlds.Add(keyToWorld[key]);
        return worlds;
    }

    protected virtual void OnSelectionChanged() //protected virtual method
    {
        SelectionChanged?.Invoke();
    }

    void SetAutoUpdateOnValueChanged()
    {
        worldDropdown.onValueChanged.AddListener(delegate
            { UpdateLevelOptions(); OnSelectionChanged(); });
        levelDropdown.onValueChanged.AddListener(delegate
            { OnSelectionChanged(); });
    }

    private void ClearOptionsAndSetDefault(TMP_Dropdown dropdown)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string> { defaultOptionAll });
    }

    public string GetWorldSelected()
    {
        return worldToKey[GetDropdownItemSelectedText(worldDropdown)];
    }

    public string GetLevelSelected()
    {
        return GetDropdownItemSelectedText(levelDropdown);
    }

    private string GetDropdownItemSelectedText(TMP_Dropdown dropdown)
    {
        int index = dropdown.value;
        return dropdown.options[index].text;
    }

    public List<(string, string)> GetSelectedWorldsLevels()
    {
        string worldSelected = GetWorldSelected();
        string levelSelected = GetLevelSelected();

        List<(string, string)> selectedWorldsLevels = new List<(string, string)>();

        foreach (KeyValuePair<string, List<string>> worldLevelsPair in worldLevelList)
        {
            if (worldSelected != defaultOptionAll && worldLevelsPair.Key != worldSelected) continue;
            foreach (string level in worldLevelsPair.Value)
            {
                if (levelSelected != defaultOptionAll && level != levelSelected) continue;
                selectedWorldsLevels.Add((worldLevelsPair.Key, level));
            }
        }
        return selectedWorldsLevels;
    }
}
