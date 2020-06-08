using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicFile : MonoBehaviour
{
    public MusicFileData musicFileData;

    public AudioSource[] audioSources;

    public MusicFile(MusicFileData musicFileDataToAdd)
    {
        musicFileData = musicFileDataToAdd;
    }

    public AudioClip[] GetAudioClips()
    {
        return musicFileData.audioClips;
    }

    public AudioMixerGroup GetMixerGroup(int i)
    {
        int index = 0;

        if(musicFileData.audioMixerGroup.name == "Song1")
        {
            index = 0;
        }
        else
        {
            index = 1;
        }

        switch (i)
        {
            case 0:
                return musicFileData.audioMixerGroup.audioMixer.FindMatchingGroups("Track1")[index];
            case 1:
                return musicFileData.audioMixerGroup.audioMixer.FindMatchingGroups("Track2")[index];
            case 2:
                return musicFileData.audioMixerGroup.audioMixer.FindMatchingGroups("Track3")[index];
            case 3:
                return musicFileData.audioMixerGroup.audioMixer.FindMatchingGroups("Track4")[index];
        }
        
        return musicFileData.audioMixerGroup;
    }

    //Get track
    //Etc


}
