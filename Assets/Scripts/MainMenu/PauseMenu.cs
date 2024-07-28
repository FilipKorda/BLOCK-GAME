using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private StatsPanel statsPanel;
    [SerializeField] private Player player;
    [SerializeField] private MoveTracker moveTracker;
    [SerializeField] private StarTracker starTracker;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
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

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
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
