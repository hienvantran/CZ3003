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
    [SerializeField] private TMP_Dropdown zoneDropdown;
    [SerializeField] private TMP_Dropdown levelDropdown;

    [SerializeField] private LeaderboardDatabaseManager leaderboardDatabaseManager;

    // keep tracks of the list of zones each world contains
    private Dictionary<string, List<string>> zonesInWorlds;

    // keep tracks of the list of levels each zone contains
    private Dictionary<ValueTuple<string, string>, List<string>> levelsInZones;

    public delegate void OnSelectionChangedDelegate();
    public event OnSelectionChangedDelegate SelectionChanged;

    void Start()
    {
        GetWorldZoneLevelData();
    }

    // Get the list of worlds, zones and levels
    void GetWorldZoneLevelData()
    {
        leaderboardDatabaseManager.getWorldsZonesLevels(
            res =>
            {
                GetWorldZoneLevelData(res);
                UpdateWorldOptions();
                UpdateZoneOptions();
                UpdateLevelOptions();
                SetAutoUpdateOnValueChanged();
            }
        );
    }
    void GetWorldZoneLevelData(Dictionary<string, Dictionary<string, List<string>>> worldsZonesLevels)
    {
        zonesInWorlds = new Dictionary<string, List<string>>();
        levelsInZones = new Dictionary<(string, string), List<string>>();
        foreach (KeyValuePair<string, Dictionary<string, List<string>>> world in worldsZonesLevels)
        {
            Debug.Log("world is: " + world.Key);
            List<string> zones = new List<string>();
            foreach (KeyValuePair<string, List<string>> zone in world.Value)
            {
                zones.Add(zone.Key);
                levelsInZones.Add((world.Key, zone.Key), zone.Value);
            }
            zonesInWorlds.Add(world.Key, zones);
        }
        Debug.Log("Done getting world zone level");
    }
    void UpdateWorldOptions()
    {
        ClearOptionsAndSetDefault(worldDropdown);
        Debug.Log("Updating world dropdown");
        worldDropdown.AddOptions(zonesInWorlds.Keys.ToList());
        // UpdateZoneOptions();
    }

    void UpdateZoneOptions()
    {
        ClearOptionsAndSetDefault(zoneDropdown);
        string worldSelected = GetWorldSelected();
        Debug.Log("Updating zone dropdown");
        if (worldSelected != defaultOptionAll)
        {
            zoneDropdown.AddOptions(zonesInWorlds[worldSelected]);
        }
        // UpdateLevelOptions();
    }

    void UpdateLevelOptions()
    {
        ClearOptionsAndSetDefault(levelDropdown);
        string worldSelected = GetWorldSelected();
        string zoneSelected = GetZoneSelected();
        Debug.Log("Updating level dropdown");
        if (worldSelected != defaultOptionAll && zoneSelected != defaultOptionAll)
        {
            levelDropdown.AddOptions(levelsInZones[(worldSelected, zoneSelected)]);
        }
    }

    protected virtual void OnSelectionChanged() //protected virtual method
    {
        SelectionChanged?.Invoke();
    }

    void SetAutoUpdateOnValueChanged()
    {
        worldDropdown.onValueChanged.AddListener(delegate
            { UpdateZoneOptions(); OnSelectionChanged(); });
        zoneDropdown.onValueChanged.AddListener(delegate
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

    public string GetZoneSelected()
    {
        return GetDropdownItemSelectedText(zoneDropdown);
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
