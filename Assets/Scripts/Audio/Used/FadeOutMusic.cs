using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutMusic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartFadeOut(AudioSource[] audioSources)
    {
        for(int i = 0; i < audioSources.Length; i++)
        {
            StartCoroutine(FadeOut(audioSources[i]));
        }

    }

    private float fadeRate = 0.5f;
    private IEnumerator FadeOut(AudioSource audioSource)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime * fadeRate;
            yield return new WaitForEndOfFrame();
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
        Destroy(this, 0.1f);
        yield return null;
    }
}
