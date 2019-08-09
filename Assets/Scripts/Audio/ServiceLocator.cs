using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static IAudioService audio;
    private static IScreenShakeService screenShake;

    public static void Initialize()
    {
        audio = new NullAudioProvider();
        screenShake = new NullScreenShakeProvider();
    }

    public static void ProvideAudio(IAudioService audioService)
    {
        if (audioService == null)
        {
            audioService = new NullAudioProvider();
        }

        audio = audioService;
    }

    public static void ProvideScreenShake(IScreenShakeService screenShakeService)
    {
        if(screenShakeService == null)
        {
            screenShakeService = new NullScreenShakeProvider();
        }

        screenShake = screenShakeService;
    }

    public static IAudioService GetAudio()
    {
        return audio;
    }

    public static IScreenShakeService GetScreenShake()
    {
        return screenShake;
    }

}
