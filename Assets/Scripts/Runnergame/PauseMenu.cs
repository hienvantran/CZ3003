using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public AudioMixer mixer;
    public Slider musicSlider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PauseBtn()
    {
        
        OpenMenu();
        Time.timeScale = 0;
    }

    public void ResumeBtn()
    {
        Time.timeScale = 1;
        PlayerPrefs.Save();
        CloseMenu();
    }

    public void QuitBtn(string name)
    {
        Time.timeScale = 1;
        PlayerPrefs.Save();
        SceneManager.LoadScene(name);
    }

    public void QuitBtn()
    {
        Time.timeScale = 1;
        PlayerPrefs.Save();
        if (LevelManager.Instance.previousScene != "")
            SceneManager.LoadScene(LevelManager.Instance.previousScene);
        else
            SceneManager.LoadScene("MainMenu");
    }

    public void OpenMenu()
    {
        musicSlider.GetComponent<SetVolume>().UpdateSlider();
        gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }


}
