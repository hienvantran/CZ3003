using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public class WorldLevel

{
    public string world { get; set; }
    public string level { get; set; }

    public WorldLevel(string world, string level)
    {
        this.world = world;
        this.level = level;
    }
}

public class WorldLevelParser
{
    private static Regex parseRegex = new Regex(@"^([^-\s]*?)-([^-\s]*?)$");
    public static string formatIdFromWorldLevel(string world, string level)
    {
        return $"{world}-{level}";
    }

    // parse world, zone, level from 
    public static WorldLevel parseFromScoreDocumentId(string scoreDocumentId)
    {
        Match match = parseRegex.Match(scoreDocumentId);
        string world = match.Groups[1].Value;
        string level = match.Groups[1].Value;

        return new WorldLevel(world, level);
    }

}


public class LeaderboardDatabaseManager : MonoBehaviour
{
    public DependencyStatus dependencyStatus;
    protected FirebaseFirestore db;

    private string DATABASE = "defaultlevelscore2";

    void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are available Initialize Firebase
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
        db = FirebaseFirestore.DefaultInstance;
    }

    //get worlds and list of zones in world
    public void getWorldsLevels(Action<Dictionary<string, List<string>>> result)
    {
        db = FirebaseFirestore.DefaultInstance;
        CollectionReference worldsRef = db.Collection(DATABASE);
        worldsRef.GetSnapshotAsync().ContinueWith((task) =>
        {
            Dictionary<string, List<string>> worldsLevels = new Dictionary<string, List<string>>();
            QuerySnapshot worldsLevelsQuerySnapshot = task.Result;
            foreach (DocumentSnapshot scoreDocument in worldsLevelsQuerySnapshot.Documents)
            {
                WorldLevel worldLevel = WorldLevelParser.parseFromScoreDocumentId(scoreDocument.Id);
                string world = worldLevel.world;
                string level = worldLevel.level;

                List<string> levelsInWorld;
                if (worldsLevels.TryGetValue(world, out levelsInWorld))
                {

                    levelsInWorld.Add(level);
                }
                else
                {
                    worldsLevels.Add(world, new List<string> { level });
                }
            }
            result?.Invoke(worldsLevels);
        });
    }
}