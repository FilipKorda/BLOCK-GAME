using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private StatsPanel statsPanel;
    [SerializeField] private Player player;
    [SerializeField] private MoveTracker moveTracker;
    [SerializeField] private StarTracker starTracker;
    [Tooltip("Przycisk zaznaczany po otwarciu pauzy, gdy podłączony jest pad. Puste = najwyżej położony przycisk.")]
    [SerializeField] private Selectable firstSelected;


    void Update()
    {
        if (GameInput.Pause.WasPressedThisFrame())
        {
            if (!GameIsPaused && IsLevelEnded())
            {
                return;
            }

            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // Po wygranej lub porażce trwa już przejście do innej sceny - pauza (i wyjście do menu)
    // uruchomiłaby drugie, równoległe ładowanie. Sprawdzamy wszystkich graczy z tej sceny,
    // bo w trybie dwóch kostek spada kostka, a nie główny gracz.
    private bool IsLevelEnded()
    {
        foreach (Player scenePlayer in FindObjectsByType<Player>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (scenePlayer.gameObject.scene == gameObject.scene && scenePlayer.LevelEnded)
            {
                return true;
            }
        }
        return false;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        GamepadMenuSelection.Pop(pauseMenuUI.transform);
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        statsPanel.gameObject.SetActive(false);

        moveTracker.ShowMoves();
        starTracker.ShowStars();
        statsPanel.DeactiveStatsPanel();

        Time.timeScale = 1f;
        GameIsPaused = false;
        player.canMove = true;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        GamepadMenuSelection.Push(pauseMenuUI.transform, firstSelected);
        statsPanel.gameObject.SetActive(true);

        statsPanel.UpdateMoves();
        statsPanel.UpdateStars();
        statsPanel.UpdateAttempts();
        moveTracker.HideMoves();
        starTracker.HideStars();
        statsPanel.ActiveStatsPanel();

        Time.timeScale = 0f;
        GameIsPaused = true;
        player.canMove = false;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}
