using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "MusicFileData", menuName = "Audio/MusicFile", order = 1)]
public class MusicFileData : ScriptableObject
{
    public bool loop = true;
    public AudioMixerGroup audioMixerGroup;
    public AudioClip[] audioClips;
}
