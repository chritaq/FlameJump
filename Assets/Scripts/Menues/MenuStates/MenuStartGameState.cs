using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuStartGameState : MenuState
{
    public override void Enter(MenuController menuController)
    {
        Debug.Log("Entered menustargame state");
        menuController.SetAboveState(new MenuMainState());
        menuController.startGameMenuObject.SetActive(true);
    }

    public override void Exit(MenuController menuController)
    {
        menuController.startGameMenuObject.SetActive(false);
    }

    public override MenuState Update(MenuController menuController, float t)
    {
        return null;
    }

    
}
