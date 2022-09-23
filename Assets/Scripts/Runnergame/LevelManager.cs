using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider musicSlider;
    public int debugNumQns = 10;
    public QuestionManager.OpMode debugOpMode = QuestionManager.OpMode.ADD;
    QuestionManager qm;
    float musicVol;

    // Start is called before the first frame update
    void Start()
    {
        qm = QuestionManager.instance;
        SetVolume();
        SetSliders();
        qm.StartLevel(debugNumQns, debugOpMode);
    }

    private void SetVolume()
    {
        musicVol = PlayerPrefs.GetFloat("MusicVol");
        Debug.Log(musicVol);
        if (musicVol == 0.001f)
        {
            musicVol = 0.8f;
            PlayerPrefs.SetFloat("MusicVol", 0.8f);
            PlayerPrefs.Save();
        }
        mixer.SetFloat("MusicVol", Mathf.Log10(musicVol) * 20);
    }

    private void SetSliders()
    {
        musicSlider.value = musicVol;
    }
}
