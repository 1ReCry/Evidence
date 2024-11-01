using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatsButton : MonoBehaviour
{
    public int gameID;

    public void SelectCurrentGame()
    {
        Menu menuscript = FindObjectOfType<Menu>();

        menuscript.selectedGame = gameID;

        menuscript.ButtonSound2();
        menuscript.UpdateGameStats();
        menuscript.OpenMatchStats();
    }
}
