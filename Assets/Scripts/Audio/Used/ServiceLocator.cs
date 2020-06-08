using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static IAudioService audio;
    private static IScreenShakeService screenShake;
    private static IGamepadRumbleService gamepadRumble;
    private static ITimeManagementService timeManagement;

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

    public static void ProvideGamepadRumble(IGamepadRumbleService gamepadRumbleService)
    {
        if(gamepadRumbleService == null)
        {
            gamepadRumbleService = new NullGamepadRumbleProvider();
        }

        gamepadRumble = gamepadRumbleService;
    }

    public static void ProvideTimeManagement(ITimeManagementService timeManagementService)
    {
        if (timeManagementService == null)
        {
            timeManagementService = new NullTimeManagementProvider();
        }

        timeManagement = timeManagementService;
    }

    public static IAudioService GetAudio()
    {
        return audio;
    }

    public static IScreenShakeService GetScreenShake()
    {
        return screenShake;
    }

    public static IGamepadRumbleService GetGamepadRumble()
    {
        return gamepadRumble;
    }

    public static ITimeManagementService GetTimeManagement()
    {
        return timeManagement;
    }

}
