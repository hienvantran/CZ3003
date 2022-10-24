using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SeedBtn : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    private Telegram tele;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string text, Telegram tele)
    {
        this.tele = tele;
        Debug.Log("Seed: " + text);
        tmp.text = text;
    }

    public void OnClick()
    {
        GUIUtility.systemCopyBuffer = tmp.text;
        StartCoroutine(tele.DisplayClipboardMsg(tmp.text));
    }
}
