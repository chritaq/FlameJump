using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            //ServiceLocator.GetAudio().PlayMusic("Music_Gameplay01");
            ServiceLocator.GetTimeManagement().SlowDown(0.01f, true);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ServiceLocator.GetTimeManagement().PauseTime();

        }
    }
}
