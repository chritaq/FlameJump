using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeProvider : IScreenShakeService
{
    private ScreenShaker screenShaker;
    private TransitionFx transitionFx;
    private ScreenFlashFx screenFlashFx;


    //Byt namn till "ReferenceFxs"
    public void GetCamera()
    {
        //camera = GameObject.FindGameObjectWithTag("MainCamera");
        screenShaker = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<ScreenShaker>();

        transitionFx = GameObject.FindGameObjectWithTag("Canvas").GetComponentInChildren<TransitionFx>();

        screenFlashFx = GameObject.FindGameObjectWithTag("Canvas").GetComponentInChildren<ScreenFlashFx>();
    }

    public void StartScreenShake(float time, float amount)
    {
        screenShaker.StartScreenShake(time, amount);
    }

    public void StartTransition(int fadeTime, bool fadeIn)
    {
        transitionFx.StartTransition(fadeTime, fadeIn);
    }

    public void StartSwipe(bool fadeIn)
    {
        transitionFx.StartSwipe(fadeIn);
    }

    public void StartScreenFlash(int time, float amount)
    {
        screenFlashFx.StartScreenFlash(time, amount);
    }
}
