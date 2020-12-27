using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullAudioProvider : IAudioService
{
    public void FadeOutMusic(string musicID)
    {
    }

    public void LoadSounds()
    {
    }

    public void PlayMusic(string musicID)
    {
        throw new System.NotImplementedException();
    }

    public void PlaySound(string soundID, bool interupLast)
    {
    }

    public void PlaySound(string soundID, SoundType soundType)
    {
    }

    public void StopAll()
    {
    }

    public void StopMusic(string musicID)
    {
    }

    public void StopSound(string soundID)
    {

    }
}
