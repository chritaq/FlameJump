using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventToggleGameObject : EventManager
{
    [SerializeField] private GameObject[] gameObjects;
    private bool toggle = true;

    public override void Trigger()
    {
        for(int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].SetActive(true);
        }
        base.Trigger();
    }
}
