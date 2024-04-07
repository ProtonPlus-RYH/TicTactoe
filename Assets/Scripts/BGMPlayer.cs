using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoSingleton<BGMPlayer>
{
    public AudioSource Audio;

    public AudioClip Hitoshizuku;
    public AudioClip Utakotoba;
    public AudioClip Katyusha;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Audio.clip = Hitoshizuku;
        if (!PlayerPrefs.HasKey("BGMVolume"))
        {
            PlayerPrefs.SetInt("BGMVolume", 50);
        }
        Audio.volume = 0.01f * PlayerPrefs.GetInt("BGMVolume");
        Audio.Play();
    }
}
