using System;
using System.Collections;
using UnityEngine;

public class LevelConector : MonoBehaviour
{
    public NextLevelInfo nextSceneLevelInfo;
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

    public void LoadNextLexel()
    {
        if (loadingSystem != null)
        {
            loadingSystem.LoadNextLexel();
        }
    }

    public void ResetThisLevel()
    {
        if (loadingSystem != null)
        {
            loadingSystem.ResetThisLevel();
        }
    }
}
[Serializable]
public class NextLevelInfo
{
    [Header("===========  Scene To Load  ===========")]
    public string sceneName;
    public int buildIndex;
    [Header("===========   Scene To Unload   ===========")]
    public string thisSceneName;
    public int thisBuildIndex;

}
