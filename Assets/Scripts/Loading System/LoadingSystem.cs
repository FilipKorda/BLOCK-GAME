using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class LoadingSystem : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;
    [SerializeField] private GameObject loadingHandel;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private MoveTracker moveTracker;
    [SerializeField] private StarTracker starTracker;
    [Header("===========  Load Next Level  ===========")]
    public SceneData sceneToLoad;
    public SceneData sceneToUnload;
    [Header("===========   Restart This Level   ===========")]
    public SceneData thisSceneToLoad;
    private bool isResetingLevel;
    [Header("===========   Load Main Menu   ===========")]
    public SceneData mainMenuToLoad;
    private bool returnToMainMenu;

    public SceneData thisSceneToUnload;

    private void Start()
    {
        StartCoroutine(FadeToClear());
        loadingHandel.SetActive(true);
    }

    public void LoadNextLexel()
    {
        StartCoroutine(LoadNextLevel(sceneToLoad, sceneToUnload));       
    }

    private IEnumerator LoadNextLevel(SceneData sceneData, SceneData thisSceneName)
    {
        yield return StartCoroutine(FadeToBlack());

        Debug.Log("Load: " + sceneData.stageString);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneData.sceneIndex, LoadSceneMode.Additive);

        asyncLoad.allowSceneActivation = false;

        float minimumLoadTime = 1f;
        float loadStartTime = Time.time;


        while ((Time.time - loadStartTime) < minimumLoadTime)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SceneManager.UnloadSceneAsync(thisSceneName.sceneIndex);
        GameManager.Instance.realGameTime.StartTime();

    }

    public void ResetThisLevel()
    {
        StartCoroutine(ResetThisLevel(thisSceneToLoad, thisSceneToUnload));
    }

    private IEnumerator ResetThisLevel(SceneData sceneData, SceneData thisSceneData)
    {
        isResetingLevel = true;
        yield return StartCoroutine(FadeToBlack());
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneData.sceneIndex, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        float minimumLoadTime = 1f;
        float loadStartTime = Time.time;


        while ((Time.time - loadStartTime) < minimumLoadTime)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }


        SceneManager.UnloadSceneAsync(thisSceneData.sceneIndex);
        isResetingLevel = false;
    }

    public void GoBackToMainMenu()
    {
        StartCoroutine(ReturnToMainMenu(mainMenuToLoad, thisSceneToUnload));
    }

    private IEnumerator ReturnToMainMenu(SceneData mainMenuSceneData, SceneData thisSceneData)
    {
        GameManager.Instance.moveCount = 0;
        returnToMainMenu = true;
        Time.timeScale = 1f;
        PauseMenu.GameIsPaused = false;
        yield return StartCoroutine(FadeToBlack());

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainMenuSceneData.sceneIndex, LoadSceneMode.Additive);

        asyncLoad.allowSceneActivation = false;

        float minimumLoadTime = 1f;
        float loadStartTime = Time.time;

        while ((Time.time - loadStartTime) < minimumLoadTime)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SceneManager.UnloadSceneAsync(thisSceneData.sceneIndex);

        returnToMainMenu = false;
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
                if (moveTracker != null && starTracker != null)
                {
                    moveTracker.HideMoves();
                    starTracker.HideStars();
                }
                if (!returnToMainMenu)
                {
                    if (isResetingLevel)
                    {
                        ResetShowStageText();
                    }
                    else
                    {
                        ShowStageText();
                    }
                }
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
                HideStageText();

                if (moveTracker != null && starTracker != null)
                {
                    moveTracker.ShowMoves();
                    starTracker.ShowStars();
                }
                    
                yield return null;
            }
        }
    }

    public void ShowStageText()
    {
        if (stageText != null)
            stageText.text = sceneToLoad.stageString;
    }

    private void ResetShowStageText()
    {
        if (stageText != null)
            stageText.text = thisSceneToLoad.stageString;
    }

    public void HideStageText()
    {
        if (stageText != null)
            stageText.text = "";
    }

    public void Quit()
    {
        Application.Quit();
    }
}

