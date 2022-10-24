using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using UnityEngine.UI;
using TMPro;
using System.Threading;

public class Telegram : MonoBehaviour
{

    public Button sendMessage;
    public Button updateChats;
    public Button cancelBtn;
    public Transform seedListContent;
    public GameObject seedRowPrefab;
    public TMP_InputField seedText;
    public TextMeshProUGUI msgText;
    public TextMeshProUGUI copyText;
    private int taskCount = 0;
    public GameObject errorText;
    
    public string TOKEN = "00000:aaaaaa";
    Regex chatIdRegex = new Regex(@"chat.:{.id.:(-[0-9]+)");
    List<string> chatIdList = new List<string>();

    private bool isUpdating = false;

    // Start is called before the first frame update
    void Start()
    {
        sendMessage?.onClick.AddListener(() =>
        {
            StartCoroutine(SendMessage(seedText.text, msgText.text));
        });
        updateChats?.onClick.AddListener(() =>
        {
            StartCoroutine(UpdateChats());
        });
        cancelBtn?.onClick.AddListener(() =>
        {
            CancelBtn();
        });
        Task t = FirestoreManager.Instance.GetChatIDs(res =>
        {
            chatIdList = res;
        });
        StartCoroutine(LoadSeedList());
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
        if (isUpdating)
            yield break;
        else
            isUpdating = true;

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
        Task t = FirestoreManager.Instance.GetChatIDs(res =>
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
                FirestoreManager.Instance.SaveChatID(id);
            }
        }
        isUpdating = false;
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
    public IEnumerator SendMessage(string text, string msg = null)
    {
        List<string> assignIds = new List<string>();
        Task t = FirestoreManager.Instance.GetAssignments(result =>
        {
            assignIds = result;
        });
        yield return new WaitUntil(() => t.IsCompleted);


        if (string.IsNullOrEmpty(text))
        {
            StartCoroutine(DisplayError("Error - Missing level seed!"));
            yield break;
        }

        if (!assignIds.Contains(text))
        {
            Debug.Log("Seed: " + text);
            foreach (string seed in assignIds)
                Debug.Log(seed);
            StartCoroutine(DisplayError("Error - Invalid level seed!"));
            yield break;
        }

        yield return null;
        FirebaseUser user = FirebaseManager.Instance.User;
        string message = string.Format(
            "New {0} from {1}!\nLevel Code: {2}{3}",
            FirebaseManager.Instance.IsCurrentUserTeacher() ? "Assignment" : "Challenge",
            user.DisplayName,
            text,
            msg != null && msg != ""? ("\n" + msg) : "");

        foreach (string id in chatIdList)
        {
            WWWForm form = new WWWForm();
            form.AddField("chat_id", id);
            form.AddField("text", message);
            UnityWebRequest www = UnityWebRequest.Post(API_URL + "sendMessage?", form);
            StartCoroutine(SendRequest(www));
        }

        StartCoroutine(DisplayError("Shared!"));
    }

    public IEnumerator DisplayError(string text = null)
    {
        if (errorText.activeSelf)
            yield break;
        if (!string.IsNullOrEmpty(text))
            errorText.GetComponent<TextMeshProUGUI>().SetText(text);
        errorText.SetActive(true);
        yield return new WaitForSeconds(3);
        errorText.SetActive(false);
    }

    public void CancelBtn()
    {
        seedText.text = "";
        msgText.text = "";
        errorText.SetActive(false);
    }

    public IEnumerator LoadSeedList()
    {
        List<string> keyList = new List<string>();
        Task t = FirestoreManager.Instance.GetAssignmentKeysByUID(FirebaseManager.Instance.User.UserId, res =>
        {
            Debug.Log("Key Count:" + res.Count);
            keyList = res;
            
        });
        
        yield return new WaitUntil(() => t.IsCompleted);
        foreach (string key in keyList)
        {
            Debug.Log("Key: " + key);
            GameObject row = Instantiate(seedRowPrefab, seedListContent);
            //Debug.Log(row.name);
            row.GetComponent<SeedBtn>().SetText(key, this);
        }
    }

    public IEnumerator DisplayClipboardMsg(string seed)
    {
        taskCount++;
        copyText.SetText(string.Format("{0} has been copied to your clipboard!", seed));
        copyText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        taskCount--;
        if (taskCount == 0)
            copyText.gameObject.SetActive(false);
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
