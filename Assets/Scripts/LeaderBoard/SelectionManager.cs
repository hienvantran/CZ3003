using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using TMPro;

public class SelectionManager : MonoBehaviour
{
    private string defaultOptionAll = "All";
    [SerializeField] private TMP_Dropdown worldDropdown;
    [SerializeField] private TMP_Dropdown levelDropdown;

    [SerializeField] private LeaderboardDatabaseManager leaderboardDatabaseManager;

    // keep tracks of the list of levels each world contains
    private Dictionary<string, List<string>> worldsLevels;

    public delegate void OnSelectionChangedDelegate();
    public event OnSelectionChangedDelegate SelectionChanged;

    void Start()
    {
        GetWorldLevelData();
    }

    // Get the list of worlds and levels
    void GetWorldLevelData()
    {
        leaderboardDatabaseManager.getWorldsLevels(
            res =>
            {
                worldsLevels = res;
                UpdateWorldOptions();
                UpdateLevelOptions();
                SetAutoUpdateOnValueChanged();
            }
        );
    }
    void UpdateWorldOptions()
    {
        ClearOptionsAndSetDefault(worldDropdown);
        Debug.Log("Updating world dropdown");
        worldDropdown.AddOptions(worldsLevels.Keys.ToList());
    }

    void UpdateLevelOptions()
    {
        ClearOptionsAndSetDefault(levelDropdown);
        string worldSelected = GetWorldSelected();
        Debug.Log("Updating level dropdown");
        if (worldSelected != defaultOptionAll)
        {
            levelDropdown.AddOptions(worldsLevels[worldSelected]);
        }
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
        return GetDropdownItemSelectedText(worldDropdown);
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
}
