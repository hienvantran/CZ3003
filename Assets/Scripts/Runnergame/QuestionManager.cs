using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;

public class QuestionManager : MonoBehaviour
{
    public enum OpMode { ADD, SUB, MUL, DIV, CUS }

    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private List<GameObject> optionSpawnList;
    [SerializeField] private GameObject optionPrefab;

    [Header("Game Settings")]
    [SerializeField] private OpMode opMode;
    [SerializeField] private float baseSpeed = 1f;
    private float curSpeed;

    private AudioSource audioSauce;
    [SerializeField] private AudioClip correctSfx;
    [SerializeField] private AudioClip wrongSfx;
    [SerializeField] private AudioMixer mixer;

    public static QuestionManager instance;
    private BGScroller[] BGs;
    private int num1, num2, answer;
    private int score = 0;
    private int combo = 0;
    private List<GameObject> activeOptions;
    private float intervalTime;
    private bool isInterval = false;
    private List<string> customQs;
    
    private bool isPlaying = false;
    private bool isEndless = false;
    private int numQns = 10;
    private float difficulty = 1;
    private int qnIdx = 1;
    public bool isDebug = true;

    LevelManager lm;

    //Awake
    void Awake()
    {
        if (!QuestionManager.instance)
            instance = this;
    }

    //Start
    void Start()
    {
        activeOptions = new List<GameObject>();
        audioSauce = GetComponent<AudioSource>();
        lm = LevelManager.instance;
        
        StartLevel();
    }

    //Update
    void Update()
    {
        //Interval();
    }

    public void StartLevel()
    {
        if (isPlaying)
            return;
        isPlaying = true;
        difficulty = lm.runnerDifficulty;
        opMode = lm.runnerOpMode;
        numQns = lm.numQns;
        customQs = lm.customQuestions;
        qnIdx = 0;
        intervalTime = 3f;
        curSpeed = baseSpeed;
        BGs = FindObjectsOfType<BGScroller>();
        foreach (BGScroller bg in BGs)
            bg.UpdateSpeed();

        StartCoroutine(NextQuestion());
    }

    //Create a new question
    private void NewQuestion()
    {
        if (opMode != OpMode.CUS)
        {
            answer = GenerateOptions(GenerateQuestion());
        }
        else
        {
            answer = GenerateOptions(GenerateQuestionCustomPool());
        }
    }

    //Generate Question
    private int GenerateQuestion()
    {
        
        int ans = 0;

        //Addition
        if (opMode == OpMode.ADD)
        {
            Debug.Log("Difficulty:" + difficulty);
            num1 = Random.Range(1, Mathf.RoundToInt(6 * difficulty));
            num2 = Random.Range(1, Mathf.RoundToInt(6 * difficulty));
            //set qn and ans
            questionText.SetText(string.Format("Qn {0}: {1} + {2} = ?", qnIdx, num1, num2));
            ans = num1 + num2;
        }
        //Subtraction
        else if (opMode == OpMode.SUB)
        {
            num1 = Random.Range(2, Mathf.RoundToInt(6 * difficulty));
            num2 = Random.Range(1, num1);
            //set qn and ans
            questionText.SetText(string.Format("Qn {0}: {1} - {2} = ?", qnIdx, num1, num2));
            ans = num1 - num2;
        }
        //Multiplication
        if (opMode == OpMode.MUL)
        {
            num1 = Random.Range(2, Mathf.RoundToInt(6 * difficulty));
            num2 = Random.Range(2, Mathf.RoundToInt(7 * difficulty) - num1);
            //set qn and ans
            questionText.SetText(string.Format("Qn {0}: {1} X {2} = ?", qnIdx, num1, num2));
            ans = num1 * num2;
        }
        //Division
        if (opMode == OpMode.DIV)
        {
            num1 = Random.Range(2, Mathf.RoundToInt(6 * difficulty));
            num2 = Random.Range(1, num1);

            int remainder = num1 % num2;
            num1 += (num2 - remainder);

            //set qn and ans
            questionText.SetText(string.Format("Qn {0}: {1} / {2} = ?", qnIdx, num1, num2));
            ans = num1 / num2;
        }
        return ans;
    }

    //Generate Options
    private int GenerateOptions(int ans)
    {
        Shuffle(optionSpawnList);
        List<int> answers = new List<int>();
        for (int i = 0; i < optionSpawnList.Count; i++)
        {
            GameObject op = (GameObject)Instantiate(optionPrefab, optionSpawnList[i].transform.position, Quaternion.identity);
            while (true)
            {
                int generatedAnswer = ans + (i * Random.Range(-3, 4));
                if (!answers.Contains(generatedAnswer))
                {
                    op.GetComponent<Option>().setVal(generatedAnswer);
                    activeOptions.Add(op);
                    answers.Add(generatedAnswer);
                    break;
                }
            }
        }
        return ans;
    }

    //Generate Question from custom pool
    private int GenerateQuestionCustomPool()
    {
        int ans = 0;

        if (customQs.Count > 0)
        {
            string nextqn = customQs[0];
            string[] tok = nextqn.Split(',');

            num1 = int.Parse(tok[1]);
            num2 = int.Parse(tok[2]);

            switch (tok[2])
            {
                case "0":
                    questionText.SetText(string.Format("Qn {0}: {1} + {2} = ?", qnIdx, num1, num2));
                    ans = num1 + num2;
                    break;
                case "1":
                    questionText.SetText(string.Format("Qn {0}: {1} - {2} = ?", qnIdx, num1, num2));
                    ans = num1 - num2;
                    break;
                case "2":
                    questionText.SetText(string.Format("Qn {0}: {1} x {2} = ?", qnIdx, num1, num2));
                    ans = num1 * num2;
                    break;
                case "3":
                    questionText.SetText(string.Format("Qn {0}: {1} ÷ {2} = ?", qnIdx, num1, num2));
                    ans = num1 / num2;
                    break;
                default:
                    questionText.SetText(string.Format("Qn {0}: {1} + {2} = ?", qnIdx, num1, num2));
                    ans = num1 + num2;
                    break;
            }

            customQs.RemoveAt(0);
        }
        else
        {
            Debug.Log("end of custom questions");
        }

        return ans;
    }

    //Answer Collided, when player touches one of the options
    public void AnswerCollided(int val)
    {
        //check answer
        if (val == answer)
        {
            combo++;
            score += combo * 1;
            if (combo > 2)
                curSpeed *= 1.2f;
            foreach (BGScroller bg in BGs)
                bg.UpdateSpeed();
            audioSauce.clip = correctSfx;
            questionText.SetText("Correct!");
        }
        else
        {
            combo = 0;
            curSpeed = baseSpeed;
            foreach (BGScroller bg in BGs)
                bg.UpdateSpeed();
            audioSauce.clip = wrongSfx;
            questionText.SetText("Incorrect!");
        }
        scoreText.SetText(string.Format("Score: {0}", score));
        comboText.SetText(string.Format("Combo: {0}", combo));
        
        //destroy all active options
        foreach (GameObject g in activeOptions)
        {
            Destroy(g);
        }

        intervalTime = 5f;
        isInterval = true;
        StartCoroutine(NextQuestion());

        audioSauce.Play();
    }

    /// <summary>
    /// Coroutine to call next question
    /// </summary>
    /// <returns></returns>
    IEnumerator NextQuestion()
    {
        qnIdx++;
        if (qnIdx > numQns)
        {
            EndLevel();
            yield return null;
        }
        else
        {
            while (intervalTime > 0)
            {
                float curTime = Time.time;
                yield return new WaitForSeconds(0.1f);
                float newTime = Time.time - curTime;
                intervalTime -= newTime;
                if (intervalTime <= 3f)
                {
                    questionText.SetText("Next Question in " + Mathf.RoundToInt(intervalTime).ToString());
                }
            }
            NewQuestion();
            
        }
    }

    private void EndLevel()
    {
        questionText.SetText(string.Format("Game Overrr (Temp)"));

        //WIP
    }

    //Get Game speed
    public float GetGameSpeed()
    {
        return curSpeed;
    }

    //shuffle list
    private void Shuffle(List<GameObject> list)
    {
	    for (int i = list.Count - 1; i > 0; i--)
	    {
            int n = Random.Range(0, i + 1);
		    GameObject temp = list[i];
		    list[i] = list[n];
		    list[n] = temp;
	    }
    }
}