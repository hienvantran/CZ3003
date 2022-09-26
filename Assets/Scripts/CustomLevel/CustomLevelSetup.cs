using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomLevelSetup : MonoBehaviour
{
    [SerializeField] private Transform scrollContent;
    [SerializeField] private GameObject questionPrefab;
    [SerializeField] private Slider numQnInput, gameSpeedInput;
    [SerializeField] private TextMeshProUGUI numQnLabel, gameSpeedLabel, alertLabel;

    private int questionCount = 5;
    private float gameSpeed = 1f;
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

    //On Game Speed input change
    public void GameSpeedChange()
    {
        gameSpeed = gameSpeedInput.value;
        gameSpeedLabel.SetText("Game Speed: " + gameSpeed);
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
            //for now, print out the questions
            foreach (GameObject g in qnList)
            {
                Debug.Log(g.GetComponent<CreateQnItem>().getQnString());
            }
        }
    }
}
