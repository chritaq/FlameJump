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

    //Audio sources
    GameObject audioSourcesGameObject;
    public const int SOURCE_AMOUNT = 10;
    private List<AudioSource> sources = new List<AudioSource>();

    //Sound Files
    private SoundFileData[] soundFileDatas;
    private List<SoundFile> soundFiles = new List<SoundFile>();
    Dictionary<string, SoundFile> soundDataDictionary = new Dictionary<string, SoundFile>();

    //Music Files
    private MusicFileData[] musicFileDatas;
    private List<MusicFile> musicfiles = new List<MusicFile>();
    Dictionary<string, MusicFile> musicDictionary = new Dictionary<string, MusicFile>();

    

    public void LoadSounds()
    {
        //Here we'll put all sounds to load.
        //Texture2D[] textures = Resources.LoadAll<Texture2D>("Textures");
        soundFileDatas = Resources.LoadAll<SoundFileData>("SoundFileDatas");
        musicFileDatas = Resources.LoadAll<MusicFileData>("MusicFileDatas");

        audioSourcesGameObject = new GameObject("AudioSourcesGameObject");
        Object.DontDestroyOnLoad(audioSourcesGameObject);


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
            if(!soundDataDictionary.ContainsKey(soundFileDatas[i].name))
            {
                soundDataDictionary.Add(soundFileDatas[i].name, new SoundFile(soundFileDatas[i]));
                DebugLogger("Added sound: " + soundFileDatas[i].name);
            }
            else
            {
                soundDataDictionary[soundFileDatas[i].name] = new SoundFile(soundFileDatas[i]);
                DebugLogger("Set sound to: " + soundFileDatas[i].name);
            }

        }

        for (int i = 0; i < musicFileDatas.Length; i++)
        {
            //soundFiles.Add(new SoundFile());
            //soundFiles[i].soundFileData = soundFileDatas[i];
            if (!musicDictionary.ContainsKey(musicFileDatas[i].name))
            {
                musicDictionary.Add(musicFileDatas[i].name, new MusicFile(musicFileDatas[i]));
                DebugLogger("Added music: " + musicFileDatas[i].name);
            }
            else
            {
                musicDictionary[musicFileDatas[i].name] = new MusicFile(musicFileDatas[i]);
                DebugLogger("Set music to: " + musicFileDatas[i].name);
            }

        }

    }



    public void PlaySound(string soundID)
    {
        //TODO
        //Make sure the soundfiles gets loaded at start instead of here. And then references instead.
        //SoundFileData soundFile = (SoundFileData)Resources.Load("Audio/SoundFiles" + soundID, typeof(AudioClip));

        SoundFile soundFile;

        if(soundDataDictionary.ContainsKey(soundID))
        {
            soundFile = soundDataDictionary[soundID];
            DebugLogger("Got sound in provider: " + soundDataDictionary[soundID].soundFileData.name);
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

        if(soundFile.audioSource == null || soundFile.audioSource.clip != soundFile.GetSound(0))
        {

            soundFile.audioSource = GetAvaliableAudioSource();
            if (!soundFile.audioSource)
            {
                DebugLogger("No source avaible");
                return;
            }

            if(soundFile.audioSource.clip == null)
            {
                //This is to show that the audiosource is used already
                soundFile.audioSource.clip = soundFile.GetSound(0);
            }

            soundFile.audioSource.outputAudioMixerGroup = soundFile.GetMixerGroup();
        }

        //AudioSource audioSource = GetAvaliableAudioSource();



        //soundFile.audioSource.clip = audioClip;



        soundFile.audioSource.PlayOneShot(audioClip);

        //soundFile.audioSource.Play();

    }

    public void PlayMusic(string musicID)
    {
        MusicFile musicFile;

        if (musicDictionary.ContainsKey(musicID))
        {
            musicFile = musicDictionary[musicID];
            DebugLogger("Got music in provider: " + musicDictionary[musicID].musicFileData.name);
        }
        else
        {
            DebugLogger("No music with name " + musicID);
            return;
        }

        AudioClip[] audioClips = musicFile.GetAudioClips();

        if (audioClips[0] == null)
        {
            DebugLogger("No audioclips in musicFile object");
            return;
        }

        musicFile.audioSources = new AudioSource[audioClips.Length];

        for (int i = 0; i < audioClips.Length; i++)
        {
            musicFile.audioSources[i] = GetAvaliableAudioSource();

            musicFile.audioSources[i].clip = audioClips[i];

            musicFile.audioSources[i].outputAudioMixerGroup = musicFile.GetMixerGroup(i);

            musicFile.audioSources[i].loop = musicFile.musicFileData.loop;

            musicFile.audioSources[i].Play();
        }

        if (!musicFile.audioSources[0])
        {
            DebugLogger("No source avaible");
            return;
        }

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
                    //TODO
                    //Need to add a way in which the audiosource gets avaiable again! Re-added to the pool.

                    sources[i].clip = null;
                    selected = sources[i];
                    sources.RemoveAt(i);
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
