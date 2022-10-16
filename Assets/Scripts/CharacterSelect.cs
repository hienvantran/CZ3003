using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelect : MonoBehaviour
{
    private int curChar = 0;

    [SerializeField] List<AnimatorController> charList;
    [SerializeField] Animator charAnim;
    [SerializeField] TextMeshProUGUI curCharText;
    LevelManager lm;
    // Start is called before the first frame update
    void Start()
    {
        lm = LevelManager.Instance;
        charAnim.runtimeAnimatorController = charList[lm.charSelected];
        curChar = lm.charSelected;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextChar()
    {
        curChar++;
        if (curChar >= charList.Count)
            curChar = 0;
        charAnim.runtimeAnimatorController = charList[curChar];
        curCharText.SetText((curChar + 1).ToString());
        lm.charSelected = curChar;
    }

    public void PrevChar()
    {
        curChar--;
        if (curChar < 0)
            curChar = charList.Count - 1;
        charAnim.runtimeAnimatorController = charList[curChar];
        curCharText.SetText((curChar + 1).ToString());
        lm.charSelected = curChar;
    }
}
