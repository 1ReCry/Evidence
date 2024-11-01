using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampSwitch : MonoBehaviour
{
    [Header("Привязанная лампа")]
    public LampScript linked_lamp;
    public bool isEnabled;

    private GameManager manager;

    void Start()
    {
        manager = FindObjectOfType<GameManager>();

        if(Globals.difficultyID == 0 || Globals.difficultyID == 1) isEnabled = true;
        if(Globals.difficultyID >= 2) isEnabled = false;
    }

    void Update()
    {
        if(isEnabled)
        {
            linked_lamp.lighter_enabled = true;
        }
        else
        {
            linked_lamp.lighter_enabled = false;
        }

        if(!manager.electric_enabled)
        {
            linked_lamp.lighter_enabled = false;
        }
    }

    public void Interact()
    {
        isEnabled = !isEnabled;
        if(isEnabled) GlobalFunc.CreateSound(this.gameObject, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/UI/click", 1f, Random.Range(0.97f,1.02f), false);
        if(!isEnabled) GlobalFunc.CreateSound(this.gameObject, Resources.Load<GameObject>("Audio/SoundPrefab"), "Audio/Sounds/UI/click", 1f, Random.Range(0.80f,0.86f), false);
    }
}
