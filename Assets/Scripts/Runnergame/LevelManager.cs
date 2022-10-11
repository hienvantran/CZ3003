using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public int numQns = 10;
    public float runnerDifficulty = 1;
    public QuestionManager.OpMode runnerOpMode = QuestionManager.OpMode.ADD;
    public string customQuestions;

    public FirestoreManager firestoreManager;

    public int addProgress = 0, subProgress = 0, mulProgress = 0, divProgress = 0;
    public string currentLevel = string.Empty;
    public string previousScene = "";

    private void Awake()
    {
        if (!LevelManager.instance)
            LevelManager.instance = this;

        if (LevelManager.instance != this)
            Destroy(this.gameObject);

        GetUserProgress();

        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetParams(int numQns, float difficulty, QuestionManager.OpMode opMode, string cusQuestions = "")
    {
        this.numQns = numQns;
        runnerDifficulty = difficulty;
        runnerOpMode = opMode;
        customQuestions = cusQuestions;
    }

    //set current level
    public void SetLevel(string level)
    {
        currentLevel = level;
    }

    //Gets the user's progress of the normal levels
    public void GetUserProgress()
    {
        //to edit: maybe check firebase auth confirm user login

        //to edit: pull firestore user progress fields
        addProgress = 0;
        subProgress = 0;
        mulProgress = 2;
        divProgress = 1;
    }

    //Updates the user's progress of the normal levels
    public void UpdateUserProgress(int score, bool unlockNext, bool isCus)
    {
        //to edit: maybe check firebase auth confirm user login

        //update the level progress (for non custom)
        if (!isCus && unlockNext)
        {
            switch (currentLevel[0])
            {
                case 'A':
                    addProgress = (int)char.GetNumericValue(currentLevel[1]);
                    //to edit: push firestore user progress fields
                    //push
                    break;
                case 'S':
                    subProgress = (int)char.GetNumericValue(currentLevel[1]);
                    //to edit: push firestore user progress fields
                    //push
                    break;
                case 'M':
                    mulProgress = (int)char.GetNumericValue(currentLevel[1]);
                    //to edit: push firestore user progress fields
                    //push
                    break;
                case 'D':
                    divProgress = (int)char.GetNumericValue(currentLevel[1]);
                    //to edit: push firestore user progress fields
                    //push
                    break;
            }
        }

        //to edit: then also push to firestore the completion records to Score collection
        //push score, user id, etc..

    }
}