using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOptionsControllerState : MenuState
{
    public override void Enter(MenuController menuController)
    {
        menuController.SetAboveState(new MenuOptionsState());
        menuController.controllersMenuObject.SetActive(true);
    }

    public override void Exit(MenuController menuController)
    {
        menuController.controllersMenuObject.SetActive(false);
    }

    public override MenuState Update(MenuController menuController, float t)
    {
        return null;
    }
}
