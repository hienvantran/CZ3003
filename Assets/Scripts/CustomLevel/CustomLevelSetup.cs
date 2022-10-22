using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomLevelSetup : MonoBehaviour
{
    [SerializeField] private Transform scrollContent;
    [SerializeField] private GameObject questionPrefab, createdPanel, seedBox;
    [SerializeField] private Slider numQnInput;
    [SerializeField] private TextMeshProUGUI numQnLabel, alertLabel;

    private int questionCount = 5;
    private List<GameObject> qnList;

    //Start
    void Start()
    {
        qnList = new List<GameObject>();
        
        //setup the default 5 first
        for (int i = 0; i < questionCount; i++)
        {
            scrollContent.GetChild(i).GetComponent<CreateQnItem>().setValues(i + 1);
            qnList.Add(scrollContent.GetChild(i).gameObject);
        }

        seedBox.GetComponent<TMP_InputField>().onFocusSelectAll = true;
    }

    //Create Scroll Content
    private void CreateScrollContent()
    {
        //clear out qn list first
        for (int i = qnList.Count - 1; i >= 0; i--)
        {
            GameObject tempG = qnList[i];
            qnList.RemoveAt(i);
            Destroy(tempG);
        }

        //create, create qn items
        for (int i = 0; i < questionCount; i++)
        {
            GameObject question = Instantiate(questionPrefab, scrollContent);
            question.GetComponent<CreateQnItem>().setValues(i + 1);
            qnList.Add(question);
        }

        //move content to show top
        scrollContent.GetComponent<RectTransform>().localPosition = new Vector3(0, -1 * (scrollContent.GetComponent<RectTransform>().rect.height / 2), 0);
    }

    //On Number of Qn input change
    public void NumQnChange()
    {
        questionCount = (int)numQnInput.value;
        numQnLabel.SetText("Number of Questions: " + questionCount);
        CreateScrollContent();
    }

    //Create Button Click
    public void CreateClick()
    {
        bool allValid = true;

        alertLabel.SetText("");

        //check if all questions valid
        foreach (GameObject g in qnList)
        {
            //if any of the questions not valid
            if (!g.GetComponent<CreateQnItem>().isValidQn())
            {
                //set bool false
                allValid = false;
                //break
                break;
            }
        }

        //not valid
        if (!allValid)
        {
            //set alert text
            alertLabel.SetText("Please check your inputs on the right.\n" +
                "Ensure that only whole numbers are entered into num1 and num2\n" +
                "and there are no empty fields.");
        }
        //valid
        else
        {
            //get question List as string
            List<string> qList = new List<string>();
            foreach (GameObject g in qnList)
            {
                qList.Add(g.GetComponent<CreateQnItem>().getQnString());
            }

            //create level seed
            string levelSeed = SeedEncoder.CreateSeed(questionCount, qList);
            Debug.Log("seed: " + levelSeed);

            //create level id (assignment key)
            string levelId = SeedEncoder.CreateLevelID();
            Debug.Log("level id: " + levelId);

            //push to database
            FirestoreManager.Instance.AddAssignment(levelId, levelSeed, res =>
            {
                if (res["qnsString"] != null)
                {
                    createdPanel.SetActive(true);
                    seedBox.GetComponent<TMP_InputField>().text = levelId;
                }
                else
                {
                    Debug.LogError(string.Format("Error adding assignment to database with key[{0}] and qString [{1}]", levelId, levelSeed));
                }
            });
        }
    }

    //On close button click
    public void ClosePanel()
    {
        createdPanel.SetActive(false);
    }
}
