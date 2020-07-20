using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractCommand : Command
{
    public override void Excecute(PlayerController playerController)
    {
        playerController.activeMiscCommand = PlayerController.PlayerMiscCommands.Dialouge;
    }
}
