using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            ServiceLocator.GetAudio().PlayMusic("Music_Gameplay01");
        }
    }
}
