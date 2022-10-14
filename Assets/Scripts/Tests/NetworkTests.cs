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

        int addProgress = 0, subProgress = 0, mulProgress = 0, divProgress = 0;
        bool taskComplete = false;
        fsm.getUserWorldProgress(res =>
        {
            addProgress = res["Add"];
            subProgress = res["Sub"];
            mulProgress = res["Mul"];
            divProgress = res["Div"];
            taskComplete = true;
            Debug.Log(res);
        });
        yield return new WaitUntil(() => taskComplete);
        Debug.Log(addProgress);
        bool isEqual = (addProgress == 3 && subProgress == 1 && divProgress == 2 && mulProgress == 1);

        Assert.AreEqual(true, isEqual);

    }


}
