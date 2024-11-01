using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OPS.Obfuscator.Attribute;

// сохранения
// переменные для сохранения должны называтся одинаково в Globals и GameData
// сохраняются все переменные Globals из GameData

[DoNotObfuscateClass]
[System.Serializable]
public class GameData
{
    public float inGameTime;

    public int gameCompletesCount;
    public int gamesPlayed;
    
    public int gameCompletesDifficulty0;
    public int gameCompletesDifficulty1;
    public int gameCompletesDifficulty2;
    public int gameCompletesDifficulty3;
    public int gameCompletesDifficulty4;

    public int xp;
    public int level;
    
    public int rankedPoints;
    public int fullRankedPoints;
    public int maxRankedPoints;
    public int rankedLevel;

    public int monsterEscaped;
    public int monsterMeetings;
    public int allGameNotesCollected;
    public int allItemsCollected;

    public bool controlsNotSetted;

    public int archonSeals;
}

