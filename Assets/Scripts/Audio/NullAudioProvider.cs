using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullAudioProvider : IAudioService
{
    public void LoadSounds()
    {
    }

    public void PlayMusic(string musicID)
    {
        throw new System.NotImplementedException();
    }

    public void PlaySound(string soundID)
    {
    }

    public void StopAll()
    {
    }

    public void StopSound(string soundID)
    {

    }
}
