using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullTimeManagementProvider : ITimeManagementService
{
    public void ChangeTime(float timeScale, float rate)
    {
    }

    public void ChangeTime(float timeScale, float rate, float step)
    {
    }

    public void InstantiateTimeManagement()
    {
    }

    public void PauseTime()
    {
    }

    public void ResetTimescale()
    {
    }

    public void SetTimeScale(float timeScale)
    {
    }

    public void SlowDown(float speedUpRate, bool unscaled)
    {
    }

    public void StopTimeforRealTimeSeconds(float timeToStop)
    {
        throw new System.NotImplementedException();
    }
}
