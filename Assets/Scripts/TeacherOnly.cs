using UnityEngine;
using UnityEngine.SceneManagement;

public class TeacherOnly : MonoBehaviour
{
    void Start()
    {
        if (FirebaseManager.Instance.isCurrentUserTeacher())
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
