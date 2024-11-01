using System.Data.Common;
using UnityEngine;
using OPS.Obfuscator.Attribute;

[DoNotObfuscateClass]
public class SaveLoad : MonoBehaviour
{
    public static bool isDataLoaded = false;
    private float saveTimer = 0.2f;

    private void Start()
    {
        if (!isDataLoaded)
        {
            Globals.LoadData();
            Globals.LoadGamesData();
            isDataLoaded = true;
        }
    }

    private void FixedUpdate()
    {
        saveTimer -= Time.deltaTime;
        if (saveTimer <= 0 && isDataLoaded)
        {
            saveTimer = 2;
            Globals.SaveData();
            Globals.SaveGamesData();
        }
    }
}

