﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNewGameState : MenuState
{
    private float transitionTime;

    public override void Enter(MenuController menuController)
    {
        transitionTime = menuController.longMenuTransitionTime;

        ServiceLocator.GetAudio().PlaySound("Menu_StartGame", SoundType.normal);
        //ServiceLocator.GetScreenShake().StartScreenShake(transitionTime, 1);
        ServiceLocator.GetScreenShake().StartTransition((int)transitionTime, true);
        ServiceLocator.GetGamepadRumble().StartGamepadRumble(GamepadRumbleProvider.RumbleSize.huge);
        ServiceLocator.GetScreenShake().StartScreenFlash(0.05f, 1);

        //Need to disable controls
        ServiceLocator.GetAudio().PlaySound("Menu_StartChing", SoundType.menuSound);
        Debug.Log("New Game Pressed");
    }

    public override void Exit(MenuController menuController)
    {

    }

    public override MenuState Update(MenuController menuController, float t)
    {
        //transitionTime -= t;
        transitionTime--;
        if (transitionTime <= 0)
        {
            //Behöver fixa ett snyggare upplägg för scenemanagern och dessutom få in Continue/New Game
            SceneManager.LoadScene(1);
            Debug.Log("Game started");
        }
        return null;
    }
}
