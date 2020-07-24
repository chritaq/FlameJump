using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManagement : MonoBehaviour
{
    private Coroutine timeChangerCoroutine;

    public void SlowDown(float speedUpRate, bool unscaled)
    {
        StopActiveCoroutine();
        timeChangerCoroutine = StartCoroutine(TimeSlower(speedUpRate, unscaled));
    }

    private IEnumerator TimeSlower(float speedUpRate, bool unscaled)
    {
        Time.timeScale = 0;
        while (Time.timeScale < 1)
        {
            Time.timeScale += 0.01f;
            if (unscaled)
            {
                yield return new WaitForSecondsRealtime(speedUpRate);
            }
            else
            {
                yield return new WaitForSeconds(speedUpRate);
            }
        }

        Time.timeScale = 1;
    }

    public void StopTimeForRealTimeSeconds(float timeToStop)
    {
        StopActiveCoroutine();
        timeChangerCoroutine = StartCoroutine(TimeStopper(timeToStop));
    }

    private IEnumerator TimeStopper(float timeToStop)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(timeToStop);
        Time.timeScale = 1;
        yield return null;
    }

    public void StopActiveCoroutine()
    {
        if (timeChangerCoroutine != null)
        {
            StopCoroutine(timeChangerCoroutine);
        }
    }
}
