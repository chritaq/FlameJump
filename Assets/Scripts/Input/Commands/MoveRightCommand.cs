using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRightCommand : Command
{
    public override void Excecute(PlayerController playerController)
    {
        playerController.activeHorizontalCommand = PlayerController.PlayerHorizontalCommands.Right;
    }
}
