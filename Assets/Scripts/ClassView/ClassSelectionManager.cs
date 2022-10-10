using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using TMPro;

public class ClassSelectionManager : MonoBehaviour
{
    private string defaultOptionAll = "All";
    [SerializeField] private TMP_Dropdown classDropdown;

    // keep tracks of the list of all students in each class
    private Dictionary<string, List<string>> studentsInAllClasses;

    // list of students in the selected class
    private IEnumerable<string> studentsInSelectedClass;

    // display one student info on each row
    [SerializeField] private StudentRowUi studentRowUi;

    private List<StudentRowUi> studentRowUiList;

    void Awake()
    {
        studentRowUiList = new List<StudentRowUi>();
        GetClassData();
        UpdateClassOptions();
        UpdateStudentDisplay();
        SetAutoUpdateOnValueChanged();
    }

    // Get the list of classes
    void GetClassData()
    {
        studentsInAllClasses = new Dictionary<string, List<string>>();
        // some dummy data for now
        studentsInAllClasses.Add("1A", new List<string>() {
            "Alice1A", "Bob1A", "Charlie1A", "David1A", "Elene1A"
            });
        studentsInAllClasses.Add("1B", new List<string>() {
            "Alice1B", "Bob1B", "Charlie1B", "David1B", "Elene1B"
            });
        studentsInAllClasses.Add("1C", new List<string>() {
            "Alice1C", "Bob1C", "Charlie1C", "David1C", "Elene1C"
            });
    }


    void UpdateClassOptions()
    {
        ClearOptionsAndSetDefault(classDropdown);
        classDropdown.AddOptions(studentsInAllClasses.Keys.ToList());
    }

    void UpdateStudentDisplay()
    {
        Debug.Log("Selection changed. Update leaderboard table...");
        GetStudentsInSelectedClass();
        ClearRows();
        DisplayRows();
    }

    void SetAutoUpdateOnValueChanged()
    {
        classDropdown.onValueChanged.AddListener(delegate
            { UpdateStudentDisplay(); });
    }

    void GetStudentsInSelectedClass()
    {
        string selectedClass = GetClassSelected();
        studentsInSelectedClass = Enumerable.Empty<String>();
        if (selectedClass != defaultOptionAll)
        {
            studentsInSelectedClass = studentsInAllClasses[selectedClass];
            return;
        }

        // If all classes are selected
        foreach (KeyValuePair<string, List<string>> classToStudents in studentsInAllClasses)
        {
            studentsInSelectedClass = studentsInSelectedClass.Concat(classToStudents.Value);
        }
    }

    private void ClearRows()
    {
        foreach (StudentRowUi row in studentRowUiList)
        {
            Destroy(row.gameObject);
        }
        studentRowUiList.Clear();
    }

    private void DisplayRows()
    {
        foreach (string studentUsername in studentsInSelectedClass)
        {
            StudentRowUi row = Instantiate(studentRowUi, transform).GetComponent<StudentRowUi>();
            row.displayStudent(studentUsername);
            studentRowUiList.Add(row);
        }
    }

    private void ClearOptionsAndSetDefault(TMP_Dropdown dropdown)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string> { defaultOptionAll });
    }

    public string GetClassSelected()
    {
        return GetDropdownItemSelectedText(classDropdown);
    }

    private string GetDropdownItemSelectedText(TMP_Dropdown dropdown)
    {
        int index = dropdown.value;
        return dropdown.options[index].text;
    }
}
