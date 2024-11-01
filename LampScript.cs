using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampScript : MonoBehaviour
{
    private GameManager manager;
    public Light lightsource;
    public GameObject lamp;
    public bool lighter_enabled = true;
    private bool lastStateEnabled = true;

    void Start()
    {
       manager = FindObjectOfType<GameManager>(); 

       if(Globals.difficultyID >= 2) 
       {
            lastStateEnabled = false;
            lighter_enabled = false;
       }
       if(Globals.difficultyID < 2) 
       {
            lastStateEnabled = true;
            lighter_enabled = true;
       }
    }

    void Update()
    {
        if(lighter_enabled)
        {
            lamp.SetActive(true);
            lightsource.enabled = true;
            if(!lastStateEnabled)
            {
                //GlobalFunc.CreateSound(this.gameObject, Resources.Load<GameObject>("Audio/SoundLightersOff"), "Audio/Sounds/lights_off", 1f, Random.Range(0.97f,1.02f), false);
                lastStateEnabled = true;
            }
        }

        if(!lighter_enabled)
        {
            lamp.SetActive(false);
            lightsource.enabled = false;
            if(lastStateEnabled)
            {
                //GlobalFunc.CreateSound(this.gameObject, Resources.Load<GameObject>("Audio/SoundLightersOff"), "Audio/Sounds/lights_off", 1f, Random.Range(0.97f,1.02f), false);
                lastStateEnabled = false;
            }
        }
    }
}
