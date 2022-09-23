using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;

public class QuestionManager : MonoBehaviour
{
    public enum OpMode { ADD, SUB, MUL, DIV }

    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private List<GameObject> optionSpawnList;
    [SerializeField] private GameObject optionPrefab;

    [Header("Game Settings")]
    [SerializeField] private OpMode opMode;
    [SerializeField] private float speed = 1f;

    private AudioSource audioSauce;
    [SerializeField] private AudioClip correctSfx;
    [SerializeField] private AudioClip wrongSfx;
    [SerializeField] private AudioMixer mixer;

    public static QuestionManager instance;
    private int num1, num2, answer;
    private int score = 0;
    private int combo = 0;
    private List<GameObject> activeOptions;
    private float intervalTime;
    private bool isInterval = false;

    
    private bool isPlaying = false;
    private bool isEndless = false;
    private int numQns = 10;
    private int qnIdx = 1;
    public bool isDebug = true;

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
        //SetVolume();
        //if (isDebug)
        //    StartLevel(10, opMode);

    }

    //Update
    void Update()
    {
        //Interval();
    }

    public void StartLevel(int numQns, OpMode mode)
    {
        if (isPlaying)
            return;
        isPlaying = true;
        this.numQns = numQns;
        opMode = mode;
        qnIdx = 0;
        intervalTime = 3f;
        StartCoroutine(NextQuestion());
    }

    //Create a new question
    private void NewQuestion()
    {
        answer = GenerateOptions(GenerateQuestion());
    }

    //Generate Question
    private int GenerateQuestion()
    {
        
        int ans = 0;

        //Addition
        if (opMode == OpMode.ADD)
        {
            num1 = Random.Range(1, 10);
            num2 = Random.Range(1, 10);
            //set qn and ans
            questionText.SetText(string.Format("Qn {0}: {1} + {2} = ?", qnIdx, num1, num2));
            ans = num1 + num2;
        }
        //Subtraction
        else if (opMode == OpMode.SUB)
        {
            num1 = Random.Range(2, 10);
            num2 = Random.Range(1, num1);
            //set qn and ans
            questionText.SetText(string.Format("Qn {0}: {1} - {2} = ?", qnIdx, num1, num2));
            ans = num1 - num2;
        }
        //Multiplication
        if (opMode == OpMode.MUL)
        {
            num1 = Random.Range(2, 10);
            num2 = Random.Range(2, 11-num1);
            //set qn and ans
            questionText.SetText(string.Format("Qn {0}: {1} X {2} = ?", qnIdx, num1, num2));
            ans = num1 * num2;
        }
        //Division
        if (opMode == OpMode.DIV)
        {
            num1 = Random.Range(2, 10);
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

    //Answer Collided, when player touches one of the options
    public void AnswerCollided(int val)
    {
        //check answer
        if (val == answer)
        {
            combo++;
            score += combo * 1;
            audioSauce.clip = correctSfx;
            questionText.SetText("Correct!");
        }
        else
        {
            combo = 0;
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
        return speed;
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