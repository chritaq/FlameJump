using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuContinueState : MenuState
{
    private float transitionTime;

    public override void Enter(MenuController menuController)
    {
        transitionTime = menuController.longMenuTransitionTime;
        //Need to disable controls
        Debug.Log("Continue Pressed");
    }

    public override void Exit(MenuController menuController)
    {

    }

    public override MenuState Update(MenuController menuController, float t)
    {
        transitionTime -= t;
        if (transitionTime <= 0)
        {
            //SceneManager.LoadScene(Last Played Scene);
            //Debug.Log("Game started");
        }
        return null;
    }
}
