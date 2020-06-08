using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDownCommand : Command
{
    public override void Excecute(PlayerController playerController)
    {
        playerController.activeVerticalCommand = PlayerController.PlayerVerticalCommands.Down;
    }
}
