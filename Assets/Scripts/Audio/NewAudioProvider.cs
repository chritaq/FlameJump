using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAudioProvider : IAudioService
{

    private GameObject audioSourceContainer;

    private AudioSource[] jumpAudioSource;
    private AudioSource[] landAudioSource;
    private AudioSource[] bounceAudioSource;
    private AudioSource[] dashAudioSource;
    private AudioSource deathAudioSource;
    private AudioSource[] pickupAudioSource;

    private AudioSource musicGameplay01AudioSource;

    private AudioSource menuStartGameAudioSource;

    public void LoadSounds()
    {
        audioSourceContainer = GameObject.FindGameObjectWithTag("AudioSources");

        jumpAudioSource = InstantiateAudioSources(jumpAudioSource, "Player_Jump");
        dashAudioSource = InstantiateAudioSources(dashAudioSource, "Player_Dash");
        landAudioSource = InstantiateAudioSources(landAudioSource, "Player_Land");
        deathAudioSource = instantiateSingleAudioSource(deathAudioSource, "Player_Death");
        pickupAudioSource = InstantiateAudioSources(pickupAudioSource, "Pickup_Recharge");
        bounceAudioSource = InstantiateAudioSources(bounceAudioSource, "Player_Bounce");


        musicGameplay01AudioSource = instantiateSingleAudioSource(musicGameplay01AudioSource, "Music_Gameplay01");

        menuStartGameAudioSource = instantiateSingleAudioSource(menuStartGameAudioSource, "Menu_StartGame");


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

    public void PlaySound(string soundID, bool interuptLast)
    {
        switch (soundID)
        {
            case "Player_Jump":
                PlaySoundFromArray(jumpAudioSource);
                break;

            case "Player_Dash":
                PlaySoundFromArray(dashAudioSource);
                break;

            case "Player_Bounce":
                PlaySoundFromArray(bounceAudioSource);
                break;

            case "Player_Death":
                deathAudioSource.Play();
                PlaySoundFromArray(bounceAudioSource);
                break;

            case "Player_Land":
                PlaySoundFromArray(landAudioSource);
                break;

            case "Pickup_Recharge":
                PlaySoundFromArray(pickupAudioSource);
                break;

            case "Music_Gameplay01":
                musicGameplay01AudioSource.Play();
                break;

            case "Menu_StartGame":
                menuStartGameAudioSource.Play();
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

    public void PlayMusic(string musicID)
    {
        throw new System.NotImplementedException();
    }

    public void PlaySound(string soundID, SoundType soundType)
    {
        throw new System.NotImplementedException();
    }
}
