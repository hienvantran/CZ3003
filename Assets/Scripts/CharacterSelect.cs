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
        curChar = lm.charSelected;
        SetChar();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextChar()
    {
        curChar++;
        if (curChar >= lm.charUIAnimList.Count)
            curChar = 0;
        SetChar();
    }

    public void PrevChar()
    {
        curChar--;
        if (curChar < 0)
            curChar = lm.charUIAnimList.Count - 1;
        SetChar();
    }

    private void SetChar()
    {
        charAnim.runtimeAnimatorController = lm.charUIAnimList[curChar];
        curCharText.SetText((curChar + 1).ToString());
        lm.charSelected = curChar;
    }
}
