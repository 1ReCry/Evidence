using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Keybinds : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    [Header("move_left, move_right, move_forward, move_backward, interact, use, run, crouch, pause, journal, flashlight, zoom")]
    public string keyName;
    public KeyCode keyIfNone;
    public string waitingText = "[Нажмите на клавишу]";

    void Start()
    {
        buttonText.text = PlayerPrefs.GetString(keyName, keyIfNone.ToString());
        if(buttonText.text == keyIfNone.ToString()) PlayerPrefs.SetString(keyName, keyIfNone.ToString());
    }

    void Update()
    {
        if(buttonText.text == waitingText)
        {
            foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
            {
                if(Input.GetKey(keycode))
                {
                    buttonText.text = keycode.ToString();
                    PlayerPrefs.SetString(keyName, keycode.ToString());
                    PlayerPrefs.Save();
                }
            }
        }
        if(buttonText.text != waitingText) buttonText.text = PlayerPrefs.GetString(keyName, keyIfNone.ToString());
    }

    public void ChangeKey()
    {
        buttonText.text = waitingText;
    }
}
