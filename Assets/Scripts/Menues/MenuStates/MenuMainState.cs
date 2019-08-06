using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMainState : MenuState
{
    public override void Enter(MenuController menuController)
    {
        menuController.SetAboveState(null);
        menuController.mainMenuObject.SetActive(true);
    }

    public override void Exit(MenuController menuController)
    {
        menuController.mainMenuObject.SetActive(false);
    }

    public override MenuState Update(MenuController menuController, float t)
    {
        return null;
    }
}
