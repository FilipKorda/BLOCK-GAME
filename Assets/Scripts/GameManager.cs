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

    public event Action OnStatsChanged;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    public void AddAchievementMove()
    {
        MoveCount++;
        OnStatsChanged?.Invoke();
    }

    public void AddAchievementStar()
    {
        StarCount++;
        OnStatsChanged?.Invoke();
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
