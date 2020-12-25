using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private EventManager[] eventsToTrigger;

    public void SlowDown(float slowDownTime)
    {
        ServiceLocator.GetTimeManagement().SlowDown(slowDownTime, true);
    }

    public void ScreenShakeBig(float screenShakeTime)
    {
        ServiceLocator.GetScreenShake().StartScreenShake(screenShakeTime, 1f);
    }

    public void ScreenShakeSmall(float screenShakeTime)
    {
        ServiceLocator.GetScreenShake().StartScreenShake(screenShakeTime, 0.4f);
    }

    public void ScreenFlash(float screenFlashTime)
    {
        ServiceLocator.GetScreenShake().StartScreenFlash(screenFlashTime, 1);
    }

    public void PlayAudio(string audioToPlay)
    {
        ServiceLocator.GetAudio().PlaySound(audioToPlay, SoundType.normal);
    }

    public void TriggerEvent(int triggerEventIndex)
    {
        eventsToTrigger[triggerEventIndex].Trigger();
    }
}
