using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITimeManagementService
{
    void InstantiateTimeManagement();
    void ChangeTime(float timeScale, float rate);
    void ChangeTime(float timeScale, float rate, float step);
    void SetTimeScale(float timeScale);
    void ResetTimescale();
    void PauseTime();
    void SlowDown(float speedUpRate, bool unscaled);
}
