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

    // keep tracks of the list of zones each world contains
    private Dictionary<string, List<string>> zonesInWorld;

    // keep tracks of the list of levels each zone contains
    private Dictionary<ValueTuple<string, string>, List<string>> levelsInZones;

    public delegate void OnSelectionChangedDelegate();
    public event OnSelectionChangedDelegate OnSelectionChanged;

    void Start()
    {
        GetWorldZoneLevelData();
        UpdateWorldOptions();
        SetAutoUpdateOnValueChanged();
    }

    // Get the list of worlds, zones and levels
    void GetWorldZoneLevelData()
    {
        zonesInWorld = new Dictionary<string, List<string>>();
        // some dummy data for now
        zonesInWorld.Add("Addition", new List<string>() {
            "Beginner", "Intermediate"
            });

        zonesInWorld.Add("Subtraction", new List<string>() {
            "Beginner", "Advanced"
            });

        levelsInZones = new Dictionary<(string, string), List<string>>();
        levelsInZones.Add(("Addition", "Beginner"), new List<string>() {
            "Add Begin Lvl 1", "Add Begin Lvl 2", "Add Begin Lvl 3"
            });

        levelsInZones.Add(("Addition", "Intermediate"), new List<string>() {
            "Add Inter Lvl 1", "Add Inter Lvl 2"
            });

        levelsInZones.Add(("Subtraction", "Beginner"), new List<string>() {
            "Sub Begin Lvl 1", "Sub Begin Lvl 2", "Sub Begin Lvl 3"
            });

        levelsInZones.Add(("Addition", "Advanced"), new List<string>() {
            "Sub Adv Lvl 1",
            });
    }
    void UpdateWorldOptions()
    {
        ClearOptionsAndSetDefault(worldDropdown);
        worldDropdown.AddOptions(zonesInWorld.Keys.ToList());
        UpdateZoneOptions();
    }

    void UpdateZoneOptions()
    {
        ClearOptionsAndSetDefault(zoneDropdown);
        string worldSelected = GetWorldSelected();
        if (worldSelected != defaultOptionAll)
        {
            zoneDropdown.AddOptions(zonesInWorld[worldSelected]);
        }
        UpdateLevelOptions();
    }

    void UpdateLevelOptions()
    {
        ClearOptionsAndSetDefault(levelDropdown);
        string worldSelected = GetWorldSelected();
        string zoneSelected = GetZoneSelected();
        if (worldSelected != defaultOptionAll && zoneSelected != defaultOptionAll)
        {
            levelDropdown.AddOptions(levelsInZones[(worldSelected, zoneSelected)]);
        }
        OnSelectionChanged();

    }

    void SetAutoUpdateOnValueChanged()
    {
        worldDropdown.onValueChanged.AddListener(delegate { UpdateZoneOptions(); });
        zoneDropdown.onValueChanged.AddListener(delegate { UpdateLevelOptions(); });
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
