using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electropanel : MonoBehaviour
{
    private GameManager manager;
    public GameObject enabled_ind;
    public GameObject disabled_ind;

    void Start()
    {
        manager = FindObjectOfType<GameManager>(); 
    }

    void Update()
    {
        if(manager.electric_enabled)
        {
            enabled_ind.SetActive(true);
            disabled_ind.SetActive(false);
        }
        if(!manager.electric_enabled)
        {
            enabled_ind.SetActive(false);
            disabled_ind.SetActive(true);
        }
    }

    public void Interact()
    {
        manager.electric_enabled = !manager.electric_enabled;
        GlobalFunc.CreateSound(this.gameObject, Resources.Load<GameObject>("Audio/SoundLightersOff"), "Audio/Sounds/lights_off", 1f, Random.Range(0.97f,1.02f), false);
    }
}
