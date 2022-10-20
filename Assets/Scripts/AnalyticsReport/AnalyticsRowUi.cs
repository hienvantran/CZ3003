using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnalyticsRowUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI world;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI numStudents;
    [SerializeField] private TextMeshProUGUI avgFailsPerLevel;
    [SerializeField] private TextMeshProUGUI avgCorrectPerLevel;

    Dictionary<string, string> keyToWorld = new Dictionary<string, string>()
    {
        {"add", "Addition"},
        {"sub", "Subtraction"},
        {"mul", "Multiplication"},
        {"div", "Division"},
        {"All", "All" },
    };

    public void Display(string world, string level, string numStudents, string avgFailsPerLevel, string avgCorrectPerLevel)
    {
        this.world.SetText(keyToWorld[world]);
        this.level.SetText(level);
        this.numStudents.SetText(numStudents);
        this.avgFailsPerLevel.SetText(avgFailsPerLevel);
        this.avgCorrectPerLevel.SetText(avgCorrectPerLevel);
    }
}
