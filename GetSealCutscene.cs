using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetSealCutscene : MonoBehaviour
{
    public float timer;

    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0) SceneManager.LoadScene("Menu");
    }
}
