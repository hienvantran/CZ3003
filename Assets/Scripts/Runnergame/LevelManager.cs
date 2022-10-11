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
    public string currentSeed;

    public int addProgress = 0, subProgress = 0, mulProgress = 0, divProgress = 0;
    public string currentLevel = string.Empty;
    public string previousScene = "";

    private void Awake()
    {
        if (!LevelManager.instance)
            LevelManager.instance = this;

        if (LevelManager.instance != this)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);

        StartCoroutine(GetUserProgress());
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetParams(int numQns, float difficulty, QuestionManager.OpMode opMode, string cusQuestions = "", string seed = "")
    {
        this.numQns = numQns;
        runnerDifficulty = difficulty;
        runnerOpMode = opMode;
        customQuestions = cusQuestions;
        currentSeed = seed;
    }

    //set current level
    public void SetLevel(string level)
    {
        currentLevel = level;
    }

    //Gets the user's progress of the normal levels
    public IEnumerator GetUserProgress()
    {
        //check firebase auth confirm user login
        if (FirebaseManager.instance.User == null)
        {
            Debug.LogWarning("No user account detected, world progress fields will be set to zero");
            yield break;
        }

        //pull firestore user progress fields
        var getProgressTask = FirestoreManager.instance.getUserWorldProgress(FirebaseManager.instance.User, res =>
        {
            addProgress = res["Add"];
            subProgress = res["Sub"];
            mulProgress = res["Mul"];
            divProgress = res["Div"];
        });

        yield return new WaitUntil(predicate: () => getProgressTask.IsCompleted);
    }

    //Updates the user's progress of the normal levels
    public void UpdateUserProgress(int score, bool unlockNext, bool isCus)
    {
        //check firebase auth confirm user login
        if (FirebaseManager.instance.User == null)
        {
            Debug.LogWarning("No user account detected, skipping firestore update");
            return;
        }

        //then also push to firestore the completion records to LevelScore / Assignment collection
        if (isCus)
        {
            //add user attempt for assignment
            FirestoreManager.instance.addUserAssignmentAttempts(currentSeed, FirebaseManager.instance.User.UserId, score.ToString(), res =>
            {
                Debug.Log("Pushed user(" + FirebaseManager.instance.User.UserId + ") attempt for " + currentSeed + ", score: " + score);
            });
        }
        else
        {
            string lvlID = "";
            //add user attempt for level and also update progression field if needed
            switch (currentLevel[0])
            {
                case 'A':
                    addProgress = (int)char.GetNumericValue(currentLevel[1]);
                    lvlID = "add-" + addProgress.ToString();
                    //push firestore user AddProgress field
                    if (unlockNext)
                    {
                        FirestoreManager.instance.updateUserWorldProgress(FirebaseManager.instance.User, "AddProgress", addProgress);
                    }
                    break;
                case 'S':
                    subProgress = (int)char.GetNumericValue(currentLevel[1]);
                    //push firestore user level attempt
                    lvlID = "sub-" + subProgress.ToString();
                    //push firestore user SubProgress field
                    if (unlockNext)
                    {
                        FirestoreManager.instance.updateUserWorldProgress(FirebaseManager.instance.User, "SubProgress", subProgress);
                    }
                    break;
                case 'M':
                    mulProgress = (int)char.GetNumericValue(currentLevel[1]);
                    lvlID = "mul-" + mulProgress.ToString();
                    //push firestore user MulProgress field
                    if (unlockNext)
                    {
                        FirestoreManager.instance.updateUserWorldProgress(FirebaseManager.instance.User, "MulProgress", mulProgress);
                    }
                    break;
                case 'D':
                    divProgress = (int)char.GetNumericValue(currentLevel[1]);
                    lvlID = "div-" + divProgress.ToString();
                    //push firestore user DivProgress field
                    if (unlockNext)
                    {
                        FirestoreManager.instance.updateUserWorldProgress(FirebaseManager.instance.User, "DivProgress", divProgress);
                    }
                    break;
            }
            //push firestore user level attempt
            FirestoreManager.instance.addUserLevelAttempts(lvlID, FirebaseManager.instance.User.UserId, score.ToString(), res =>
            {
                Debug.Log("Pushed user(" + FirebaseManager.instance.User.UserId + ") attempt for " + lvlID + ", score: " + score);
            });
        }
    }
}