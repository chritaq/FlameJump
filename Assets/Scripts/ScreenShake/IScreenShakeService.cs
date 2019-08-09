using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScreenShakeService
{
    void GetCamera();
    void StartScreenShake(float time, float amount);
}
