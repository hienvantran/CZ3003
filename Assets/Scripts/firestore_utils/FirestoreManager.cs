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

public class FirestoreManager : MonoBehaviour
{
    // public async void getUserbyID(string userID)
    // {
    //     FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
    //     DocumentReference docRef = db.Collection("users").Document(userID);
    //     await docRef.GetSnapshotAsync().ContinueWith((task) =>
    //         {
    //         var snapshot = task.Result;
    //         if (snapshot.Exists)
    //         {
    //             Debug.Log(String.Format("Document data for {0} document:", snapshot.Id));
    //             User user = snapshot.ConvertTo<User>();
    //             Debug.Log(String.Format("Name: {0}", user.Name));
    //         }
    //         else
    //         {
    //             Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
    //         }
    //         });
        
    // }

    public async void addUser (FirebaseUser User) {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference users = db.Collection("users").Document(User.UserId);
        Dictionary<string, object> user = new Dictionary<string, object>
        {
                { "Name", User.DisplayName },
                { "Email", User.Email },
                { "Role", "Student" },
                { "UID", User.UserId }
        };
        await users.SetAsync(user).ContinueWithOnMainThread(task => {
                Debug.Log("Added data of new user document in the users collection.");
        });
    }

}