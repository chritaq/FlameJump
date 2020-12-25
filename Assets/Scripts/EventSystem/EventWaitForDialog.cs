using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventWaitForDialog : EventManager
{

    public override void Trigger()
    {
        StartCoroutine(WaitForDialog());
        
    }

    private IEnumerator WaitForDialog()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        while(DialougeManagerV2.instance.CheckInDialouge())
        {
            yield return new WaitForEndOfFrame();
        }
        base.Trigger();
    }
}
