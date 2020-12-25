using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventMoveGameObject : EventManager
{
    [SerializeField] private GameObject gameObjectToMove;
    [SerializeField] private GameObject positionToMoveTo;
    public override void Trigger()
    {
        gameObjectToMove.transform.position = positionToMoveTo.transform.position;
        base.Trigger();
    }
}
