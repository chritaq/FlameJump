using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScreenShakeService
{
    void GetCamera();
    void StartScreenShake(float time, float amount);
    void StartTransition(int fadeTime, bool fadeIn);
    void StartScreenFlash(int time, float amount);
}
