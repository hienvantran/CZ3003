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

public class WorldZoneLevel

{
    public string world { get; set; }
    public string level { get; set; }
    public string zone { get; set; }

    public WorldZoneLevel(string world, string level, string zone)
    {
        this.world = world;
        this.zone = zone;
        this.level = level;
    }
}

public class WorldZoneLevelParser
{
    private static Regex parseRegex = new Regex(@"^([^-\s]*?)-([^-\s]*?)-([^-\s]*?)$");
    public static string formatIdFromWorldZoneLevel(string world, string zone, string level)
    {
        return $"{world}-{zone}-{level}";
    }

    // parse world, zone, level from 
    public static WorldZoneLevel parseFromScoreDocumentId(string scoreDocumentId)
    {
        Match match = parseRegex.Match(scoreDocumentId);
        string world = match.Groups[1].Value;
        string zone = match.Groups[2].Value;
        string level = match.Groups[3].Value;

        return new WorldZoneLevel(world, zone, level);
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
    public void getWorldsZonesLevels(Action<Dictionary<string, Dictionary<string, List<string>>>> result)
    {
        db = FirebaseFirestore.DefaultInstance;
        CollectionReference worldsRef = db.Collection(DATABASE);
        worldsRef.GetSnapshotAsync().ContinueWith((task) =>
        {
            Dictionary<string, Dictionary<string, List<string>>> worldsZonesLevels = new Dictionary<string, Dictionary<string, List<string>>>();
            QuerySnapshot worldsZonesLevelsQuerySnapshot = task.Result;
            foreach (DocumentSnapshot scoreDocument in worldsZonesLevelsQuerySnapshot.Documents)
            {
                WorldZoneLevel worldZoneLevel = WorldZoneLevelParser.parseFromScoreDocumentId(scoreDocument.Id);
                string world = worldZoneLevel.world;
                string zone = worldZoneLevel.zone;
                string level = worldZoneLevel.level;

                Dictionary<string, List<string>> zonesLevelsInWorld;
                List<string> levelsInZone;
                if (worldsZonesLevels.TryGetValue(world, out zonesLevelsInWorld))
                {
                    if (zonesLevelsInWorld.TryGetValue(zone, out levelsInZone))
                    {
                        levelsInZone.Add(level);
                    }
                    else
                    {
                        zonesLevelsInWorld.Add(zone, new List<string> { level });
                    }
                }
                else
                {
                    worldsZonesLevels.Add(world, new Dictionary<string, List<string>>
                    {
                        {zone, new List<string>{level}}
                    });
                }
            }
            result?.Invoke(worldsZonesLevels);
        });
    }
}