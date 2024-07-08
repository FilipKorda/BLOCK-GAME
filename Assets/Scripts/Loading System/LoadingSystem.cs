using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class LoadingSystem : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;
    [SerializeField] private GameObject loadingHandel;
    [Header("===========  Load Next Level  ===========")]
    public SceneData sceneToLoad;
    public SceneData sceneToUnload;
    [Header("===========   Restart This Level   ===========")]
    public SceneData thisSceneToLoad;  
    [Header("===========   Load Main Menu   ===========")]
    public SceneData mainMenuToLoad;

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

       // yield return StartCoroutine(FadeToClear());
    }

    public void ResetThisLevel()
    {
        StartCoroutine(ResetCurretLevel(thisSceneToLoad, thisSceneToUnload));
    }

    private IEnumerator ResetCurretLevel(SceneData sceneData, SceneData thisSceneData)
    {

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

    }

    public void GoBackToMainMenu()
    {
        StartCoroutine(ReturnToMainMenu(mainMenuToLoad, thisSceneToUnload));
    }

    private IEnumerator ReturnToMainMenu(SceneData mainMenuSceneData, SceneData thisSceneData)
    {
        Time.timeScale = 1f;


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
                yield return null;
            }
        }
    }
}

