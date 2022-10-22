using System;
using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public string currentRole;

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
    [SerializeField] private TMP_Dropdown roleRegisterDropdown;
    private TMP_InputField accessCode;
    private TMP_InputField passwordRegisterField;
    private TMP_InputField confirmPasswordRegisterField;
    private TMP_Text statusRegisterText;

    //Forgot variables
    public GameObject forgotGroup;
    public TMP_InputField emailForgotField;
    public TMP_Text statusForgotText;


    private static FirebaseManager m_Instance;

    //Test variables
    public bool instantiated = false;
    public string access = "123456";

    public Regex emailRegex = new Regex(@"^[a-zA-Z0-9]+@[a-z]+\.com");
    

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
        roleRegisterDropdown = registerGroup.transform.Find("Role").GetComponent<TMP_Dropdown>();
        accessCode = registerGroup.transform.Find("AccessCode").GetComponent<TMP_InputField>();
        passwordRegisterField = registerGroup.transform.Find("Password").GetComponent<TMP_InputField>();
        confirmPasswordRegisterField = registerGroup.transform.Find("Confirm Password").GetComponent<TMP_InputField>();
        statusRegisterText = registerGroup.transform.Find("Status").GetComponent<TMP_Text>();

        //Forgot Group
        forgotGroup = GameObject.Find("ForgotGroup");
        emailForgotField = forgotGroup.transform.Find("ForgotEmail").GetComponent<TMP_InputField>();
        statusForgotText = forgotGroup.transform.Find("Status").GetComponent<TMP_Text>();

        //set register group to inactive
        registerGroup.SetActive(false);
        //set forgot group to inactive
        forgotGroup.SetActive(false);
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
        roleRegisterDropdown.value = 0;
        accessCode.text = "";
        accessCode.gameObject.SetActive(false);
        passwordRegisterField.text = "";
        confirmPasswordRegisterField.text = "";
        statusRegisterText.text = "";
        emailForgotField.text = "";
        statusForgotText.text = "";
    }

    //Register button (from login screen)
    public void RegisterButton()
    {
        loginGroup.SetActive(false);
        registerGroup.SetActive(true);
        this.ClearFields();
    }

    //Forgot Pass button (from login screen)
    public void ForgotButton()
    {
        loginGroup.SetActive(false);
        forgotGroup.SetActive(true);
        this.ClearFields();
    }

    //Back button (from register and forgot screens)
    public void BackButton()
    {
        loginGroup.SetActive(true);
        registerGroup.SetActive(false);
        forgotGroup.SetActive(false);
        this.ClearFields();
    }

    //Perform Registration with Firebase
    public void DoRegisterButton()
    {
        StartCoroutine(Register());
    }

    //Perform send forgot password email
    public void DoForgotPassword()
    {
        StartCoroutine(ForgotPassword());
    }

    public void Logout()
    {
        //sign out Firebase auth
        Debug.LogFormat("User signed out successfully: {0} ({1})", User.DisplayName, User.Email);
        auth.SignOut();
        User = null;
        loginstate = LoginState.OUT;
        currentRole = "";
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
            var GetUserRoleTask = FirestoreManager.Instance.GetUserRolebyID(User.UserId,
                res =>
                {
                    Debug.LogFormat("GetUserRoleTask done");
                    currentRole = res;
                });
            yield return new WaitUntil(predicate: () => GetUserRoleTask.IsCompleted);
            Debug.LogFormat("GetUserRoleTask done 2");
            SceneManager.LoadScene("MainMenu");
            result?.Invoke("Login Success");
        }
        Debug.LogFormat("GetUserRoleTask done 3");
    }

    private IEnumerator Register()
    {
        string _email = emailRegisterField.text;
        string _password = passwordRegisterField.text;
        string _username = nameRegisterField.text;
        string _role = roleRegisterDropdown.options[roleRegisterDropdown.value].text;
        string _confirmed_password = confirmPasswordRegisterField.text;

        ShowRegisterStatus("");

        //Disable access code input field if role is switched to Student
        if (!IsRoleTeacher(_role))
        {
            accessCode.gameObject.SetActive(false);
        }

        //ensure username is filled
        if (IsEmpty(_username))
        {
            ShowRegisterStatus("Username cannot be empty");
            yield break;
        }

        //ensure email is filled
        if (IsEmpty(_email))
        {
            ShowRegisterStatus("Email is missing");
            yield break;
        }

        //ensure email is valid
        if (!IsValidEmail(_email))
        {
            ShowRegisterStatus("Email is invalid");
            yield break;
        }

        //ensure password is filled
        if (IsEmpty(_password))
        {
            ShowRegisterStatus("Password is missing");
            yield break;
        }

        //ensure password and confirm password match
        if (!IsPasswordSameAsConfirmedPassword(_password, _confirmed_password))
        {
            ShowRegisterStatus("Passwords Do Not Match");
            yield break;
        }

        if (IsRoleTeacher(_role))
        {
            if (accessCode.text == "")
            {
                // ensure teacher account registration need access code
                accessCode.gameObject.SetActive(IsRoleTeacher(_role));
                ShowRegisterStatus("Teacher must provide access code");
                yield break;
            }
            else if (accessCode.text != access)
            {
                // ensure access code is 123456
                accessCode.gameObject.SetActive(IsRoleTeacher(_role));
                ShowRegisterStatus("Access code is wrong");
                yield break;
            }
        } 

        bool isDuplicatedUsername = false;

        var CheckDuplicatedUsernameTask = FirestoreManager.Instance.IsDuplicatedUsername(_username, res =>
        {
            //successful registration, go back to login screen
            if (res)
            {
                isDuplicatedUsername = res;
            }
        });
        yield return new WaitUntil(predicate: () => CheckDuplicatedUsernameTask.IsCompleted);

        if (isDuplicatedUsername)
        {
            ShowRegisterStatus("Username must be unique");
            yield break;
        }
        

        //Firebase create user with email & pass
        var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
        //wait
        yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

        if (RegisterTask.Exception != null)
        {
            HandleRegisterTaskException(RegisterTask.Exception);
            yield break;
        }

        //get result of created user
        User = RegisterTask.Result;

        if (User == null) yield break;

        //Firebase auth update user profile
        UserProfile profile = new UserProfile { DisplayName = _username };
        var ProfileTask = User.UpdateUserProfileAsync(profile);
        //Wait
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);
        
        if (ProfileTask.Exception != null)
        {
            HandleProfileTaskException(ProfileTask.Exception);
            var DeleteUserTask = User.DeleteAsync();
            yield return new WaitUntil(predicate: () => DeleteUserTask.IsCompleted);
            User = null;
            yield break;
        }

        //add firestore user data
        var AddUserFirestoreTask = FirestoreManager.Instance.AddUser(User, _role,
        res =>
        {
            //successful registration, go back to login screen
            Debug.LogFormat("User Registered: {0} ({1})", res["Name"], res["UID"]);
            ShowRegisterStatus("");
            statusLoginText.text = "Account Created";
            this.BackButton();
        });

        yield return new WaitUntil(predicate: () => AddUserFirestoreTask.IsCompleted);
    }

    //Send Forgot Password
    private IEnumerator ForgotPassword()
    {
        string emailAddress = emailForgotField.text;

        //validattion
        //ensure email is filled
        if (IsEmpty(emailAddress))
        {
            statusForgotText.text = "Email is missing";
            yield break;
        }

        //ensure email is valid
        if (!IsValidEmail(emailAddress))
        {
            statusForgotText.text = "Email is invalid";
            yield break;
        }

        var ForgotTask = auth.SendPasswordResetEmailAsync(emailAddress);

        yield return new WaitUntil(predicate: () => ForgotTask.IsCompleted);

        if (ForgotTask.Exception != null)
        {
            Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + ForgotTask.Exception);
            HandleForgotTaskException(ForgotTask.Exception);
            yield break;
        }


        statusForgotText.text = "Password reset email sent to " + emailAddress;
    }

    //handle forgot task exception
    private void HandleForgotTaskException(AggregateException exception)
    {
        //handle errors
        FirebaseException firebaseEx = exception.GetBaseException() as FirebaseException;
        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
        Debug.Log(errorCode.ToString());
        switch (errorCode)
        {
            case AuthError.MissingEmail:
                statusForgotText.text = "Missing Email";
                break;
            case AuthError.InvalidEmail:
                statusForgotText.text = "Invalid email";
                break;
            case AuthError.UserNotFound:
                statusForgotText.text = "Email is not registered";
                break;
            default:
                statusForgotText.text = errorCode.ToString();
                break;
        }
    }

    private void ShowRegisterStatus(string _status)
    {
        statusRegisterText.text = _status;
    }

    private bool IsEmpty(string _str)
    {
        return _str == "";
    }

    /// <summary>
    /// Checks if input is a valid email string.
    /// </summary>
    /// <param name="_email">Email string</param>
    /// <returns></returns>
    private bool IsValidEmail(string _email)
    {
        return emailRegex.IsMatch(_email);
    }

    private bool IsPasswordSameAsConfirmedPassword(string _password, string _confirmed_password)
    {
        return _password == _confirmed_password;
    }

    private bool IsRoleTeacher(string _role)
    {
        bool shown = false;
        if (_role == "Teacher") {
            shown = true;
        }
        return shown;
    }

    private void HandleRegisterTaskException(AggregateException exception)
    {
        //handle errors
        Debug.LogWarning(message: $"Failed to register task with {exception}");
        FirebaseException firebaseEx = exception.GetBaseException() as FirebaseException;
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
            case AuthError.InvalidEmail:
                message = "Invalid email";
                break;
        }
        ShowRegisterStatus(message);
    }

    private void HandleProfileTaskException(AggregateException exception)
    {
        Debug.LogWarning(message: $"Failed to register task with {exception}");
        FirebaseException firebaseEx = exception.GetBaseException() as FirebaseException;
        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
        ShowRegisterStatus("Username Set Failed!");
    }

    public bool IsCurrentUserTeacher()
    {
        if (loginstate != LoginState.IN)
        {
            return false;
        }
        return (currentRole == "Teacher");
    }
}
