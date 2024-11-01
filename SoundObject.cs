using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : MonoBehaviour
{
    private AudioSource audiocomp;
    public AudioClip sound;
    public float volume;
    public float pitch;

    void Start()
    {
        audiocomp = GetComponent<AudioSource>();
        if(sound!=null) audiocomp.clip = sound;
        audiocomp.volume = volume;
        audiocomp.pitch = pitch;
        audiocomp.Play();
    }

    void Update()
    {
        // Уничтожаем объект, когда звук перестанет проигрываться
        if (!audiocomp.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
