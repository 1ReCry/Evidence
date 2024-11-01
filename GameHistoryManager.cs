using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHistoryManager : MonoBehaviour
{
    public static GameHistoryManager Instance;
    public List<GameInfo> gameHistory = new List<GameInfo>();
    public int test;

    void Awake()
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

    public void AddGame(GameInfo game)
    {
        gameHistory.Add(game);
    }
}