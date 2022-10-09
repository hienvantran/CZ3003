using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using System;
using TMPro;
using Facebook.Unity.Example;

public class FBManager : MonoBehaviour
{
    public TextMeshProUGUI logText;

    // Awake function from Unity's MonoBehavior
    void Awake()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    public void FBLogin()
    {
        var perms = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }

    public void FBLogOut()
    {
        FB.LogOut();
        logText.SetText("User logged out");
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }
            DisplayProfile();
        }
        else
        {
            Debug.Log("User cancelled login");
            logText.SetText(result.Error);
        }
    }

    private void DisplayProfile()
    {
        var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
        FB.API("/me?fields=first_name", HttpMethod.GET, DisplayUsername);
        
    }

    private void DisplayUsername(IResult result)
    {
        if (result.Error == null)
        {
;            string name = "" + result.ResultDictionary["first_name"];

            Debug.Log("name: " + name);
            logText.SetText("Name: " + name);
        } 
        else
        {
            logText.SetText(result.Error);
        }
    }

    public void FBSharePost()
    {
        FBGamingServices.UploadImageToMediaLibrary(
                                "test Image",
                                new Uri("C:/Users/Admin/Pictures/JohnCena.jpg"),
                                true,
                                HandleResult);
    }

    private void ShareCallback(IShareResult result)
    {
        if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
        {
            Debug.Log("ShareLink Error: " + result.Error);
            logText.SetText(result.Error);
        }
        else if (!String.IsNullOrEmpty(result.PostId))
        {
            // Print post identifier of the shared content
            Debug.Log(result.PostId);
        }
        else
        {
            // Share succeeded without postID
            Debug.Log("ShareLink success!");
        }
    }

    protected void HandleResult(IResult result)
    {
        if (result == null)
        {
            LogView.AddLog("Null Response\n");
            return;
        }

        // Some platforms return the empty string instead of null.
        if (!string.IsNullOrEmpty(result.Error))
        {
            // handle error case here.
        }
        else if (result.Cancelled)
        {
            // a dialog was cancelled.
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            // success case! Do something useful with this.
        }
        else
        {
            // we got an empty response
        }
    }

}
