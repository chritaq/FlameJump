using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioService
{
    void LoadSounds();
    void PlaySound(string soundID, SoundType soundType);
    void PlayMusic(string musicID);
    /// <summary>
    /// This stops all sound
    /// </summary>
    /// <param name="soundID">the ID of the soundclip (aka name)</param>
    void StopSound(string soundID);
    void StopAll();
}
