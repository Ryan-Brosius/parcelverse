using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKeyDown)
        {
            LoadNextSceneInBuildSettings();
        }
    }
    
    void LoadNextSceneInBuildSettings()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
