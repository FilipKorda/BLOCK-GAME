using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Collections.Generic;

public class Root : MonoBehaviour
{
    public List<SceneInfo> scenesToLoad = new();

    public void Start()
    {
        StartCoroutine(LoadScenes());
    }

    private IEnumerator LoadScenes()
    {
        foreach (var sceneInfo in scenesToLoad)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneInfo.buildIndex, LoadSceneMode.Additive);
            yield return asyncLoad;
        }
    }


}
[Serializable]
public class SceneInfo
{
    public string sceneName;
    public int buildIndex;
}