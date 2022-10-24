using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropdownSearch : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;

    [SerializeField]
    private TMP_Dropdown dropdown;
    private List<TMP_Dropdown.OptionData> dropdownOptions;

    private float debounceTimer = 0f;
    private bool filter = false;

    //Start
    private void Start()
    {
        StartCoroutine(GetAssignmentsAsDropdown());
    }

    //Update
    private void Update()
    {
        if (filter)
        {
            if (debounceTimer <= 0f)
            {
                DropdownFilter(inputField.text);
                filter = false;
            }
            else
            {
                debounceTimer -= Time.deltaTime;
            }
        }
    }

    //get assignments
    private IEnumerator GetAssignmentsAsDropdown()
    {
        List<string> assKeys = new List<string>();

        //get lsit of assignment ids
        var retrieveAssignments = FirestoreManager.Instance.GetAssignments(res =>
        {
            assKeys = res;
        });

        yield return new WaitUntil(predicate: () => retrieveAssignments.IsCompleted);

        dropdown.ClearOptions();
        dropdown.AddOptions(assKeys);
        dropdown.RefreshShownValue();

        //store list of options so we don't lose them while filtering
        dropdownOptions = dropdown.options;
    }


    // For each key press with .5 second debouncing
    public void EditInput()
    {
        debounceTimer = 0.5f;
        filter = true;
    }

    //Filter dropdown values based on input
    private void DropdownFilter(string input)
    {
        debounceTimer = 0.5f;

        dropdown.options = dropdownOptions.FindAll(option => option.text.IndexOf(input) >= 0);
        dropdown.RefreshShownValue();
        dropdown.Hide();
        dropdown.Show();
        if (!inputField.isFocused)
        {
            inputField.Select();
            inputField.caretPosition = inputField.text.Length;
        }
    }
}
