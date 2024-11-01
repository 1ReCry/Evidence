using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelectButton : MonoBehaviour
{

    public string mapName;
    public string sceneName;
    public int setDifficultyID;

    public void SelectMap()
    {
        Globals.selectedMapSceneName = sceneName;
        Globals.selectedMapUIName = mapName;
    }

    public void SelectDifficulty()
    {
        Globals.difficultyID = setDifficultyID;
    }

    public void ResetSelectMap()
    {
        Globals.selectedMapSceneName = "";
        Globals.selectedMapUIName = "";
        Globals.difficultyID = -1;
    }
}
