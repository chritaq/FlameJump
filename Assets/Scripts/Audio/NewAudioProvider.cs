﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAudioProvider : IAudioService
{

    private GameObject audioSourceContainer;

    private AudioSource[] jumpAudioSource;
    private AudioSource[] dashAudioSource;
    private AudioSource[] deathAudioSource;
    private AudioSource[] pickupAudioSource;

    private AudioSource musicGameplay01AudioSource;

    public void LoadSounds()
    {
        audioSourceContainer = GameObject.FindGameObjectWithTag("AudioSources");

        jumpAudioSource = InstantiateAudioSources(jumpAudioSource, "Player_Jump");
        dashAudioSource = InstantiateAudioSources(dashAudioSource, "Player_Dash");
        deathAudioSource = InstantiateAudioSources(deathAudioSource, "Player_Death");
        pickupAudioSource = InstantiateAudioSources(deathAudioSource, "Pickup_Recharge");


        musicGameplay01AudioSource = instantiateSingleAudioSource(musicGameplay01AudioSource, "Music_Gameplay01");
    }

    private AudioSource instantiateSingleAudioSource(AudioSource audioSource, string soundID)
    {
        Transform tempTransform = audioSourceContainer.transform.Find(soundID);
        audioSource = tempTransform.GetComponent<AudioSource>();
        return audioSource;
    }

    private AudioSource[] InstantiateAudioSources(AudioSource[] audioSource, string soundID)
    {
        audioSource = new AudioSource[CountAudioSources(soundID)];

        for (int i = 0; i < CountAudioSources(soundID); i++)
        {
            Transform tempTransform;

            tempTransform = audioSourceContainer.transform.Find(soundID + i);

            if (tempTransform != null)
            {
                audioSource[i] = tempTransform.GetComponent<AudioSource>();
            }
        }
        return audioSource;
    }

    private int CountAudioSources(string soundID)
    {
        int numberOfSounds = 0;
        while (audioSourceContainer.transform.Find(soundID + numberOfSounds) != null)
        {
            numberOfSounds++;
        }
        return numberOfSounds;
    }

    public void PlaySound(string soundID)
    {
        switch (soundID)
        {
            case "Player_Jump":
                PlaySoundFromArray(jumpAudioSource);
                break;

            case "Player_Dash":
                PlaySoundFromArray(dashAudioSource);
                break;

            case "Player_Death":
                PlaySoundFromArray(deathAudioSource);
                break;

            case "Pickup_Recharge":
                PlaySoundFromArray(pickupAudioSource);
                break;

            case "Music_Gameplay01":
                musicGameplay01AudioSource.Play();
                break;
        }

    }

    private void PlaySoundFromArray(AudioSource[] audioSourceToPlay)
    {
        if(audioSourceToPlay.Length != 0)
        {
            if (audioSourceToPlay.Length > 1)
            {
                audioSourceToPlay[Random.Range(0, audioSourceToPlay.Length)].Play();
            }
            else
            {
                audioSourceToPlay[0].Play();
            }
        }
    }

    public void StopAll()
    {

    }

    public void StopSound(string soundID)
    {

    }

    
}
