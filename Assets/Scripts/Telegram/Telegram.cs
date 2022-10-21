using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;

public class Telegram : MonoBehaviour
{

    [System.Serializable]
    public class View
    {
        [Header("Buttons")]
        public Button sendMessage;

        [Space(10)]
        [Header("Text")]
        public TextMeshProUGUI inputText;
    }

    public Button sendMessage;
    public TextMeshProUGUI inputText;
    public string TOKEN = "00000:aaaaaa";
    Regex chatIdRegex = new Regex(@"chat.:{.id.:(-[0-9]+)");
    // Start is called before the first frame update
    void Start()
    {
        sendMessage.onClick.AddListener(() =>
        {
            StartCoroutine(SendMessage(inputText.text));
        });
    }

    public string API_URL
    {
        get
        {
            return string.Format("https://api.telegram.org/bot{0}/", TOKEN);
        }
    }

    public void GetMe()
    {
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Post(API_URL + "getMe", form);
        StartCoroutine(SendRequest(www));
    }

    IEnumerator GetUpdates(Action<string> callback = null)
    {
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Post(API_URL + "getUpdates", form);
        yield return StartCoroutine(SendRequest(www, res =>
        {
            callback?.Invoke(res);
        }));
    }

    public new IEnumerator SendMessage(string text)
    {
        string chat_id = "";
        List<string> chat_ids = new List<string>();
        yield return StartCoroutine(GetUpdates(res =>
        {
            var result = chatIdRegex.Matches(res);
            if (result.Count > 0)
            {
                foreach (Match match in result)
                {
                    GroupCollection captures = match.Groups;
                    chat_id = captures[1].Value;
                    
                    if (!chat_ids.Contains(chat_id))
                    {
                        Debug.Log(chat_id);
                        chat_ids.Add(chat_id);
                        WWWForm form = new WWWForm();
                        form.AddField("chat_id", chat_id);
                        form.AddField("text", text);
                        UnityWebRequest www = UnityWebRequest.Post(API_URL + "sendMessage?", form);
                        StartCoroutine(SendRequest(www));
                    }
                }
            }
        }));
    }

    IEnumerator SendRequest(UnityWebRequest www, Action<string> callback = null)
    {
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Success!\n" + www.downloadHandler.text);
            callback?.Invoke(www.downloadHandler.text);
        }
    }
}
