using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundFile 
{

    public SoundFileData soundFileData;
    
    public SoundFile(SoundFileData soundFileDataToAdd)
    {
        soundFileData = soundFileDataToAdd;
    }

    int index = 0;

    public AudioClip GetSound()
    {
        switch (soundFileData.soundType)
        {
            case SoundFileData.SoundType.oneShot:
                return GetOneShot();
            case SoundFileData.SoundType.random:
                return GetRandom();
            case SoundFileData.SoundType.sequence:
                return GetSequence();
        }

        return null;
    }

    public AudioClip GetSound(int index)
    {
        return soundFileData.audioClips[index];
    }

    private AudioClip GetOneShot()
    {
        return soundFileData.audioClips[0];
    }

    private AudioClip GetRandom()
    {
        index = Random.Range(0, soundFileData.audioClips.Length);
        return soundFileData.audioClips[index];
    }

    private AudioClip GetSequence()
    {
        AudioClip audioClipToReturn = soundFileData.audioClips[index];

        if(index < soundFileData.audioClips.Length - 1)
            index++;
        else
        {
            index = 0;
        }

        return audioClipToReturn;
    }

    public AudioMixerGroup GetMixerGroup()
    {
        return soundFileData.audioMixerGroup;
    }

    [HideInInspector] public AudioSource audioSource;

}
