using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Root : MonoBehaviour
{
    public SceneData sceneToLoad;
    //public SceneData fadeSceneToLoad;
    public SceneData thisRootSceneToUnload;

    public void Start()
    {
        StartCoroutine(LoadScenes());
    }

    private IEnumerator LoadScenes()
    {
        // Asynchroniczne ³adowanie scen
        AsyncOperation loadScene = SceneManager.LoadSceneAsync(sceneToLoad.sceneIndex, LoadSceneMode.Additive);
       // AsyncOperation loadfadeScene = SceneManager.LoadSceneAsync(fadeSceneToLoad.sceneIndex, LoadSceneMode.Additive);

        // Czekaj, a¿ sceny zostana za³adowane
        while (!loadScene.isDone)
        {
            yield return null;
        }

        // Asynchroniczne wy³adowanie bie¿¹cej sceny
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(thisRootSceneToUnload.sceneIndex);

        // Czekaj, a¿ scena zostanie wy³adowana
        while (!unloadOperation.isDone)
        {
            yield return null;
        }
    }
}