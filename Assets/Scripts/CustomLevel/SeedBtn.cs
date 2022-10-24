using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SeedBtn : MonoBehaviour
{
    public TextMeshProUGUI tmp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string text)
    {
        Debug.Log("Seed: " + text);
        tmp.text = text;
    }

    public void OnClick()
    {
        GUIUtility.systemCopyBuffer = tmp.text;
    }
}
