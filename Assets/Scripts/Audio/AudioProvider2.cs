using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioProvider2 : IAudioService
{
    public bool shouldDebug = false;
    public AudioProvider2(bool newShouldDebug)
    {
        shouldDebug = newShouldDebug;
    }

    GameObject audioSourcesGameObject;

    public const int SOURCE_AMOUNT = 10;
    private List<AudioSource> sources = new List<AudioSource>();
    private SoundFileData[] soundFileDatas;
    //private SoundFile[] soundFiles;

    private List<SoundFile> soundFiles = new List<SoundFile>();

    Dictionary<string, SoundFile> soundDictionary = new Dictionary<string, SoundFile>();

    public void LoadSounds()
    {
        //Here we'll put all sounds to load.
        //Texture2D[] textures = Resources.LoadAll<Texture2D>("Textures");
        soundFileDatas = Resources.LoadAll<SoundFileData>("SoundFileDatas");

        audioSourcesGameObject = new GameObject("AudioSourcesGameObject");

        for (int i = 0; i < SOURCE_AMOUNT; i++)
        {
            sources.Add(audioSourcesGameObject.AddComponent<AudioSource>());
            sources[i].playOnAwake = false;
            DebugLogger("Added new audiosource");
        }


        //soundFileDatas = (SoundFileData[])Resources.LoadAll("Audio/SoundFileDatas", typeof(SoundFileData));
        //soundFiles = new SoundFile[soundFileDatas.Length];

        DebugLogger("Amount of soundfileDatas: " + soundFileDatas.Length);

        for (int i = 0; i < soundFileDatas.Length; i++)
        {
            //soundFiles.Add(new SoundFile());
            //soundFiles[i].soundFileData = soundFileDatas[i];
            if(!soundDictionary.ContainsKey(soundFileDatas[i].name))
            {
                soundDictionary.Add(soundFileDatas[i].name, new SoundFile(soundFileDatas[i]));
                DebugLogger("Added sound: " + soundFileDatas[i].name);
            }
            else
            {
                soundDictionary[soundFileDatas[i].name] = new SoundFile(soundFileDatas[i]);
                DebugLogger("Set sound to: " + soundFileDatas[i].name);
            }

        }
        
    }

    public void PlaySound(string soundID)
    {
        //TODO
        //Make sure the soundfiles gets loaded at start instead of here. And then references instead.
        //SoundFileData soundFile = (SoundFileData)Resources.Load("Audio/SoundFiles" + soundID, typeof(AudioClip));

        SoundFile soundFile;

        if(soundDictionary.ContainsKey(soundID))
        {
            soundFile = soundDictionary[soundID];
            DebugLogger("Got sound in provider: " + soundDictionary[soundID].soundFileData.name);
        }
        else
        {
            DebugLogger("No Sound with name " + soundID);
            return;
        }

        AudioClip audioClip = soundFile.GetSound();

        if (audioClip == null)
        {
            DebugLogger("No audioclip in soundfile object");
            return;
        }

        AudioSource audioSource = GetAvaliableAudioSource();

        if (!audioSource)
        {
            DebugLogger("No source avaible");
            return;
        }

        audioSource.clip = audioClip;

        audioSource.outputAudioMixerGroup = soundFile.GetMixerGroup();

        audioSource.Play();


    }

    private AudioSource GetAvaliableAudioSource()
    {
        AudioSource selected = null;

        for(int i = 0; i < sources.Count; i++)
        {
            DebugLogger("Checking index " + i);
            if(sources[i] != null)
            {
                if(sources[i].clip == null || !sources[i].isPlaying)
                {
                    selected = sources[i];
                    break;
                }
                
            }
            else
            {
                DebugLogger("Source at index " + i + "is null");
            }
            
        }


        if(selected == null)
        {
            sources.Add(audioSourcesGameObject.AddComponent<AudioSource>());
            sources[sources.Count - 1].playOnAwake = false;
            selected = sources[sources.Count - 1];
        }

        return selected;
    }

    public void StopAll()
    {
        for(int i = 0; i < sources.Count - 1; i++)
        {
            sources[i].Stop();
        }
    }

    public void StopSound(string soundID)
    {

    }

    
    private void DebugLogger(object message)
    {
        if(shouldDebug)
        {
            Debug.Log(message);
        }
    }


}
