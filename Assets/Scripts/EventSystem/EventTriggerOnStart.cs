using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTriggerOnStart : EventManager
{
    public EventManager[] eventsToTrigger;

    void Start()
    {
        for(int i = 0; i < eventsToTrigger.Length; i++)
        {
            eventsToTrigger[i].Trigger();
        }

        TriggerFirstChild();
    }
}
