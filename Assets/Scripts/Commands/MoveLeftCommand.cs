using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeftCommand : Command
{
    public override void Excecute(PlayerController playerController)
    {
        playerController.activeHorizontalCommand = PlayerController.PlayerHorizontalCommands.Left;
    }
}
