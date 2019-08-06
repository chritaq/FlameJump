using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountingSystem : Observer
{

    private void Start()
    {
        //Used for testing:
        //PlayerPrefs.DeleteAll();

        //Ändra detta till spelare istället?
        foreach (var poi in FindObjectsOfType<Unit>())
        {
            poi.AddObserver(this);
        }
    }

    public override void OnNotify(object counterName, NotificationType notificationType)
    {
        if (notificationType == NotificationType.Counter)
        {
            string counterKey = "counter-" + counterName;

            int counter = PlayerPrefs.GetInt(counterKey) + 1;;

            //Sets the playerpreferences of the counter to the value. Ex: Jump to 15 if you've jumped 15 times.
            PlayerPrefs.SetInt(counterKey, counter);

            if(counter == 10 || counter == 100 || counter == 1000 || counter == 10000 || counter == 100000)
                Debug.Log(counterName + " is set to " + counter);
        }
    }
}
