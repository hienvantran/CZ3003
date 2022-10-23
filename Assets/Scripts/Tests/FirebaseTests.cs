using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
// using Firebase;
// using Firebase.Auth;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class FirebaseTests
{
    // public FirebaseAuth auth;

    // users collection

    // [UnityTest]
    // public IEnumerator AddUser()
    // {
        
    //     //Firebase create user with email & pass
    //     var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync("test@gmail.com", "test123");
    //     //wait
    //     yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

    //     if (RegisterTask.Exception != null)
    //     {
    //         HandleRegisterTaskException(RegisterTask.Exception);
    //         yield break;
    //     }

    //     //get result of created user
    //     User = RegisterTask.Result;

    //     if (User == null) yield break;

    //     //Firebase auth update user profile
    //     UserProfile profile = new UserProfile { DisplayName = _username };
    //     var ProfileTask = User.UpdateUserProfileAsync(profile);
    //     //Wait
    //     yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);
        
    //     if (ProfileTask.Exception != null)
    //     {
    //         HandleProfileTaskException(ProfileTask.Exception);
    //         var DeleteUserTask = User.DeleteAsync();
    //         yield return new WaitUntil(predicate: () => DeleteUserTask.IsCompleted);
    //         User = null;
    //         yield break;
    //     }

    //     FirestoreManager fsm = FirestoreManager.Instance;
        
    //     yield return fsm.AddUser(User, "Student", res =>
    //     {
    //         Assert.IsNotNull(res);
    //         var DeleteUserTask = User.DeleteAsync();
    //         yield return new WaitUntil(predicate: () => DeleteUserTask.IsCompleted);

    //     });
    // }

    // assignments collection

    [UnityTest]
    public IEnumerator AddAssignment()
    {
        yield return SceneManager.LoadSceneAsync("Login");
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        FirestoreManager fsm = FirestoreManager.Instance;

        fsm.AddAssignment("assignmentId", "qnsStr", res =>
        {
            Assert.IsNull(res);
        });
    }

    [UnityTest]
    public IEnumerator AddUserAssignmentAttempts()
    {
        yield return SceneManager.LoadSceneAsync("Login");
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        FirestoreManager fsm = FirestoreManager.Instance;

        fsm.AddUserAssignmentAttempts("assignmentId", "userId", "userScore", "correct", "fail", res =>
        {
            Assert.IsNull(res);
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

        fsm.AddLevel("levelId");
    }

    [UnityTest]
    public IEnumerator AddUserLevelAttempts()
    {
        yield return SceneManager.LoadSceneAsync("Login");
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        FirestoreManager fsm = FirestoreManager.Instance;

        fsm.AddUserLevelAttempts("levelId", "userId", "userScore", "correct", "fail", res =>
        {
            Assert.IsNull(res);
        });
    }
}
