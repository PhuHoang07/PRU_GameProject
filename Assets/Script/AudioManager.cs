using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Source")]
    [SerializeField] AudioSource gameMusic;
    [SerializeField] AudioSource gameSFX;

    [Header("Audio Clip")]
    public AudioClip backgroundMusic;
    public AudioClip bounceSound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    private void Start()
    {
        gameMusic.clip = backgroundMusic;
        gameMusic.Play();
    }

    public void changeMusicVolumn(float volume)
    {
        gameMusic.volume = volume; 
    }
    
    public void changeSFXVolumn(float volume)
    {
        gameSFX.volume = volume; 
    }

    public void PlaySFX(AudioClip audioClip)
    {
        gameSFX.PlayOneShot(audioClip);
    }

}
