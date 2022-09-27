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
    public List<string> customQuestions;

    private void Awake()
    {
        if (!LevelManager.instance)
            LevelManager.instance = this;

        if (LevelManager.instance != this)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetParams(int numQns, float difficulty, QuestionManager.OpMode opMode, List<string> cusQuestions = null)
    {
        this.numQns = numQns;
        runnerDifficulty = difficulty;
        runnerOpMode = opMode;
        customQuestions = cusQuestions;
    }


}
