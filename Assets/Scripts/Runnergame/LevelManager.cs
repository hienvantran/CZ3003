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

    //public FirestoreManager firestoreManager;

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

        //update the level progress (for non custom)
        if (!isCus && unlockNext)
        {
            switch (currentLevel[0])
            {
                case 'A':
                    addProgress = (int)char.GetNumericValue(currentLevel[1]);
                    //push firestore user AddProgress field
                    FirestoreManager.instance.updateUserWorldProgress(FirebaseManager.instance.User, "AddProgress", addProgress);
                    break;
                case 'S':
                    subProgress = (int)char.GetNumericValue(currentLevel[1]);
                    //push firestore user SubProgress field
                    FirestoreManager.instance.updateUserWorldProgress(FirebaseManager.instance.User, "SubProgress", subProgress);
                    break;
                case 'M':
                    mulProgress = (int)char.GetNumericValue(currentLevel[1]);
                    //push firestore user MulProgress field
                    FirestoreManager.instance.updateUserWorldProgress(FirebaseManager.instance.User, "MulProgress", mulProgress);
                    break;
                case 'D':
                    divProgress = (int)char.GetNumericValue(currentLevel[1]);
                    //push firestore user DivProgress field
                    FirestoreManager.instance.updateUserWorldProgress(FirebaseManager.instance.User, "DivProgress", divProgress);
                    break;
            }
        }

        //to edit: then also push to firestore the completion records to Score collection
        //push score, user id, etc..

    }
}