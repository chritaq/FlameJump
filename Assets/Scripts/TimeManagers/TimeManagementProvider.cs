using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManagementProvider : MonoBehaviour, ITimeManagementService
{
    private float unPausedTime = Time.timeScale;
    private TimeManagement timeManagement;



    public void PauseTime()
    {
        Debug.Log("tried to pause");
        if (Time.timeScale > 0)
        {
            unPausedTime = Time.timeScale;

            Debug.Log("Timescale set to 0");
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = unPausedTime;
        }
    }

    public void StopTimeforRealTimeSeconds(float timeToStop)
    {
        timeManagement.StopTimeForRealTimeSeconds(timeToStop);
    }

    public void ResetTimescale()
    {
        timeManagement.StopActiveCoroutine();
        Time.timeScale = 1;
    }

    public void SetTimeScale(float timeScale)
    {
        timeManagement.StopActiveCoroutine();
        Time.timeScale = timeScale;
    }

    public void SlowDown(float speedUpRate, bool unscaled)
    {
        timeManagement.SlowDown(speedUpRate, unscaled);
    }


    public void ChangeTime(float newtimeScale, float rate)
    {

    }

    public void ChangeTime(float newTimeScale, float rate, float step)
    {

    }

    public void InstantiateTimeManagement()
    {
        timeManagement = Instantiate(new GameObject().AddComponent<TimeManagement>());
        Object.DontDestroyOnLoad(timeManagement.gameObject);
    }
}
