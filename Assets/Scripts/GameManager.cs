using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int moveCount = 0;

    public int starCount = 0;

    public int attemptsNumber = 1;

    public RealGameTime realGameTime;
  
    public int currentUnlockedAvailableCodesIndex = -1;
    public HashSet<int> completedLevels = new();

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
    }

}
