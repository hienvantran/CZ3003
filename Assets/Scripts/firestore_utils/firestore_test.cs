// string assignmentId = "assign2";
// string qnsStr = "123456";
// string uid = User.UserId;
// string score = "153";
// Debug.Log("---Add assignment---");
// firestoreManager.addAssignment(assignmentId, qnsStr);

// Debug.Log("---Get assignment---");
// Debug.Log("---Get assignment qns---");
// var attemptsTask = firestoreManager.getAssignmentAttemptsbyID(assignmentId);
// //wait
// yield return new WaitUntil(predicate: () => attemptsTask.IsCompleted);
// if (attemptsTask.Exception != null){
//     Debug.LogWarning(message: $"Failed to register task with {attemptsTask.Exception}");
// } else {
//     List<UserAttempts> attempts = attemptsTask.Result;
//     Debug.LogFormat("User {0} with score ({1})", attempts[0].UID, attempts[0].score);
// }

// Debug.Log("---Get assignment userAttempts array---");
// var qnsTask = firestoreManager.getAssignmentQnsStrbyID(assignmentId);
// //wait
// yield return new WaitUntil(predicate: () => qnsTask.IsCompleted);
// if (qnsTask.Exception != null){
//     Debug.LogWarning(message: $"Failed to register task with {qnsTask.Exception}");
// } else {
//     string qns = qnsTask.Result;
//     Debug.Log(qns);
// }

// Debug.Log("---Add assignment userAttempt---");
// firestoreManager.addUserAttempts(assignmentId, uid, score);

// Debug.Log("---Get assignment userAttempt of specific user---");
// var specificUserTask = firestoreManager.getSpecificUserAttempt(assignmentId, uid);;
// //wait
// yield return new WaitUntil(predicate: () => specificUserTask.IsCompleted);
// if (specificUserTask.Exception != null){
//     Debug.LogWarning(message: $"Failed to register task with {specificUserTask.Exception}");
// } else {
//     UserAttempts userAttempt = specificUserTask.Result;
//     Debug.LogFormat("User {0} with score {1}", userAttempt.UID, userAttempt.score);
// }






// test level
            // string levelId = "test1-1";
            // string userId = User.UserId;
            // string userScore = "16";
            // firestoreManager.addLevel(levelId);
            // firestoreManager.addUserLevelAttempts(levelId, userId, userScore, result =>
            //     {
            //         Debug.LogFormat("User {0} attempt score is: {1}", userId, result["score"]);
            //     });
            
            // firestoreManager.getLevelAttemptsbyID(levelId, result =>
            //     {
            //         foreach (var attempt in result)
            //         {
            //             Debug.Log("Test get all attempts");
            //             Debug.LogFormat("Score of {0} is: {1}", attempt["uid"], attempt["score"]);
            //             // Newline to separate entries
            //             Debug.Log("");
            //         }
            //     });
            
            // firestoreManager.getSpecificUserLevelAttempt(levelId, userId, result =>
            //     {
            //         Debug.Log("Test get specific attempt");
            //         Debug.LogFormat("User {0} score is: {1}", userId, result.score);
            //     });