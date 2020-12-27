using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutMusic : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void StartFadeOut(AudioSource[] audioSources)
    {
        for(int i = 0; i < audioSources.Length; i++)
        {
            StartCoroutine(FadeOut(audioSources[i]));
            //audioSources[i].Stop();
        }
        //Destroy(this, 5);
    }

    private float fadeOutStep = 0.01f;
    private IEnumerator FadeOut(AudioSource audioSource)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= fadeOutStep;
            yield return new WaitForSecondsRealtime(0.01f);
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
        yield return null;
    }
}
