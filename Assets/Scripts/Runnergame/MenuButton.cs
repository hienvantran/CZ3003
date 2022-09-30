using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuButton : MonoBehaviour
{

    public int numQns;
    public float difficulty;
    public QuestionManager.OpMode opMode;
    public string seed;

    [SerializeField] private TMP_InputField seedInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //load normal
    public void LoadLevel()
    {
        LevelManager.instance.SetParams(numQns, difficulty, opMode);
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

}
