using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestionManager : MonoBehaviour
{
    private enum OpMode { ADD, SUB, MUL, DIV }

    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private List<GameObject> optionSpawnList;
    [SerializeField] private GameObject optionPrefab;

    [Header("Game Settings")]
    [SerializeField] private OpMode opMode;
    [SerializeField] private float speed = 1f;

    public static QuestionManager instance;
    private int num1, num2, answer;
    private int score = 0;
    private List<GameObject> activeOptions;
    private float intervalTime;
    private bool isInterval = false;

    //Awake
    void Awake()
    {
        instance = this;
    }

    //Start
    void Start()
    {
        activeOptions = new List<GameObject>();
        NextQuestion();
    }

    //Update
    void Update()
    {
        Interval();
    }

    //Next Question
    private void NextQuestion()
    {
        answer = GenerateOptions(GenerateQuestion());
    }

    //Generate Question
    private int GenerateQuestion()
    {
        num1 = Random.Range(1, 10);
        num2 = Random.Range(1, 10);
        int ans = 0;

        //Addition
        if (opMode == OpMode.ADD)
        {
            //set qn and ans
            questionText.SetText(string.Format("{0} + {1} = ?", num1, num2));
            ans = num1 + num2;
        }
        //Subtraction
        else if (opMode == OpMode.SUB)
        {
            //make the numbers friendly for pri sch kids
            //if ans will be 0, add 1 ~ 3 to num1
            if (num1 == num2)
            {
                num1 += Random.Range(1, 4);
            }
            //if ans will be -tive, swap num1 & num2
            else if (num1 < num2)
            {
                num1 = num1 ^ num2;
                num2 = num1 ^ num2;
                num1 = num1 ^ num2;
            }
            //set qn and ans
            questionText.SetText(string.Format("{0} - {1} = ?", num1, num2));
            ans = num1 - num2;
        }
        //Multiplication
        if (opMode == OpMode.MUL)
        {
            //set qn and ans
            questionText.SetText(string.Format("{0} x {1} = ?", num1, num2));
            ans = num1 * num2;
        }
        //Division
        if (opMode == OpMode.DIV)
        {
            //make sure num1 is > num2 and that num1 % num2 = 0
            if (num1 == num2)
            {
                num1++;
            }
            else if (num1 < num2)
            {
                num1 = num1 ^ num2;
                num2 = num1 ^ num2;
                num1 = num1 ^ num2;
            }
            int remainder = num1 % num2;
            num1 += (num2 - remainder);

            //set qn and ans
            questionText.SetText(string.Format("{0} ÷ {1} = ?", num1, num2));
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
                int generatedAnswer = ans + (i * Random.Range(-4, 5));
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
            score++;
            questionText.SetText("Correct!");
        }
        else
        {
            questionText.SetText("Incorrect!");
        }
        scoreText.SetText(string.Format("Score: {0}", score));
        
        //destroy all active options
        foreach (GameObject g in activeOptions)
        {
            Destroy(g);
        }

        intervalTime = 5f;
        isInterval = true;
    }

    //Interval between questions
    private void Interval()
    {
        if (isInterval)
        {
            intervalTime -= Time.deltaTime;
            if (intervalTime <= 3f)
            {
                questionText.SetText("Next Question in " + Mathf.RoundToInt(intervalTime).ToString());
            }
            if (intervalTime <= 0)
            {
                isInterval = false;
                NextQuestion();
            }
        }
    }

    //Get Game speed
    public float GetGameSpeed()
    {
        return this.speed;
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