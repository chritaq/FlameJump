using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialougeTrigger : MonoBehaviour
{
    [SerializeField] private Dialouge dialouge;

    public void TriggerDialouge()
    {
        DialougeManagerV2.instance.StartDialouge(dialouge);
    }
    
}
