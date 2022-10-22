using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{

    public AudioMixer mixer;
    public Slider musicSlider;

    float musicVol;

    // Start is called before the first frame update
    void Start()
    {
        SetVolume();
        SetSliders();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetVolume()
    {
        musicVol = PlayerPrefs.GetFloat("MusicVol", 0.5f);
        mixer.SetFloat("MusicVol", Mathf.Log10(musicVol) * 20);
    }

    private void SetSliders()
    {
        musicSlider.value = musicVol;
    }
}
