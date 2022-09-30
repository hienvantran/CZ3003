using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    public void BackToScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
