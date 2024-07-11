using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Root : MonoBehaviour
{
    public SceneData sceneToLoad;
    public SceneData thisRootSceneToUnload;

    public void Start()
    {
        StartCoroutine(LoadScenes());
       
    }

    private IEnumerator LoadScenes()
    {
        AsyncOperation loadScene = SceneManager.LoadSceneAsync(sceneToLoad.sceneIndex, LoadSceneMode.Additive);
        while (!loadScene.isDone)
        {
            yield return null;
        }
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(thisRootSceneToUnload.sceneIndex);
        while (!unloadOperation.isDone)
        {
            yield return null;
        }
    }
}