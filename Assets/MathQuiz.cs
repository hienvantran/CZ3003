using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MathQuiz : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI qnText;
    [SerializeField] private TextMeshProUGUI totalAnsText;
    [SerializeField] private TextMeshProUGUI totalAnsText2;
    [SerializeField] private Button quitBtn;
    [SerializeField] private Button option1Btn;
    [SerializeField] private Button option2Btn;
    [SerializeField] private Button option3Btn;
    [SerializeField] private Button option4Btn;
    [SerializeField] private GameObject quizScreen;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private Slider timerBar;

    List<Button> btnList = new List<Button>();
    int valX, valY, ansBtn, totalCorrect, totalWrong;
    public float timerTime = 15f;
    private float curTime;

    // Start is called before the first frame update
    void Start()
    {
        btnList.Add(option1Btn);
        btnList.Add(option2Btn);
        btnList.Add(option3Btn);
        btnList.Add(option4Btn);

        NewGame();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void NewQuestion()
    {
        valX = Random.Range(1, 10);
        valY = Random.Range(1, 10);
        int answer = valX + valY;
        ansBtn = Random.Range(1, btnList.Count+1);
        List<int> fakeAnswers = new List<int>();
        qnText.SetText(string.Format("{0} + {1} = ?", valX, valY));

        for (int i=0; i < btnList.Count; i++)
        {
            if (i == ansBtn-1)
                btnList[i].GetComponentInChildren<TextMeshProUGUI>().SetText(answer.ToString());
            else
            {
                while (true)
                {
                    int fake = Random.Range((btnList.Count + 2)*-1, btnList.Count + 3);
                    if (!fakeAnswers.Contains(fake) && fake != 0)
                    {
                        fakeAnswers.Add(fake);
                        btnList[i].GetComponentInChildren<TextMeshProUGUI>().SetText((answer + fake).ToString());
                        break;
                    }
                }
            }
        }
    }

    public void CheckAnswer(int btnId)
    {
        if (btnId == ansBtn)
        {
            totalCorrect++;
            UpdateTotalAnswerText();
            curTime += 1f;
        }
        else
        {
            totalWrong++;
            UpdateTotalAnswerText();
            curTime -= 0.5f;
        }
        NewQuestion();
    }

    private void UpdateTotalAnswerText()
    {
        totalAnsText.SetText(string.Format("Correct: {0}\nWrong: {1}", totalCorrect, totalWrong));
        totalAnsText2.SetText(string.Format("Correct: {0}\nWrong: {1}", totalCorrect, totalWrong));
    }

    public void QuitLevel()
    {
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator StartTimer()
    {
        curTime = timerTime;

        while (curTime > 0)
        {
            timerBar.value = (curTime / timerTime);
            Debug.Log(curTime);
            curTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        GameOver();
    }

    private void GameOver()
    {
        quizScreen.SetActive(false);
        gameOverScreen.SetActive(true);
    }

    public void NewGame()
    {
        totalCorrect = 0;
        totalWrong = 0;
        UpdateTotalAnswerText();
        quizScreen.SetActive(true);
        gameOverScreen.SetActive(false);
        NewQuestion();
        StartCoroutine(StartTimer());
    }

}
