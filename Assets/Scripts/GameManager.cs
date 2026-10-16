using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int MoveCount = 0;

    public int StarCount = 0;

    public int AttemptsNumber = 1;

    public RealGameTime realGameTime;
  
    public int currentUnlockedAvailableCodesIndex = -1;
    public HashSet<int> completedLevels = new();

    // Poziomy naprawdę wygrane (gracz wszedł do mety). completedLevels dopisuje poziom już przy
    // jego wczytaniu (odblokowanie kodów), więc do odblokowywania skinów służy ta lista.
    public HashSet<string> finishedLevels = new();

    public void MarkLevelFinished(string levelName)
    {
        if (finishedLevels.Add(levelName))
        {
            SaveProgress();
            OnStatsChanged?.Invoke();
        }
    }

    public event Action OnStatsChanged;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SaveSystem.RestoreProgress(this);
        }
        else
        {
            Destroy(gameObject);
        }

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    public void SaveProgress() => SaveSystem.SaveProgress(this);

    private void OnApplicationQuit()
    {
        if (Instance == this) SaveProgress();
    }

    public void AddAchievementMove()
    {
        MoveCount++;
        OnStatsChanged?.Invoke();
    }

    // Gwiazdki sumują się przez całą rozgrywkę (zerowane dopiero po powrocie do menu).
    // StarCount zawiera też gwiazdki z bieżącego poziomu - wygrana zatwierdza je na stałe,
    // a spadnięcie lub restart poziomu je cofa (gwiazdki w poziomie pojawiają się z powrotem).
    private int levelStarCount;

    public void AddAchievementStar()
    {
        StarCount++;
        levelStarCount++;
        OnStatsChanged?.Invoke();
    }

    public void CommitLevelStars()
    {
        levelStarCount = 0;
    }

    public void DiscardLevelStars()
    {
        StarCount = Mathf.Max(0, StarCount - levelStarCount);
        levelStarCount = 0;
        OnStatsChanged?.Invoke();
    }

    public void ResetRunStats()
    {
        MoveCount = 0;
        StarCount = 0;
        AttemptsNumber = 1;
        levelStarCount = 0;
    }

    public void AddAchievementAttempts()
    {
        AttemptsNumber++;
        OnStatsChanged?.Invoke();
    }


    //TEST Buttons
    public void AddTESTAchievementMove(int moveCount)
    {
        MoveCount += moveCount;
        OnStatsChanged?.Invoke();
    }
    public void AddTESTAchievementStar(int starCount)
    {
        StarCount += starCount;
        OnStatsChanged?.Invoke();
    }
    public void AddTESTAchievementAttempts(int attemptsNumber)
    {
        AttemptsNumber += attemptsNumber;
        OnStatsChanged?.Invoke();
    }
}
