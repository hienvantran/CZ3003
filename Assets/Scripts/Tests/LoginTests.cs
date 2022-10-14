using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
public class LoginTests
{

    // A Test behaves as an ordinary method
    [Test]
    public void TestScriptSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator TestScriptWithEnumeratorPasses()
    {
        
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }

    [UnityTest]
    public IEnumerator LoginTime()
    {
        
        yield return SceneManager.LoadSceneAsync("Login");
        double prevTime = Time.unscaledTimeAsDouble;
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        yield return fm.Login(
            "yoho@mail.com",
            "123456",
            (result) =>
                Assert.GreaterOrEqual((double)5, Time.unscaledTimeAsDouble - prevTime));

    }

    [UnityTest]
    public IEnumerator LoginPositive()
    {
        yield return SceneManager.LoadSceneAsync("Login");
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        yield return fm.Login(
            "yoho@mail.com",
            "123456",
            (result) =>
                Assert.AreEqual(result, "Login Success"));
        
    }

    [UnityTest]
    public IEnumerator LoginNegativePassword()
    {
        yield return SceneManager.LoadSceneAsync("Login");
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        yield return fm.Login(
            "yoho@mail.com",
            "12345",
            (result) =>
                Assert.AreEqual(result, "Wrong Password"));

    }

    [UnityTest]
    public IEnumerator LoginNegativeEmail()
    {
        yield return SceneManager.LoadSceneAsync("Login");
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        yield return fm.Login(
            "yoho",
            "12345",
            (result) =>
                Assert.AreEqual(result, "Invalid Email"));
    }

    [UnityTest]
    public IEnumerator LoginMissingEmail()
    {
        yield return SceneManager.LoadSceneAsync("Login");
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        yield return fm.Login(
            "",
            "12345",
            (result) =>
                Assert.AreEqual(result, "Missing Email"));
    }

    [UnityTest]
    public IEnumerator LoginNegativeAccount()
    {
        yield return SceneManager.LoadSceneAsync("Login");
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        yield return fm.Login(
            "yoho123@mala.com",
            "12345",
            (result) =>
                Assert.AreEqual(result, "Account does not exist"));
    }
}
