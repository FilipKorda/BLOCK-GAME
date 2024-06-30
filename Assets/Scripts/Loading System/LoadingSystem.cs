using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingSystem : MonoBehaviour
{
    public Image fadeImage;
    public Canvas canvas;
    public float fadeDuration = 1f;
    private LevelConector levelConector;

    public void LoadStartScene()
    {
        StartCoroutine(LoadStartSceneAsync());
        canvas.sortingOrder = 2;
    }

    private IEnumerator LoadStartSceneAsync()
    {
        yield return StartCoroutine(FadeToBlack());

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("TestScene", LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SceneManager.UnloadSceneAsync("MainMenu");

        yield return StartCoroutine(FadeToClear());

        StartCoroutine(LoadLoadingScreenAndFindSystem());
    }

    public void LoadNextLexel()
    {
        StartCoroutine(LoadNextLevel());
    }

    private IEnumerator LoadNextLevel()
    {
        yield return StartCoroutine(FadeToBlack());

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelConector.nextSceneLevelInfo.sceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SceneManager.UnloadSceneAsync(levelConector.nextSceneLevelInfo.thisSceneName);

        yield return StartCoroutine(FadeToClear());

        StartCoroutine(LoadLoadingScreenAndFindSystem());
    }

    private IEnumerator LoadLoadingScreenAndFindSystem()
    {
        var levelConectorObject = FindObjectOfType<LevelConector>();
        if (levelConectorObject != null)
        {
            levelConector = levelConectorObject;
        }
        else
        {
            Debug.LogError("LoadingSystem not found in LevelConector");
        }
        yield return null;
    }

    public void ResetThisLevel()
    {
        StartCoroutine(ResetCurretLevel());
    }

    private IEnumerator ResetCurretLevel()
    {
        yield return StartCoroutine(FadeToBlack());

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelConector.nextSceneLevelInfo.thisBuildIndex, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SceneManager.UnloadSceneAsync(levelConector.nextSceneLevelInfo.thisSceneName);      

        yield return StartCoroutine(FadeToClear());

        StartCoroutine(LoadLoadingScreenAndFindSystem());
    }

    private IEnumerator FadeToBlack()
    {
        yield return new WaitForSeconds(0.5f);
        if (fadeImage != null)
        {
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
                fadeImage.color = new Color(0f, 0f, 0f, alpha);
                yield return null;
            }
        }
    }

    private IEnumerator FadeToClear()
    {
        if (fadeImage != null)
        {
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
                fadeImage.color = new Color(0f, 0f, 0f, alpha);
                yield return null;
            }
        }
    }
}
