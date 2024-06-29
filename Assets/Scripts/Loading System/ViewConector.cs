using System.Collections;
using UnityEngine;

public class ViewConector : MonoBehaviour
{
    private LoadingSystem loadingSystem;

    private void Start()
    {
        StartCoroutine(LoadLoadingScreenAndFindSystem());
    }

    private IEnumerator LoadLoadingScreenAndFindSystem()
    {
        var loadingSystemObject = FindObjectOfType<LoadingSystem>();
        if (loadingSystemObject != null)
        {
            loadingSystem = loadingSystemObject;
        }
        else
        {
            Debug.LogError("LoadingSystem not found in LoadingScreen");
        }
        yield return null;
    }

    public void OnLoadNextSceneButtonClicked()
    {
        if (loadingSystem != null)
        {
            loadingSystem.LoadStartScene();
        }
    }
}
