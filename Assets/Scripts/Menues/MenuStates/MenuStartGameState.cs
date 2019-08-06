using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuStartGameState : MenuState
{
    private float transitionTime;

    public override void Enter(MenuController menuController)
    {
        transitionTime = menuController.longMenuTransitionTime;
        //Need to disable controls
        Debug.Log("Start Game pressed");
    }

    public override void Exit(MenuController menuController)
    {

    }

    public override MenuState Update(MenuController menuController, float t)
    {
        transitionTime -= t;
        if(transitionTime <= 0)
        {
            //Change scene
            Debug.Log("Game started");
        }
        return null;
    }

    
}
