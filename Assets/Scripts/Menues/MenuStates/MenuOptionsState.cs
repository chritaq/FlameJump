using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOptionsState : MenuState
{
    public override void Enter(MenuController menuController)
    {
        menuController.SetAboveState(new MenuMainState());
        menuController.optionsMenuObject.SetActive(true);
    }

    public override void Exit(MenuController menuController)
    {
        menuController.optionsMenuObject.SetActive(false);
    }

    public override MenuState Update(MenuController menuController, float t)
    {
        return null;
    }
}
