using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventWait : EventManager
{
    [SerializeField] private float timeToWait = 0f;
    public override void Trigger()
    {
        Debug.Log("Triggered");
        StartCoroutine(WaitThenTrigger());
    }

    private IEnumerator WaitThenTrigger()
    {
        yield return new WaitForSecondsRealtime(timeToWait);
        TriggerFirstChild();
        yield return null;
    }
}
