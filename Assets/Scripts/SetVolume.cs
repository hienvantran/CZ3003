using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{

    public AudioMixer mixer;

    public void Start()
    {
        //float musicVol;
        //mixer.GetFloat("MusicVol", out musicVol);
        //GetComponent<Slider>().value = musicVol;
    }

    public void UpdateSlider()
    {
        GetComponent<Slider>().value = PlayerPrefs.GetFloat("MusicVol");
    }

    public void SetLevel(float sliderVal)
    {
        mixer.SetFloat("MusicVol", Mathf.Log10(sliderVal) * 20);
        PlayerPrefs.SetFloat("MusicVol", sliderVal);
    }
}
