using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class NetworkTests
{
    [UnityTest]
    public IEnumerator GetUserInfo()
    {
        yield return SceneManager.LoadSceneAsync("Login");
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        FirestoreManager fsm = FirestoreManager.Instance;
        yield return fm.Login(
            "yoho@mail.com",
            "123456");

        yield return fsm.getUserWorldProgress(res =>
        {
            Assert.IsNotNull(res);
        });
    }

    [UnityTest]
    public IEnumerator GetAssignmentQnStringById()
    {
        yield return SceneManager.LoadSceneAsync("Login");
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        FirestoreManager fsm = FirestoreManager.Instance;

        yield return fsm.getAssignmentQnsStrbyID("2eybpX", res =>
        {
            //Assert.AreEqual("08!2A2!1A3!1A2!1A3!1A3!3A1!2A2!1A3", res);
            Assert.IsNotNull(res);
        });
    }

    [UnityTest]
    public IEnumerator GetAssignmentAttemptsById()
    {
        yield return SceneManager.LoadSceneAsync("Login");
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        FirestoreManager fsm = FirestoreManager.Instance;

        yield return fsm.getAssignmentAttemptsbyID("aBcDeF", res =>
        {
            Assert.IsNotEmpty(res);
        });
    }

    [UnityTest]
    public IEnumerator GetEmptyAssignmentQnStringById()
    {
        yield return SceneManager.LoadSceneAsync("Login");
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        FirestoreManager fsm = FirestoreManager.Instance;

        yield return fsm.getAssignmentQnsStrbyID("emptycase", res =>
        {
            //Assert.AreEqual("08!2A2!1A3!1A2!1A3!1A3!3A1!2A2!1A3", res);
            Assert.IsNull(res);
        });
    }

    [UnityTest]
    public IEnumerator GetEmptyAssignmentAttemptsById()
    {
        yield return SceneManager.LoadSceneAsync("Login");
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        FirestoreManager fsm = FirestoreManager.Instance;

        yield return fsm.getAssignmentAttemptsbyID("emptycase", res =>
        {
            Assert.IsEmpty(res);
        });
    }

    [UnityTest]
    public IEnumerator GetSpecificUserLevelAttempt()
    {
        yield return SceneManager.LoadSceneAsync("Login");
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        FirestoreManager fsm = FirestoreManager.Instance;
        yield return fm.Login(
            "yoho@mail.com",
            "123456");

        yield return fsm.getSpecificUserLevelAttempt("add-1", res =>
        {
            Assert.IsNotNull(res);
        });
    }
}
