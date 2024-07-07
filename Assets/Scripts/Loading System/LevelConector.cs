using System.Collections;
using UnityEngine;

public class LevelConector : MonoBehaviour
{
    [Header("===========  Scene To Load  ===========")]
    public SceneData sceneData;
    [Header("===========   Scene To Unload   ===========")]
    public SceneData thisSceneData;
    [Header("===========   Main Menu To Load   ===========")]
    public SceneData mainMenuSceneData;

    [SerializeField] private Player player;
    private LoadingSystem loadingSystem;
    public PauseMenu pauseMenu;
    public bool isResetingLevel;
    public bool goingBackToMainMenu;

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
            isResetingLevel = true;
            loadingSystem.ResetThisLevel();
        }
    }

    public void ReturnToMainMenu()
    {
        if (loadingSystem != null && player != null && pauseMenu != null)
        {
            goingBackToMainMenu = true;
            player.canMove = false;
            PauseMenu.GameIsPaused = false;
            pauseMenu.pauseMenuUI.SetActive(false);
            loadingSystem.GoBackToMainMenu();
        }
    }
}
