using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StudentRowUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI username;

    // display the rank, username and score
    public void displayStudent(string username)
    {
        this.username.SetText(username);
    }
}
