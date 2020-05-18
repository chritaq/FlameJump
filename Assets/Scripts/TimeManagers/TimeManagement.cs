using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManagement : MonoBehaviour
{
    private IEnumerator timeChangerCoroutine;

    public void SlowDown(float speedUpRate, bool unscaled)
    {
        StopActiveCoroutine();
        timeChangerCoroutine = TimeSlower(speedUpRate, unscaled);
        StartCoroutine(timeChangerCoroutine);
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

    public void StopActiveCoroutine()
    {
        if (timeChangerCoroutine != null)
        {
            StopCoroutine(timeChangerCoroutine);
        }
    }
}
