using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementSystem : Observer
{
    private void Start()
    {
        //TODO
        //Remove this
        //Used for testing:
        PlayerPrefs.DeleteAll();

        foreach (var pointOfInterest in FindObjectsOfType<PointOfInterest>())
        {
            pointOfInterest.AddObserver(this);
        }
    }

    public override void OnNotify(object achivementName, NotificationType notificationType)
    {
        if(notificationType == NotificationType.AchivementUnlocked)
        {
            string achivementKey = "achivement-" + achivementName;

            if(PlayerPrefs.GetInt(achivementKey) == 1)
            {
                return;
            }

            PlayerPrefs.SetInt(achivementKey, 1);
            Debug.Log("Unlocked" + achivementName);
        }
    }
}