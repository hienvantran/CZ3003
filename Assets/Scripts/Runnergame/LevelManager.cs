using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager m_Instance;
    public int numQns = 10;
    public float runnerDifficulty = 1;
    public QuestionManager.OpMode runnerOpMode = QuestionManager.OpMode.ADD;
    public string customQuestions;
    public string currentSeed;
    public int charSelected = 0;

    public int addProgress = 0, subProgress = 0, mulProgress = 0, divProgress = 0;
    public string currentLevel = string.Empty;
    public string previousScene = "";

    public List<RuntimeAnimatorController> charUIAnimList;
    public List<RuntimeAnimatorController> charAnimList;

    public static LevelManager Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = new GameObject("LevelManager").AddComponent<LevelManager>();
            return m_Instance;
        }
    }

    private void Awake()
    {
        if (!LevelManager.m_Instance)
            LevelManager.m_Instance = this;

        if (LevelManager.Instance != this)
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
        if (FirebaseManager.Instance.User == null)
        {
            Debug.LogWarning("No user account detected, world progress fields will be set to zero");
            yield break;
        }

        //pull firestore user progress fields
        var getProgressTask = FirestoreManager.Instance.getUserWorldProgress(res =>
        {
            addProgress = res["Add"];
            subProgress = res["Sub"];
            mulProgress = res["Mul"];
            divProgress = res["Div"];
        });

        yield return new WaitUntil(predicate: () => getProgressTask.IsCompleted);
    }

    //Updates the user's progress of the normal levels
    public void UpdateUserProgress(int score, int correctCount, bool unlockNext, bool incFail, bool isCus)
    {
        StartCoroutine(doUpdateUserProgress(score, correctCount, unlockNext, incFail, isCus));
    }
    public IEnumerator doUpdateUserProgress(int score, int correctCount, bool unlockNext, bool incFail, bool isCus)
    {
        //check firebase auth confirm user login
        if (FirebaseManager.Instance.User == null)
        {
            Debug.LogWarning("No user account detected, skipping firestore update");
            yield break;
        }
        
        int prevScore = 0;
        int prevCorrCount = 0;
        int prevFail = 0;

        //then also push to firestore the completion records to LevelScore / Assignment collection
        if (isCus)
        {
            //check for existing attempt and update higher score
            var checkExistTask = FirestoreManager.Instance.getSpecificUserAssignmentAttempt(currentSeed, FirebaseManager.Instance.User.UserId, res =>
            {
                prevScore = int.Parse(res.score);
            });

            yield return new WaitUntil(predicate: () => checkExistTask.IsCompleted);

            //add user attempt for assignment
            FirestoreManager.Instance.addUserAssignmentAttempts(currentSeed, FirebaseManager.Instance.User.UserId, Mathf.Max(prevScore, score).ToString(), res =>
            {
                Debug.Log("Pushed user(" + FirebaseManager.Instance.User.UserId + ") attempt for " + currentSeed + ", score: " + Mathf.Max(prevScore, score));
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
                        FirestoreManager.Instance.updateUserWorldProgress(FirebaseManager.Instance.User, "AddProgress", addProgress);
                    }
                    break;
                case 'S':
                    lvlID = "sub-" + char.GetNumericValue(currentLevel[1]);
                    //push firestore user SubProgress field
                    if (unlockNext)
                    {
                        subProgress++;
                        FirestoreManager.Instance.updateUserWorldProgress(FirebaseManager.Instance.User, "SubProgress", subProgress);
                    }
                    break;
                case 'M':
                    lvlID = "mul-" + char.GetNumericValue(currentLevel[1]);
                    //push firestore user MulProgress field
                    if (unlockNext)
                    {
                        mulProgress++;
                        FirestoreManager.Instance.updateUserWorldProgress(FirebaseManager.Instance.User, "MulProgress", mulProgress);
                    }
                    break;
                case 'D':
                    lvlID = "div-" + char.GetNumericValue(currentLevel[1]);
                    //push firestore user DivProgress field
                    if (unlockNext)
                    {
                        divProgress++;
                        FirestoreManager.Instance.updateUserWorldProgress(FirebaseManager.Instance.User, "DivProgress", divProgress);
                    }
                    break;
            }

            //check for existing attempt and update higher score
            var checkExistTask = FirestoreManager.Instance.getSpecificUserLevelAttempt(lvlID, res =>
            {
                prevScore = int.Parse(res.score);
                prevCorrCount = int.Parse(res.correct);
                prevFail = int.Parse(res.fail);
            });

            yield return new WaitUntil(predicate: () => checkExistTask.IsCompleted);

            //push firestore user level attempt
            string pushScore = Mathf.Max(prevScore, score).ToString();
            string pushCorrect = Mathf.Max(prevCorrCount, correctCount).ToString();
            string pushFail = incFail ? (prevFail += 1).ToString() : prevFail.ToString();

            FirestoreManager.Instance.addUserLevelAttempts(lvlID, FirebaseManager.Instance.User.UserId, pushScore, pushCorrect, pushFail, res =>
            {
                Debug.Log("Pushed user(" + FirebaseManager.Instance.User.UserId + ") attempt for " + lvlID + 
                    "\nscore: " + pushScore +
                    "\ncorrect: " + pushCorrect +
                    "\nfail: " + pushFail
                );
            });
        }
    }
}