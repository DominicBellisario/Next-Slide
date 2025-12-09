using System;
using System.Collections;
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

    public void LoadSceneNextFrame(string sceneName) { StartCoroutine(LoadSceneLate(sceneName)); }
    private IEnumerator LoadSceneLate(string sceneName)
    {
        yield return new WaitForEndOfFrame();
        LoadScene(sceneName);
    }

    public static void ReloadScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void LoadNextSlide()
    {
        // get the current slide and level number
        int currentSlideNumber = 10 * int.Parse(SceneManager.GetActiveScene().name[^2].ToString()) + int.Parse(SceneManager.GetActiveScene().name[^1].ToString());
        string currentLevelString = SceneManager.GetActiveScene().name[^6].ToString() + SceneManager.GetActiveScene().name[^5].ToString();
        string nextSlideString;
        // format correctly
        if (currentSlideNumber < 9) { nextSlideString = "0" + (currentSlideNumber + 1); }
        else { nextSlideString = "" + (currentSlideNumber + 1); }
        // put together scene name
        LoadScene("L" + currentLevelString + "_S" + nextSlideString);
    }
}
