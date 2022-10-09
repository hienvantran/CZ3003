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
        public string UID { get; set;}

        [FirestoreProperty]
        public string score { get; set;}
}

public class FirestoreManager : MonoBehaviour
{
    //add user details to firestore
    //* add functions don't actually need the calllback action but good to have incase you want to notify when done or smth
    public void addUser (FirebaseUser User, Action<Dictionary<string, object>> result) {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference users = db.Collection("users").Document(User.UserId);
        Dictionary<string, object> user = new Dictionary<string, object>
        {
                { "Name", User.DisplayName },
                { "Email", User.Email },
                { "Role", "Student" },
                { "UID", User.UserId }
        };
        users.SetAsync(user).ContinueWithOnMainThread(task => {
            Debug.Log("Added data of new user document in the users collection.");
            result?.Invoke(user);
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
    public void getAssignmentQnsStrbyID(string assignID, Action<string> result)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference assignRef = db.Collection("assignments").Document(assignID);
        
        assignRef.GetSnapshotAsync().ContinueWith((task) =>
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
                Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
            }
            return;
        });
    }

    //get user attempts for an assignment ID/Key
    public void getAssignmentAttemptsbyID(string assignID, Action<List<UserAttempts>> result)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        
        CollectionReference attemptsRef = db.Collection("assignments").Document(assignID).Collection("userAttempts");
        attemptsRef.GetSnapshotAsync().ContinueWith((task) =>
        {
            List<UserAttempts> userAttempts = new List<UserAttempts>();
            QuerySnapshot allAttemptsQuerySnapshot = task.Result;
            foreach (DocumentSnapshot attemptSnapshot in allAttemptsQuerySnapshot.Documents)
            {
                UserAttempts attempt = attemptSnapshot.ConvertTo<UserAttempts>();
                userAttempts.Add(attempt);
                // Newline to separate entries
                Debug.Log("");
            }
            result?.Invoke(userAttempts);
        
        });
    }

    //add a user attempt for an assignment ID/Key
    //* add functions don't actually need the calllback action but good to have incase you want to notify when done or smth
    public void addUserAttempts (string assignmentId, string userId, string userScore, Action<Dictionary<string, object>> result) {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference assignRef = db.Collection("assignments").Document(assignmentId);
        DocumentReference userAttemptsRef = assignRef.Collection("userAttempts").Document(userId);
 
        Dictionary<string, object> userAttempt = new Dictionary<string, object>
        {
                { "score", userScore },
                { "UID", userId }
        };
        userAttemptsRef.SetAsync(userAttempt).ContinueWithOnMainThread(task => {
            Debug.Log("Added score of new user document in the assignment collection.");
            result?.Invoke(userAttempt);
        });
    }

    //get a specific user's attempt for an assignment ID/Key
    public void getSpecificUserAttempt(string assignmentId, string userId, Action<UserAttempts> result)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference assignRef = db.Collection("assignments").Document(assignmentId);
        DocumentReference userAttemptsRef = assignRef.Collection("userAttempts").Document(userId);
        
        userAttemptsRef.GetSnapshotAsync().ContinueWith((task) =>
        {
            var snapshot = task.Result;
            UserAttempts userAttempt = new UserAttempts();
            if (snapshot.Exists)
            {
                    userAttempt = snapshot.ConvertTo<UserAttempts>();
                    Debug.Log(String.Format("UID {0} and score {1}:", userAttempt.UID, userAttempt.score)); 
            }
            else
            {
                    Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
            }
            result?.Invoke(userAttempt);
        });
    }
}