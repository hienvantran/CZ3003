using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{

    private float numQns = 0;
    private float curQn = 0;
    private Slider slider;
    [SerializeField] Animator handleAnim;

    // Start is called before the first frame update
    void Awake()
    {
        slider = gameObject.GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeBar(int qns)
    {
        numQns = qns;
        curQn = 0;
        slider.value = 0;
        handleAnim.runtimeAnimatorController = LevelManager.Instance.charUIAnimList[LevelManager.Instance.charSelected];
    }

    public void UpdateProgress(int curQn)
    {
        slider.value = curQn / numQns;
    }

    public void NextQn()
    {
        curQn += 1;
        slider.value = curQn / numQns;
    }
}
