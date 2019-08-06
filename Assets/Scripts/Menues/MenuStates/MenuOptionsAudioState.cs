using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOptionsAudioState : MenuState
{
    public override void Enter(MenuController menuController)
    {
        menuController.SetAboveState(new MenuOptionsState());
        menuController.audioMenuObject.SetActive(true);
    }

    public override void Exit(MenuController menuController)
    {
        menuController.audioMenuObject.SetActive(false);
    }

    public override MenuState Update(MenuController menuController, float t)
    {
        return null;
    }
}
