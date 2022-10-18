using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;


[FirestoreData]
public class User
{
        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Email { get; set; }

        [FirestoreProperty]
        public string Role { get; set; }

        [FirestoreProperty]
        public string UID { get; set; }

        [FirestoreProperty]
        public int AddProgress { get; set; }

        [FirestoreProperty]
        public int SubProgress { get; set; }

        [FirestoreProperty]
        public int MulProgress { get; set; }

        [FirestoreProperty]
        public int DivProgress { get; set; }
}

[FirestoreData]
public class Assignment
{
        /* 
        Read operations on Firestore are always shallow, so it is indeed expected that 
        a subcollection  is not automatically read too. 
        -> perform a separate read operation on the subcollection to get those documentations, after reading the parent document. 
        */
        [FirestoreProperty]
        public string qnsString { get; set; }
}

[FirestoreData]
public class UserAttempts 
{
        [FirestoreProperty]
        public string score { get; set;}
}

[FirestoreData]
public class LevelScoreUserAttempts 
{
        [FirestoreProperty]
        public string score { get; set;}

        [FirestoreProperty]
        public string correct { get; set;}

        [FirestoreProperty]
        public string fail { get; set;}
}

public class FirestoreManager : MonoBehaviour
{
    private static FirestoreManager m_Instance;

    public static FirestoreManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new GameObject("FirestoreManager").AddComponent<FirestoreManager>();
            }
            return m_Instance;
        }
    }

    //instance
    private void Awake()
    {
        if (!FirestoreManager.m_Instance)
            FirestoreManager.m_Instance = this;

        if (FirestoreManager.m_Instance != this)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    //add user details to firestore
    //* add functions don't actually need the calllback action but good to have incase you want to notify when done or smth
    public void addUser (FirebaseUser User, string role, Action<Dictionary<string, object>> result) {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference users = db.Collection("users").Document(User.UserId);
        
        // default value for role is Student
        string roleOfUser = "Student";
        if (role.ToUpper() == "STUDENT")
        {
            roleOfUser = "Student";
        } else {
            roleOfUser = "Teacher";
        }
        Dictionary<string, object> user = new Dictionary<string, object>
        {
                { "Name", User.DisplayName },
                { "Email", User.Email },
                { "Role", roleOfUser },
                { "UID", User.UserId },
                { "AddProgress", 0 },
                { "SubProgress", 0 },
                { "MulProgress", 0 },
                { "DivProgress", 0 }

        };
        users.SetAsync(user).ContinueWithOnMainThread(task => {
            Debug.Log("Added data of new user document in the users collection.");
            result?.Invoke(user);
        });
    }

    //update user world progress
    public void updateUserWorldProgress(FirebaseUser User, string field, int val)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference docRef = db.Collection("users").Document(User.UserId);

        docRef.UpdateAsync(field, val).ContinueWithOnMainThread(task => 
        {
            Debug.Log("Updated " + field + " to " + val);
        });
    }

    //get user world progress
    public Task getUserWorldProgress(Action<Dictionary<string, int>> result)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference docRef = db.Collection("users").Document(FirebaseManager.Instance.User.UserId);
        return docRef.GetSnapshotAsync().ContinueWith((task) =>
        {
            var snapshot = task.Result;
            Dictionary<string, int> userProg = new Dictionary<string, int>();
            if (snapshot.Exists)
            {
                User refUser = snapshot.ConvertTo<User>();
                userProg.Add("Add", refUser.AddProgress);
                userProg.Add("Sub", refUser.SubProgress);
                userProg.Add("Mul", refUser.MulProgress);
                userProg.Add("Div", refUser.DivProgress);
                result?.Invoke(userProg);
            }
            else
            {
                Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
                result?.Invoke(null);
            }
            return;
        });
    }

    //add assignment to firestore
    //* add functions don't actually need the calllback action but good to have incase you want to notify when done or smth
    public void addAssignment (string assignmentId, string qnsStr, Action<Dictionary<string, object>> result) {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference assignRef = db.Collection("assignments").Document(assignmentId);

        Dictionary<string, object> questionsStr = new Dictionary<string, object>
        {
            { "qnsString", qnsStr },
        };
        assignRef.SetAsync(questionsStr).ContinueWithOnMainThread(task => {
            Debug.Log("Added assignment and its qnsStr document in the assignment collection.");
            result?.Invoke(questionsStr);
        });
    }

    //get assignment question string by assignment ID/Key
    public Task getAssignmentQnsStrbyID(string assignID, Action<string> result)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference assignRef = db.Collection("assignments").Document(assignID);
        
        return assignRef.GetSnapshotAsync().ContinueWith((task) =>
        {
            var snapshot = task.Result;
            string assignmentQnsStr = "";
            if (snapshot.Exists)
            {
                Assignment assignment = snapshot.ConvertTo<Assignment>();
                assignmentQnsStr = assignment.qnsString;
                result?.Invoke(assignmentQnsStr);
            }
            else
            {
                result?.Invoke(null);
                Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
            }
            return;
        });
    }

    //get user attempts for an assignment ID/Key
    public Task getAssignmentAttemptsbyID(string assignID, Action<List<Dictionary<string, object>>> result)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        
        CollectionReference attemptsRef = db.Collection("assignments").Document(assignID).Collection("userattempts");
        return attemptsRef.GetSnapshotAsync().ContinueWith((task) =>
        {
            List<Dictionary<string, object>> userAttempts = new List<Dictionary<string, object>>();
            QuerySnapshot allAttemptsQuerySnapshot = task.Result;
            foreach (DocumentSnapshot attemptSnapshot in allAttemptsQuerySnapshot.Documents)
            {
                UserAttempts attempt = attemptSnapshot.ConvertTo<UserAttempts>();
                Dictionary<string, object> userAttempt = new Dictionary<string, object>
                {
                    { "score", attempt.score },
                    { "uid" , attemptSnapshot.Id}
                };
                userAttempts.Add(userAttempt);
                // Newline to separate entries
                Debug.Log("");
            }
            result?.Invoke(userAttempts);
        
        });
    }

    //add a user attempt for an assignment ID/Key
    //* add functions don't actually need the calllback action but good to have incase you want to notify when done or smth
    public void addUserAssignmentAttempts (string assignmentId, string userId, string userScore, Action<Dictionary<string, object>> result) {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference assignRef = db.Collection("assignments").Document(assignmentId);
        DocumentReference userAttemptsRef = assignRef.Collection("userattempts").Document(userId);
 
        Dictionary<string, object> userAttempt = new Dictionary<string, object>
        {
            { "score", userScore }
        };
        userAttemptsRef.SetAsync(userAttempt).ContinueWithOnMainThread(task => {
            Debug.Log("Added score of new user document in the assignment collection.");
            result?.Invoke(userAttempt);
        });
    }

    //get a specific user's attempt for an assignment ID/Key
    public Task getSpecificUserAssignmentAttempt(string assignmentId, string userId, Action<UserAttempts> result)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference assignRef = db.Collection("assignments").Document(assignmentId);
        DocumentReference userAttemptsRef = assignRef.Collection("userattempts").Document(userId);
        
        return userAttemptsRef.GetSnapshotAsync().ContinueWith((task) =>
        {
            var snapshot = task.Result;
            UserAttempts userAttempt = new UserAttempts();
            if (snapshot.Exists)
            {
                userAttempt = snapshot.ConvertTo<UserAttempts>();
                Debug.Log(String.Format("UID {0} and score {1}:", userId, userAttempt.score)); 
            }
            else
            {
                Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
            }
            result?.Invoke(userAttempt);
        });
    }


    // levelscore collections
    // add levelscore to firestore
    //* add functions don't actually need the calllback action but good to have incase you want to notify when done or smth
    public void addLevel (string levelId) {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference levelRef = db.Collection("levelscore").Document(levelId);
        var empty_object = new Dictionary < string, object > ();
        levelRef.SetAsync(empty_object).ContinueWithOnMainThread(task => {
            Debug.Log("Added new level in the defaultlevelscore collection.");
        });
    }

    //get user attempts for a levelscore ID/Key
    public Task getLevelAttemptsbyID(string levelId, Action<List<Dictionary<string, object>>> result)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        CollectionReference attemptsRef = db.Collection("levelscore").Document(levelId).Collection("userattempts");

        return attemptsRef.GetSnapshotAsync().ContinueWith((task) =>
        {
            List<Dictionary<string, object>> userAttempts = new List<Dictionary<string, object>>();
            QuerySnapshot allAttemptsQuerySnapshot = task.Result;
            foreach (DocumentSnapshot attemptSnapshot in allAttemptsQuerySnapshot.Documents)
            {
                LevelScoreUserAttempts attempt = attemptSnapshot.ConvertTo<LevelScoreUserAttempts>();
                Dictionary<string, object> userAttempt = new Dictionary<string, object>
                {
                    { "score", attempt.score },
                    { "correct", attempt.correct},
                    { "fail", attempt.fail},
                    { "uid" , attemptSnapshot.Id}
                };
                userAttempts.Add(userAttempt);
                // Newline to separate entries
                Debug.Log("");
            }
            result?.Invoke(userAttempts);
        
        });
    }

    //add a user attempt for a levelscore ID/Key
    //* add functions don't actually need the calllback action but good to have incase you want to notify when done or smth
    public void addUserLevelAttempts (string levelId, string userId, string userScore, string correct, string fail, Action<Dictionary<string, object>> result) {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference userAttemptsRef = db.Collection("levelscore").Document(levelId).Collection("userattempts").Document(userId);

        Dictionary<string, object> userAttempt = new Dictionary<string, object>
        {
            { "score", userScore },
            { "correct", correct },
            { "fail", fail}
        };

        userAttemptsRef.SetAsync(userAttempt).ContinueWithOnMainThread(task => {
            result?.Invoke(userAttempt);
        });
    }

    //get a specific user's attempt for a levelscore ID/Key
    public Task getSpecificUserLevelAttempt(string levelId, Action<LevelScoreUserAttempts> result)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference userAttemptsRef = db.Collection("levelscore").Document(levelId).Collection("userattempts").Document(FirebaseManager.Instance.User.UserId);
        
        return userAttemptsRef.GetSnapshotAsync().ContinueWith((task) =>
        {
            var snapshot = task.Result;
            LevelScoreUserAttempts userAttempt = null;
            if (snapshot.Exists)
            {
                userAttempt = snapshot.ConvertTo<LevelScoreUserAttempts>();
            }
            else
            {
                Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
            }
            result?.Invoke(userAttempt);
        });
    }
}