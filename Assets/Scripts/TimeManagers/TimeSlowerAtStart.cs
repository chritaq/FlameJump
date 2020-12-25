using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSlowerAtStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Slowdown", 0.5f);
    }

    void Slowdown()
    {
        ServiceLocator.GetTimeManagement().SlowDown(0.001f, true);
    }
}
