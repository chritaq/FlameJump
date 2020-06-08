using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartCommand : Command
{
    public override void Excecute(PlayerController playerController)
    {
        playerController.transform.position = playerController.GetSpawnPosition();
    }
}
