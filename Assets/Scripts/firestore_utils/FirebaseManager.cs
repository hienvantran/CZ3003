using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using System;

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
    private GameObject loginGroup;
    private TMP_InputField emailLoginField;
    private TMP_InputField passwordLoginField;
    private TMP_Text statusLoginText;

    //Register variables
    private GameObject registerGroup;
    private TMP_InputField nameRegisterField;
    private TMP_InputField emailRegisterField;
    private TMP_InputField roleRegisterField;
    private TMP_InputField passwordRegisterField;
    private TMP_InputField confirmPasswordRegisterField;
    private TMP_Text statusRegisterText;

    private static FirebaseManager m_Instance;

    //Test variables
    public bool instantiated = false;

    public static FirebaseManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new GameObject("FirebaseManager").AddComponent<FirebaseManager>();
            }
            return m_Instance;
        }
    }

    void Awake()
    {
        if (!FirebaseManager.m_Instance)
            FirebaseManager.m_Instance = this;

        if (FirebaseManager.Instance != this)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);

        StartCoroutine(GetObjectReferences());

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
        instantiated = true;
    }

    public FirebaseUser GetUser()
    {
        return User;
    }

    //get object references
    private IEnumerator GetObjectReferences()
    {
        //wait for 1 frame just to make sure GUI and display is drawn and accessible
        yield return null;

        //Login Group
        loginGroup = GameObject.Find("LoginGroup");

        emailLoginField = loginGroup.transform.Find("Email").GetComponent<TMP_InputField>();
        passwordLoginField = loginGroup.transform.Find("Password").GetComponent<TMP_InputField>();
        statusLoginText = loginGroup.transform.Find("Status").GetComponent<TMP_Text>();

        //Register Group
        registerGroup = GameObject.Find("RegisterGroup");
        nameRegisterField = registerGroup.transform.Find("Name").GetComponent<TMP_InputField>();
        emailRegisterField = registerGroup.transform.Find("Email").GetComponent<TMP_InputField>();
        roleRegisterField = registerGroup.transform.Find("Role").GetComponent<TMP_InputField>();
        passwordRegisterField = registerGroup.transform.Find("Password").GetComponent<TMP_InputField>();
        confirmPasswordRegisterField = registerGroup.transform.Find("Confirm Password").GetComponent<TMP_InputField>();
        statusRegisterText = registerGroup.transform.Find("Status").GetComponent<TMP_Text>();

        //set register group to inactive
        registerGroup.SetActive(false);
    }

    //Login Button
    public void LoginButton()
    {
        //if is logged out, user pressed login
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    //clear input fields
    private void ClearFields()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
        statusLoginText.text = "";
        nameRegisterField.text = "";
        emailRegisterField.text = "";
        roleRegisterField.text = "";
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
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, nameRegisterField.text, roleRegisterField.text));
    }

    public void Logout()
    {
        //sign out Firebase auth
        auth.SignOut();
        //logged out
        Debug.LogFormat("User signed out successfully: {0} ({1})", User.DisplayName, User.Email);
        User = null;
        this.ClearFields();
        loginstate = LoginState.OUT;
        Destroy(this.gameObject);
        SceneManager.LoadScene("Login");
    }

    public IEnumerator Login(string _email, string _password, Action<string> result = null)
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
            result?.Invoke(message);
        }
        else
        {
            //logged in
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            this.ClearFields();
            loginstate = LoginState.IN;
            SceneManager.LoadScene("MainMenu");
            result?.Invoke("Login Success");
        }
    }

    private IEnumerator Register(string _email, string _password, string _username, string _role)
    {
        List<string> strings = new List<string>();
        strings.Add("STUDENT");
        strings.Add("TEACHER");
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
        else if(!strings.Contains(_role.ToUpper()))
        {
            //ensure password and confirm password match
            statusRegisterText.text = "Role must be Student or Teacher";
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
                        //add firestore user data
                        FirestoreManager.Instance.addUser(User, _role, res =>
                        {
                            //successful registration, go back to login screen
                            Debug.LogFormat("User Registered: {0} ({1})", res["Name"], res["UID"]);
                            statusRegisterText.text = "";
                            statusLoginText.text = "Account Created";
                            this.BackButton();
                        });
                    }
                }
            }
        }
    }
}
