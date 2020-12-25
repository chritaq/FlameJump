using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public virtual void Trigger()
    {
        TriggerFirstChild();
    }

    public void TriggerFirstChild()
    {
        Transform firstActiveGameObject;

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).gameObject.activeSelf == true)
            {
                firstActiveGameObject = gameObject.transform.GetChild(i);
                if(firstActiveGameObject.GetComponent<EventManager>() != null)
                {
                    firstActiveGameObject.GetComponent<EventManager>().Trigger();
                    break;
                }

            }
        }
    }
}
