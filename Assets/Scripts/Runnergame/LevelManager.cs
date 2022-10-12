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
        StartCoroutine(doUpdateUserProgress(score, unlockNext, isCus));
    }
    public IEnumerator doUpdateUserProgress(int score, bool unlockNext, bool isCus)
    {
        //check firebase auth confirm user login
        if (FirebaseManager.instance.User == null)
        {
            Debug.LogWarning("No user account detected, skipping firestore update");
            yield break;
        }
        
        int prevScore = 0;

        //then also push to firestore the completion records to LevelScore / Assignment collection
        if (isCus)
        {
            //check for existing attempt and update higher score
            var checkExistTask = FirestoreManager.instance.getSpecificUserAssignmentAttempt(currentSeed, FirebaseManager.instance.User.UserId, res =>
            {
                prevScore = int.Parse(res.score);
            });

            yield return new WaitUntil(predicate: () => checkExistTask.IsCompleted);

            //add user attempt for assignment
            FirestoreManager.instance.addUserAssignmentAttempts(currentSeed, FirebaseManager.instance.User.UserId, Mathf.Max(prevScore, score).ToString(), res =>
            {
                Debug.Log("Pushed user(" + FirebaseManager.instance.User.UserId + ") attempt for " + currentSeed + ", score: " + Mathf.Max(prevScore, score));
            });
        }
        else
        {
            //add user attempt for level and also update progression field if needed
            string lvlID = "";
            switch (currentLevel[0])
            {
                case 'A':
                    lvlID = "add-" + char.GetNumericValue(currentLevel[1]);
                    //push firestore user AddProgress field
                    if (unlockNext)
                    {
                        addProgress++;
                        FirestoreManager.instance.updateUserWorldProgress(FirebaseManager.instance.User, "AddProgress", addProgress);
                    }
                    break;
                case 'S':
                    lvlID = "sub-" + char.GetNumericValue(currentLevel[1]);
                    //push firestore user SubProgress field
                    if (unlockNext)
                    {
                        subProgress++;
                        FirestoreManager.instance.updateUserWorldProgress(FirebaseManager.instance.User, "SubProgress", subProgress);
                    }
                    break;
                case 'M':
                    lvlID = "mul-" + char.GetNumericValue(currentLevel[1]);
                    //push firestore user MulProgress field
                    if (unlockNext)
                    {
                        mulProgress++;
                        FirestoreManager.instance.updateUserWorldProgress(FirebaseManager.instance.User, "MulProgress", mulProgress);
                    }
                    break;
                case 'D':
                    lvlID = "div-" + char.GetNumericValue(currentLevel[1]);
                    //push firestore user DivProgress field
                    if (unlockNext)
                    {
                        divProgress++;
                        FirestoreManager.instance.updateUserWorldProgress(FirebaseManager.instance.User, "DivProgress", divProgress);
                    }
                    break;
            }

            //check for existing attempt and update higher score
            var checkExistTask = FirestoreManager.instance.getSpecificUserLevelAttempt(lvlID, FirebaseManager.instance.User.UserId, res =>
            {
                prevScore = int.Parse(res.score);
            });

            yield return new WaitUntil(predicate: () => checkExistTask.IsCompleted);

            //push firestore user level attempt
            FirestoreManager.instance.addUserLevelAttempts(lvlID, FirebaseManager.instance.User.UserId, Mathf.Max(prevScore, score).ToString(), res =>
            {
                Debug.Log("Pushed user(" + FirebaseManager.instance.User.UserId + ") attempt for " + lvlID + ", score: " + Mathf.Max(prevScore, score));
            });
        }
    }
}