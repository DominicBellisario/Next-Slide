using System;
using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Helper : MonoBehaviour
{
    public static event Action LoadingScene;

    public static IEnumerator DoThisAfterDelay(Action onReset, float delay)
    {
        yield return new WaitForSeconds(delay);
        onReset?.Invoke();
    }

    public static void LoadScene(string sceneName)
    {
        LoadingScene?.Invoke();
        SceneManager.LoadScene(sceneName);
    }
    
    public static void ReloadScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }
}
