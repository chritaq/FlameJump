using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManagementProvider : MonoBehaviour, ITimeManagementService
{
    private Coroutine timeCoroutine;
    private float unPausedTime;

    public void PauseTime(bool pause)
    {
        if (Time.timeScale != 0)
            unPausedTime = Time.timeScale;

        Time.timeScale = 0;
    }

    public void ResetTimescale()
    {
        Time.timeScale = 1;
    }

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    public void ChangeTime(float newtimeScale, float rate)
    {

    }

    public void ChangeTime(float newTimeScale, float rate, float step)
    {

    }

    //private IEnumerator TimeChanger(float timeScale, float rate, float step)
    //{
    //    bool AutomateDown;
    //    if(Time.timeScale < )

    //    yield return null;
    //}

    //private IEnumerator TimeChanger(float newTimeScale, float rate)
    //{
    //    if (Time.timeScale > newTimeScale)
    //        slowDownTime = true;
    //    else if(Time.timeScale < newTimeScale)
    //    {
    //        slowDownTime = false;
    //    }
    //}

    private void StopActiveCoroutine()
    {
        if(timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
        }
    }
}
