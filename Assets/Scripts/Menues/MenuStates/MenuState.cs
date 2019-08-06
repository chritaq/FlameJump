using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuState
{
    public abstract void Enter(MenuController menuController);
    public abstract void Exit(MenuController menuController);
    public abstract MenuState Update(MenuController menuController, float t);

    //A or start, move to next state. B move to former state.
    //public abstract MenuState HandleInput(KeyCode key = KeyCode.None, EventType eventType = EventType.Ignore);


    //public abstract MenuState ChooseNextState(MenuController menuController);
}
