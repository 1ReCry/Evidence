using System.Collections;
using System.Collections.Generic;
using OPS.Obfuscator.Attribute;
using UnityEngine;
using System;

[DoNotObfuscateClass]
[System.Serializable]
public class GameInfo
{
    public int MapID;
    public int DifficultyID;
    public int NotesCollected;
    public int NotesOnMap;
    public int SpecialItem;
    public int MonsterType;
    public bool IsWin;
    public bool IsRating;
    public int rankID;
    public int fullRating;
    public float matchTime;
    public string date; 
    public string time;

    public bool itemFinded;
    public bool monsterFinded;
}
