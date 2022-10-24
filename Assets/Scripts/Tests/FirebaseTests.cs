using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class FirebaseTests
{
    // users collection

    [UnityTest]
    public IEnumerator AddUser()
    {
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        
        FirestoreManager fsm = FirestoreManager.Instance;

        fm.DoesUserExist(
            "test@mail.com",
            "123456",
            "username", "Student", result =>
            {
                Assert.IsTrue(result);
            });
    }

    // assignments collection

    [UnityTest]
    public IEnumerator AddAssignment()
    {
        yield return SceneManager.LoadSceneAsync("Login");
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        FirestoreManager fsm = FirestoreManager.Instance;

        Dictionary<string, object> addRes = new Dictionary<string, object>();
        fsm.AddAssignment("assignmentId", "qnsStr", "user", res =>
        {
            addRes = res;
            fsm.DeleteAssignment("assignmentId", res =>
            {
                Assert.IsNull(addRes);
            });
        });
        
    }

    [UnityTest]
    public IEnumerator AddUserAssignmentAttempts()
    {
        yield return SceneManager.LoadSceneAsync("Login");
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        FirestoreManager fsm = FirestoreManager.Instance;
        Dictionary<string, object> addRes = new Dictionary<string, object>();

        fsm.AddUserAssignmentAttempts("assignmentId", "userId", "userScore", "correct", "fail", res =>
        {
            addRes = res;
            fsm.DeleteAssignment("assignmentId", res => {
                Assert.IsNull(addRes);
                });
        });
    }

    // levelscore collection

    [UnityTest]
    public IEnumerator AddLevel()
    {
        yield return SceneManager.LoadSceneAsync("Login");
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        FirestoreManager fsm = FirestoreManager.Instance;

        string addRes = "";
        fsm.AddLevel("levelId", res =>
        {
            addRes = res;
            fsm.DeleteLevel("levelId", res => {
                Assert.IsEmpty(addRes);
            });
        });
    }

    [UnityTest]
    public IEnumerator AddUserLevelAttempts()
    {
        yield return SceneManager.LoadSceneAsync("Login");
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        FirestoreManager fsm = FirestoreManager.Instance;

        Dictionary<string, object> addRes = new Dictionary<string, object>();
        fsm.AddUserLevelAttempts("levelId", "userId", "userScore", "correct", "fail", res =>
        {
            addRes = res;
            fsm.DeleteLevel("levelId", res =>
            {
                Assert.IsNull(addRes);
            });
        });
    }

    // telechats collection
    [UnityTest]
    public IEnumerator SaveChatID()
    {
        yield return SceneManager.LoadSceneAsync("Login");
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        FirestoreManager fsm = FirestoreManager.Instance;

        bool addRes = false;
        fsm.SaveChatID("chatId", res =>
        {
            addRes = res;
            fsm.DeleteChatID("chatId", res =>
            {
                Assert.IsTrue(addRes);
            });
        });
        
    }
}
