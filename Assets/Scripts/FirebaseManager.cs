/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;

public class FirebaseManager : MonoBehaviour
{
    public enum LoginState { IN, OUT }

    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;    
    public FirebaseUser User;

    //Login variables
    [Header("Login")]
    public LoginState loginstate = LoginState.OUT;
    public GameObject loginGroup;
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text loginoutButtonText;
    public TMP_Text statusLoginText;

    //Register variables
    [Header("Register")]
    public GameObject registerGroup;
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField confirmPasswordRegisterField;
    public TMP_Text statusRegisterText;

    void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
    }

    //Login & Logout Button
    public void LoginoutButton()
    {
        if (loginstate == LoginState.IN)
        {
            //if is logged in, user pressed logout
            this.Logout();
        }
        else
        {
            //if is logged out, user pressed login
            StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
        }
    }

    //clear input fields
    private void ClearFields()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
        statusLoginText.text = "";
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        confirmPasswordRegisterField.text = "";
        statusRegisterText.text = "";
    }

    //Register button (from login screen)
    public void RegisterButton()
    {
        loginGroup.SetActive(false);
        registerGroup.SetActive(true);
        this.ClearFields();
    }

    //Back button (from register screen)
    public void BackButton()
    {
        loginGroup.SetActive(true);
        registerGroup.SetActive(false);
        this.ClearFields();
    }

    //Perform Registration with Firebase
    public void DoRegisterButton()
    {
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    private void Logout()
    {
        //sign out Firebase auth
        auth.SignOut();
        //logged out
        Debug.LogFormat("User signed out successfully: {0} ({1})", User.DisplayName, User.Email);
        User = null;
        this.ClearFields();
        statusLoginText.text = "Logged Out";
        loginoutButtonText.text = "Login";
        loginstate = LoginState.OUT;
    }

    private IEnumerator Login(string _email, string _password)
    {
        //Firebase auth signin with email & pass
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //wait
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //handle errors
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            statusLoginText.text = message;
        }
        else
        {
            //logged in
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            this.ClearFields();
            statusLoginText.text = "Logged In";
            loginoutButtonText.text = "Logout";
            loginstate = LoginState.IN;
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //ensure username is filled
            statusRegisterText.text = "Username cannot be empty";
        }
        else if(passwordRegisterField.text != confirmPasswordRegisterField.text)
        {
            //ensure password and confirm password match
            statusRegisterText.text = "Passwords Do Not Match";
        }
        else 
        {
            //Firebase create user with email & pass
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //wait
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //handle errors
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                statusRegisterText.text = message;
            }
            else
            {
                //get result of created user
                User = RegisterTask.Result;

                if (User != null)
                {
                    //Firebase auth update user profile
                    UserProfile profile = new UserProfile{DisplayName = _username};
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        statusRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //successful registration, go back to login screen
                        statusRegisterText.text = "";
                        statusLoginText.text = "Account Created";
                        this.BackButton();
                    }
                }
            }
        }
    }
}
*/