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
    [SerializeField] private MovesManager movesManager;
    [SerializeField] private GameObject loadingHandel;
    [SerializeField] private TextMeshProUGUI stageText;
    [Header("===========  Scene To Load  ===========")]
    public SceneData sceneData;
    [Header("===========   Scene To Unload   ===========")]
    public SceneData mainMenuSceneData;

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

    public void LoadStartScene()
    {
        StartCoroutine(LoadStartSceneAsync(sceneData, mainMenuSceneData));
        canvas.sortingOrder = 2;
    }

    private IEnumerator LoadStartSceneAsync(SceneData sceneData, SceneData mainMenuSceneData)
    {   
        yield return StartCoroutine(FadeToBlack());

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneData.sceneIndex, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        float minimumLoadTime = 1f;
        float loadStartTime = Time.time;

        Debug.Log("Rozpoczêto ³adowanie sceny: " + Time.time);

        while ((Time.time - loadStartTime) < minimumLoadTime)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("Zakoñczono ³adowanie sceny: " + Time.time);

        SceneManager.UnloadSceneAsync(mainMenuSceneData.sceneIndex);

        StartCoroutine(LoadLoadingScreenAndFindSystem());

        stageText.gameObject.SetActive(true);

        yield return StartCoroutine(FadeToClear());

    }

    public void LoadNextLexel()
    {
        StartCoroutine(LoadNextLevel(levelConector.sceneData, levelConector.thisSceneData));
    }

    private IEnumerator LoadNextLevel(SceneData sceneData, SceneData thisSceneName)
    {
        yield return StartCoroutine(FadeToBlack());

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneData.sceneIndex, LoadSceneMode.Additive);

        asyncLoad.allowSceneActivation = false;

        float minimumLoadTime = 1f;
        float loadStartTime = Time.time;

        Debug.Log("Rozpoczêto ³adowanie sceny: " + Time.time);

        while ((Time.time - loadStartTime) < minimumLoadTime)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("Zakoñczono ³adowanie sceny: " + Time.time);
        SceneManager.UnloadSceneAsync(thisSceneName.sceneIndex);

        StartCoroutine(LoadLoadingScreenAndFindSystem());

        yield return StartCoroutine(FadeToClear());
    }

    public void ResetThisLevel()
    {
        StartCoroutine(ResetCurretLevel(levelConector.thisSceneData, levelConector.thisSceneData));
    }

    private IEnumerator ResetCurretLevel(SceneData sceneData, SceneData thisSceneData)
    {
        yield return StartCoroutine(FadeToBlack());

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneData.sceneIndex, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        float minimumLoadTime = 1f;
        float loadStartTime = Time.time;

        Debug.Log("Rozpoczêto ³adowanie sceny: " + Time.time);

        while ((Time.time - loadStartTime) < minimumLoadTime)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("Zakoñczono ³adowanie sceny: " + Time.time);

        SceneManager.UnloadSceneAsync(thisSceneData.sceneIndex);

        StartCoroutine(LoadLoadingScreenAndFindSystem());

        yield return StartCoroutine(FadeToClear());

    }

    public void GoBackToMainMenu()
    {
        StartCoroutine(ReturnToMainMenu(levelConector.mainMenuSceneData, levelConector.thisSceneData));
    }

    private IEnumerator ReturnToMainMenu(SceneData mainMenuSceneData, SceneData thisSceneData)
    {
        Time.timeScale = 1f;

        yield return StartCoroutine(FadeToBlack());

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainMenuSceneData.sceneIndex, LoadSceneMode.Additive);

        movesManager.moveCount = 0;
        movesManager.textMeshPro.text = $"Moves: {movesManager.moveCount}";
        movesManager.textMeshPro.gameObject.SetActive(false);
        canvas.sortingOrder = 0;
        levelConector.pauseMenu.pauseMenuUI.SetActive(false);

        asyncLoad.allowSceneActivation = false;

        float minimumLoadTime = 1f;
        float loadStartTime = Time.time;

        Debug.Log("Rozpoczêto ³adowanie sceny: " + Time.time);

        while ((Time.time - loadStartTime) < minimumLoadTime)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("Zakoñczono ³adowanie sceny: " + Time.time);

        SceneManager.UnloadSceneAsync(thisSceneData.sceneIndex);

        yield return StartCoroutine(FadeToClear());

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

                loadingHandel.SetActive(true);

                stageText.gameObject.SetActive(true);
                ShowCurrentStage();

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

                loadingHandel.SetActive(false);
                stageText.text = "";
                stageText.gameObject.SetActive(false);

                yield return null;
            }
        }
    }

    private void ShowCurrentStage()
    {
        bool isMainMenuLoaded = false;
        if (SceneManager.sceneCount > 1)
        {
            Scene secondScene = SceneManager.GetSceneAt(1);
            if (secondScene.name == "MainMenu")
            {
                isMainMenuLoaded = true;
            }
        }

        if (!isMainMenuLoaded)
        {
            stageText.gameObject.SetActive(true);
            stageText.text = levelConector.sceneData.stageString;
        }
    }
}
