using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.PerformanceTesting;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.Profiling;

public class PerformanceTests
{
    [UnityTest, Performance]
    public IEnumerator GetUserWorldProgress()
    {
        yield return SceneManager.LoadSceneAsync("Login");
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        FirestoreManager fsm = FirestoreManager.Instance;
        yield return fm.Login(
            "yoho@mail.com",
            "123456");

        Task getTask;
        //mreasure scope
        using (Measure.Scope(new SampleGroup("worldprogress")))
        {
            getTask = fsm.GetUserWorldProgress(res =>
            {
            });
        }

        yield return new WaitUntil(predicate: () => getTask.IsCompleted);

        //measure memory
        MeasureMemory();

        //get frames
        yield return Measure.Frames().Run();
    }

    [UnityTest, Performance]
    public IEnumerator MultipleReadWrites()
    {
        yield return SceneManager.LoadSceneAsync("Login");
        FirebaseManager fm = FirebaseManager.Instance;
        yield return new WaitUntil(() => fm.instantiated);
        FirestoreManager fsm = FirestoreManager.Instance;

        //measure scope
        using (Measure.Scope(new SampleGroup("MultiReadWrite")))
        {
            int numDocsToAdd = 10;
            //add docs
            int addCount = 0;
            for (int i = 0; i < numDocsToAdd; i++)
            {
                fsm.AddAssignment(("ass"+(i+1).ToString()), "qnsStr", "user", res => { addCount++; });
            }

            yield return new WaitWhile(predicate: () => addCount < 10);

            //get some data
            Task getAss = fsm.GetAssignments();

            //delete docs
            int delCount = 0;
            for (int i = 0; i < numDocsToAdd; i++)
            {
                fsm.DeleteAssignment("ass" + (i + 1).ToString(), res => { delCount++; });
            }

            //wait for the tasks
            yield return new WaitWhile(predicate: () => delCount < 10);
            yield return new WaitUntil(predicate: () => getAss.IsCompleted);
        }

        //measure memory
        MeasureMemory();

        //measure frames
        yield return Measure.Frames().Run();
    }

    //measure memory use
    public void MeasureMemory()
    {
        var allocated = new SampleGroup("TotalAllocatedMemory", SampleUnit.Megabyte);
        var reserved = new SampleGroup("TotalReservedMemory", SampleUnit.Megabyte);
        Measure.Custom(allocated, Profiler.GetTotalAllocatedMemoryLong() / 1048576f);
        Measure.Custom(reserved, Profiler.GetTotalReservedMemoryLong() / 1048576f);
    }
}
