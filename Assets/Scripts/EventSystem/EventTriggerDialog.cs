using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTriggerDialog : EventManager
{
    [SerializeField] private NPCControllerV2 npc;

    public override void Trigger()
    {
        npc.StartCutsceneDialog();
        base.Trigger();
    }
}
