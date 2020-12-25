using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTriggerAnimation : EventManager
{
    [SerializeField] private Animator animator;
    [SerializeField] private string triggerToSet;
    public override void Trigger()
    {
        animator.SetTrigger(triggerToSet);
        base.Trigger();
    }
}
