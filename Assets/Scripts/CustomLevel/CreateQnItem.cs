using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateQnItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI qnNumberLabel;
    [SerializeField] private TMP_Dropdown opInput;
    [SerializeField] private GameObject num1Input, num2Input;
    
    private int qnNumber, num1, num2, op;

    //set values
    public void setValues(int qNum)
    {
        qnNumber = qNum;
        qnNumberLabel.SetText(string.Format("Q{0}.", qNum));
        op = opInput.value;
        num1 = -9999;
        num2 = -9999;
    }

    //get
    public int getQnNumber()
    {
        return qnNumber;
    }
    public int getNum1()
    {
        return num1;
    }
    public int getNum2()
    {
        return num2;
    }
    public int getOp()
    {
        return op;
    }
    public string getQnString()
    {
        return string.Format("{0},{1},{2},{3}", qnNumber, num1, op, num2);
    }

    //check if this question is valid
    public bool isValidQn()
    {
        bool valid = true;

        if (num1 == -9999 || num2 == -9999)
        {
            valid = false;
        }

        return valid;
    }

    //On change num1 input field
    public void changeNum1()
    {
        int temp;
        if(int.TryParse(num1Input.GetComponent<TMP_InputField>().text, out temp))
        {
            num1 = temp;
        }
        else
        {
            num1 = -9999;
        }
    }

    //On change num2 input field
    public void changeNum2()
    {
        int temp;
        if (int.TryParse(num2Input.GetComponent<TMP_InputField>().text, out temp))
        {
            num2 = temp;
        }
        else
        {
            num2 = -9999;
        }
    }

    //On change operator dropdown selection
    public void changeOp()
    {
        op = opInput.value;
    }
}
