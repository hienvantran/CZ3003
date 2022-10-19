using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuButton : MonoBehaviour
{

    public int numQns;
    public float difficulty;
    public QuestionManager.OpMode opMode;
    public string levelString;
    public int level;
    private GameObject tooltip;
    private TextMeshProUGUI leveltext;

    [SerializeField] private TMP_InputField seedInput;


    // Start is called before the first frame update
    void Start()
    {
        
        leveltext = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        tooltip = transform.Find("Tooltip").gameObject;

        leveltext.SetText(string.Format("Level {0}", level));
        tooltip.GetComponent<TextMeshProUGUI>().SetText(string.Format("Complete Level {0} with at least half correct to unlock", level - 1));

        switch (opMode)
        {
            case QuestionManager.OpMode.ADD:
                levelString = "A" + level;
                break;
            case QuestionManager.OpMode.SUB:
                levelString = "S" + level;
                break;
            case QuestionManager.OpMode.MUL:
                levelString = "M" + level;
                break;
            case QuestionManager.OpMode.DIV:
                levelString = "D" + level;
                break;
        }

        CheckProgress();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    //Load normal
    public void LoadLevel()
    {
        LevelManager.Instance.SetParams(numQns, difficulty, opMode);
        LevelManager.Instance.SetLevel(levelString);
        LevelManager.Instance.previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("RunnerGame");
    }

    //Load custom
    public void LoadLevelCustom()
    {
        StartCoroutine(LoadCustomLevel());
    }
    public IEnumerator LoadCustomLevel()
    {
        //decode seed for options
        Debug.Log("it's custom time");
        string seed = seedInput.text;

        //get qstring by seed(assignment key)
        var qnStrParamsTask = FirestoreManager.Instance.getAssignmentQnsStrbyID(seed, res =>
        {
            Debug.Log("qnStr is : " + res);
            (int, string) seedParams = SeedEncoder.DecodeSeed(res);
            LevelManager.Instance.SetParams(seedParams.Item1, 1, QuestionManager.OpMode.CUS, seedParams.Item2, seed);
        });

        //wait
        yield return new WaitUntil(predicate: () => qnStrParamsTask.IsCompleted);

        //set prev scene and launch runner game scene
        LevelManager.Instance.previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("RunnerGame");
    }

    //tooltip for levels that are still locked
    public void MouseEnter()
    {
        if (!GetComponent<Button>().interactable)
        {
            tooltip.SetActive(true);
        }
    }
    public void MouseExit()
    {
        if (!GetComponent<Button>().interactable)
        {
            tooltip.SetActive(false);
        }
    }

    //check progress and set interactability
    private void CheckProgress()
    {

        //Add
        if (opMode == QuestionManager.OpMode.ADD)
        {
            if (LevelManager.Instance.addProgress >= level - 1)
            {
                GetComponent<Button>().interactable = true;
                leveltext.color = Color.white;
            }
        }
        //Sub
        else if (opMode == QuestionManager.OpMode.SUB)
        {
            if (LevelManager.Instance.subProgress >= level - 1)
            {
                GetComponent<Button>().interactable = true;
                leveltext.color = Color.white;
            }
        }
        //Mul
        else if (opMode == QuestionManager.OpMode.MUL)
        {
            if (LevelManager.Instance.mulProgress >= level - 1)
            {
                GetComponent<Button>().interactable = true;
                leveltext.color = Color.white;
            }
        }
        //Div
        else if (opMode == QuestionManager.OpMode.DIV)
        {
            if (LevelManager.Instance.divProgress >= level - 1)
            {
                GetComponent<Button>().interactable = true;
                leveltext.color = Color.white;
            }
        }
    }
}
