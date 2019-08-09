using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNewGameState : MenuState
{
    private float transitionTime;

    public override void Enter(MenuController menuController)
    {
        transitionTime = menuController.longMenuTransitionTime;
        //Need to disable controls
        Debug.Log("New Game Pressed");
    }

    public override void Exit(MenuController menuController)
    {

    }

    public override MenuState Update(MenuController menuController, float t)
    {
        transitionTime -= t;
        if (transitionTime <= 0)
        {
            //Behöver fixa ett snyggare upplägg för scenemanagern och dessutom få in Continue/New Game
            SceneManager.LoadScene(1);
            Debug.Log("Game started");
        }
        return null;
    }
}
