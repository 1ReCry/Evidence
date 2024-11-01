using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalFunc
{
    public static void CreateSound(GameObject entity_object, GameObject sound_prefab, string sound_path, float volume, float pitch, bool pinToObject)
    {
        Debug.Log("Created Sound | by:"+entity_object.name+" | prefab: " + sound_prefab.name + " | sound_path: " + sound_path + " | vol: " + volume + " | pitch: " + pitch);
        GameObject sound = Object.Instantiate(sound_prefab);
        if(pinToObject)
        {
            sound.transform.SetParent(entity_object.transform);
            sound.transform.localPosition = new Vector3(0, 0, 0);
        }
        if(!pinToObject)
        {
            sound.transform.position = entity_object.transform.position;
        }
        
        SoundObject step_sound_obj = sound.GetComponent<SoundObject>();
        step_sound_obj.sound = Resources.Load<AudioClip>(sound_path);
        step_sound_obj.volume = volume;
        step_sound_obj.pitch = pitch;
    }

    public static void LoadSettings()
    {
        Globals.sensitivity = PlayerPrefs.GetFloat("sensitivity", 130f);
        Globals.postprocessing = PlayerPrefs.GetInt("postprocessing", 1);
        Globals.fpscounter = PlayerPrefs.GetInt("fpscounter", 0);
    }

    public static void ResetControls()
    {
        //"move_left, move_right, move_forward, move_backward, interact, use, run, crouch, pause, journal, flashlight, zoom"
        PlayerPrefs.SetString("move_left", "A");
        PlayerPrefs.SetString("move_right", "D");
        PlayerPrefs.SetString("move_forward", "W");
        PlayerPrefs.SetString("move_backward", "S");
        PlayerPrefs.SetString("interact", "E");
        PlayerPrefs.SetString("use", "Mouse0");
        PlayerPrefs.SetString("run", "LeftShift");
        PlayerPrefs.SetString("crouch", "LeftControl");
        PlayerPrefs.SetString("pause", "Escape");
        PlayerPrefs.SetString("journal", "J");
        PlayerPrefs.SetString("flashlight", "F");
        PlayerPrefs.SetString("zoom", "V");
    }
}
