using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeProvider : IScreenShakeService
{
    private GameObject camera;
    private ScreenShaker screenShaker;

    public void GetCamera()
    {
        //camera = GameObject.FindGameObjectWithTag("MainCamera");
        screenShaker = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<ScreenShaker>();
    }

    public void StartScreenShake(float time, float amount)
    {
        screenShaker.StartScreenShake(time, amount);
    }
}
