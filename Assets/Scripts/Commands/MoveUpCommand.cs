using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUpCommand : Command
{
    public override void Excecute(PlayerController playerController)
    {
        playerController.activeVerticalCommand = PlayerController.PlayerVerticalCommands.Up;
    }
}
