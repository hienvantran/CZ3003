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

    [SerializeField] private TMP_InputField seedInput;


    // Start is called before the first frame update
    void Start()
    {
        CheckProgress();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Load normal
    public void LoadLevel()
    {
        LevelManager.instance.SetParams(numQns, difficulty, opMode);
        LevelManager.instance.SetLevel(levelString);
        SceneManager.LoadScene("RunnerGame");
    }

    //Load custom
    public void LoadCustomLevel()
    {
        //decode seed for options
        Debug.Log("it's custom time");
        string seed = seedInput.text;
        (int, string) seedParams = SeedEncoder.DecodeSeed(seed);

        LevelManager.instance.SetParams(seedParams.Item1, 1, QuestionManager.OpMode.CUS, seedParams.Item2);
        SceneManager.LoadScene("RunnerGame");
    }

    //tooltip for levels that are still locked
    public void MouseEnter()
    {
        if (!GetComponent<Button>().interactable)
        {
            transform.Find("Tooltip").gameObject.SetActive(true);
        }
    }
    public void MouseExit()
    {
        if (!GetComponent<Button>().interactable)
        {
            transform.Find("Tooltip").gameObject.SetActive(false);
        }
    }

    //check progress and set interactability
    private void CheckProgress()
    {
        //Add
        if (opMode == QuestionManager.OpMode.ADD)
        {
            if (this.gameObject.name.Equals("Level2Btn") && LevelManager.instance.addProgress >= 1)
            {
                GetComponent<Button>().interactable = true;
            }
            else if (this.gameObject.name.Equals("Level3Btn") && LevelManager.instance.addProgress >= 2)
            {
                GetComponent<Button>().interactable = true;
            }
        }
        //Sub
        else if (opMode == QuestionManager.OpMode.SUB)
        {
            if (this.gameObject.name.Equals("Level2Btn") && LevelManager.instance.subProgress >= 1)
            {
                GetComponent<Button>().interactable = true;
            }
            else if (this.gameObject.name.Equals("Level3Btn") && LevelManager.instance.subProgress >= 2)
            {
                GetComponent<Button>().interactable = true;
            }
        }
        //Mul
        else if (opMode == QuestionManager.OpMode.MUL)
        {
            if (this.gameObject.name.Equals("Level2Btn") && LevelManager.instance.mulProgress >= 1)
            {
                GetComponent<Button>().interactable = true;
            }
            else if (this.gameObject.name.Equals("Level3Btn") && LevelManager.instance.mulProgress >= 2)
            {
                GetComponent<Button>().interactable = true;
            }
        }
        //Div
        else if (opMode == QuestionManager.OpMode.DIV)
        {
            if (this.gameObject.name.Equals("Level2Btn") && LevelManager.instance.divProgress >= 1)
            {
                GetComponent<Button>().interactable = true;
            }
            else if (this.gameObject.name.Equals("Level3Btn") && LevelManager.instance.divProgress >= 2)
            {
                GetComponent<Button>().interactable = true;
            }
        }
    }
}
