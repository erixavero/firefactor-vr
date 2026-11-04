using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRestartManager : MonoBehaviour
{
    void Update()
    {
        // Check if the 'R' key is pressed down
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartCurrentScene();
        }
    }

    public void RestartCurrentScene()
    {
        // Get the currently active scene and reload it
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}