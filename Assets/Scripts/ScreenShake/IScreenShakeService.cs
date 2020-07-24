using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScreenShakeService
{
    void GetCamera();
    void StartScreenShake(float time, float amount);
    void StartTransition(float fadeTime, bool fadeIn);
    void StartScreenFlash(float time, float amount);
    void StartSwipe(bool fadeIn);
}
