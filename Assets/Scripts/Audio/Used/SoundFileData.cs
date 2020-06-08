using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SoundFileData", menuName = "Audio/SoundFile", order = 1)]
public class SoundFileData : ScriptableObject
{
    public enum SoundType
    {
        oneShot,
        random,
        sequence
    }

    public SoundType soundType;

    public AudioClip[] audioClips;
    public AudioMixerGroup audioMixerGroup;
}
