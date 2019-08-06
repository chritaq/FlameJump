using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCommand : Command
{
    public override void Excecute(PlayerController playerController)
    {
        playerController.activeActionCommand = PlayerController.PlayerActionCommands.Jump;
    }
}
