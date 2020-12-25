using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSetCutsceneState : EventManager
{
    [SerializeField] private bool toggle = true;

    public override void Trigger()
    {
        if(toggle)
        {
            PlayerController.playerInstance.GoToCutsceneState();
        }
        else
        {
            PlayerController.playerInstance.GoToIdleState();
        }
        base.Trigger();
    }
}
