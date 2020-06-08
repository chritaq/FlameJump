using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitCommand : Command
{
    public override void Excecute(PlayerController playerController)
    {
        Debug.Log("Works");
        playerController.activeActionCommand = PlayerController.PlayerActionCommands.Exit;
    }

}
