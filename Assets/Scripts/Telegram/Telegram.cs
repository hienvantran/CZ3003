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
    public Button updateChats;
    public TextMeshProUGUI inputText;
    public string TOKEN = "00000:aaaaaa";
    Regex chatIdRegex = new Regex(@"chat.:{.id.:(-[0-9]+)");
    List<string> chatIdList = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        sendMessage?.onClick.AddListener(() =>
        {
            StartCoroutine(SendMessage(inputText.text));
        });
        updateChats?.onClick.AddListener(() =>
        {
            StartCoroutine(UpdateChats());
        });
        Task t = FirestoreManager.Instance.getChatIDs(res =>
        {
            chatIdList = res;
        });
    }

    public string API_URL
    {
        get
        {
            return string.Format("https://api.telegram.org/bot{0}/", TOKEN);
        }
    }

    /// <summary>
    /// Get telegram bot details
    /// </summary>
    public void GetMe()
    {
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Post(API_URL + "getMe", form);
        StartCoroutine(SendRequest(www));
    }

    /// <summary>
    /// Update list of telegram chat_id in Firebase
    /// </summary>
    /// <returns></returns>
    public IEnumerator UpdateChats()
    {
        string chat_id = "";
        List<string> updatedIds = new List<string>();
        bool teleDone = false;

        // Get ids of new chat groups
        StartCoroutine(GetUpdates(res =>
        {
            var result = chatIdRegex.Matches(res);
            if (result.Count > 0)
            {
                foreach (Match match in result)
                {
                    GroupCollection captures = match.Groups;
                    chat_id = captures[1].Value;

                    if (!updatedIds.Contains(chat_id))
                    {
                        Debug.Log(chat_id);
                        updatedIds.Add(chat_id);
                    }
                }
            }
            teleDone = true;
        }));

        // Get stored list of ids in Firebase 
        Task t = FirestoreManager.Instance.getChatIDs(res =>
        {
            chatIdList = res;
        });

        yield return new WaitUntil(() => t.IsCompleted && teleDone);

        // Compare and save new ids in Firebase
        foreach (string id in updatedIds)
        {
            if (!chatIdList.Contains(id))
            {
                chatIdList.Add(id);
                FirestoreManager.Instance.saveChatID(id);
            }
        }
    }

    /// <summary>
    /// Telegram bot getUpdates API call
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    IEnumerator GetUpdates(Action<string> callback = null)
    {
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Post(API_URL + "getUpdates", form);
        yield return StartCoroutine(SendRequest(www, res =>
        {
            callback?.Invoke(res);
        }));
    }

    /// <summary>
    /// Sends a message to every chat group that the telegram bot is in
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public new IEnumerator SendMessage(string text)
    {
        yield return null;
        foreach (string id in chatIdList)
        {
            WWWForm form = new WWWForm();
            form.AddField("chat_id", id);
            form.AddField("text", text);
            UnityWebRequest www = UnityWebRequest.Post(API_URL + "sendMessage?", form);
            StartCoroutine(SendRequest(www));
        }
    }

    /// <summary>
    /// Generic web request
    /// </summary>
    /// <param name="www"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
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
