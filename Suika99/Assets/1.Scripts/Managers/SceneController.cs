using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void SceneChange(int sceneIndex)
        => SceneManager.LoadSceneAsync(sceneIndex);

    public void SceneChange(string sceneName)
        => SceneManager.LoadSceneAsync(sceneName);

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
