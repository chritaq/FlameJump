using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandlerWithReplay : MonoBehaviour
{
    private PlayerController playerController;
    private GameObject playerPrefab;

    Queue<Command> actionInputQueue = new Queue<Command>();
    Queue<Command> horizontalInputQueue = new Queue<Command>();
    Queue<Command> verticalInputQueue = new Queue<Command>();

    //Used for the replay
    //private static List<Command> replayCommands = new List<Command>();
    //private List<float> commandTimer = new List<float>();

    private Queue<Command> recordedReplayHorizontalMovementCommands = new Queue<Command>();
    private Queue<Command> recordedReplayVerticalMovementCommands = new Queue<Command>();
    private Queue<Command> recordedReplayActionCommands = new Queue<Command>();
    private Queue<Command> replayHorizontalMovementCommands = new Queue<Command>();
    private Queue<Command> replayVerticalMovementCommands = new Queue<Command>();
    private Queue<Command> replayActionCommands = new Queue<Command>();
    private Queue<int> horizontalMovementCommandTimer = new Queue<int>();
    private Queue<int> verticalMovementCommandTimer = new Queue<int>();
    private Queue<int> actionCommandTimer = new Queue<int>();

    private Queue<Vector3> recordedTransform = new Queue<Vector3>();


    //Commands for controller
    Command zButtonCommand = new JumpCommand();
    Command zButtonHoldCommand = new JumpHoldCommand();
    Command xButtonCommand = new DashCommand();
    Command restartCommand = new RestartCommand();
    //Command bButton;
    //Command yCommand;
    //Command rtCommand;
    //Command rbCommand;
    //Command ltCommand;
    //Command lbCommand;

    //Movement Commands
    Command moveUpCommand = new MoveUpCommand();
    Command moveDownCommand = new MoveDownCommand();
    Command moveLeftCommand = new MoveLeftCommand();
    Command moveRightCommand = new MoveRightCommand();
    Command noHorizontalCommand = new NoHorizontalCommand();
    Command noVerticalCommand = new NoVerticalCommand();


    void Start()
    {
        if (playerPrefab == null)
            playerPrefab = GameObject.FindWithTag("Player");

        playerController = playerPrefab.GetComponent<PlayerController>();

        StartCoroutine("PositionRecorder");

    }

    private IEnumerator PositionRecorder()
    {
        while (!isReplaying)
        {
            recordedTransform.Enqueue(playerController.transform.position);
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    private IEnumerator PositionReplayer()
    {
        while (isReplaying && recordedTransform.Count > 0)
        {
            playerController.transform.SetPositionAndRotation(recordedTransform.Dequeue(), playerController.transform.rotation);
            yield return new WaitForEndOfFrame();
        }
        isReplaying = false;

        yield return null;
    }


    private bool isReplaying = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //Re-ad this!
            //playerController.SetRigidBodyToKinematic();
            isReplaying = true;
            restartCommand.Excecute(playerController);
            StartCoroutine("HorizontalMovementReplayer");
            StartCoroutine("VerticalMovementReplayer");
            StartCoroutine("ActionReplayer");
            StartCoroutine("PositionReplayer");
            StopCoroutine("PositionRecorder");
        }

        if (!isReplaying)
        {
            HandleActionInput();
            ExcecuteSelectedCommand(actionInputQueue, true, false);
            HandleMovementInput();
            ExcecuteSelectedCommand(horizontalInputQueue, false);
            ExcecuteSelectedCommand(verticalInputQueue, false);
        }

        if (isReplaying)
        {

            //HandleReplayActionInput();
            ExcecuteSelectedCommand(replayActionCommands, true, true);
            //HandleReplayMovementInput();
            ExcecuteSelectedCommand(replayVerticalMovementCommands, false);
            ExcecuteSelectedCommand(replayHorizontalMovementCommands, true);
        }

    }

    private void HandleActionInput()
    {

        if (Input.GetKeyDown(KeyCode.Z))
        {
            actionInputQueue.Enqueue(zButtonCommand);
            QueueReplayCommand(zButtonCommand, recordedReplayActionCommands, actionCommandTimer);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            actionInputQueue.Enqueue(zButtonHoldCommand);
            QueueReplayCommand(zButtonHoldCommand, recordedReplayActionCommands, actionCommandTimer);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            actionInputQueue.Enqueue(xButtonCommand);
            QueueReplayCommand(xButtonCommand, recordedReplayActionCommands, actionCommandTimer);
        }
        //inputQueue.Enqueue(null);
    }

    private void HandleMovementInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow))
        {
            horizontalInputQueue.Enqueue(moveRightCommand);
            QueueReplayCommand(moveRightCommand, recordedReplayHorizontalMovementCommands, horizontalMovementCommandTimer);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalInputQueue.Enqueue(moveLeftCommand);
            QueueReplayCommand(moveLeftCommand, recordedReplayHorizontalMovementCommands, horizontalMovementCommandTimer);
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalInputQueue.Enqueue(noHorizontalCommand);
            QueueReplayCommand(noHorizontalCommand, recordedReplayHorizontalMovementCommands, horizontalMovementCommandTimer);
        }



        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) && Input.GetKey(KeyCode.UpArrow))
        {
            verticalInputQueue.Enqueue(moveUpCommand);
            QueueReplayCommand(moveUpCommand, recordedReplayVerticalMovementCommands, verticalMovementCommandTimer);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow))
        {
            verticalInputQueue.Enqueue(moveDownCommand);
            QueueReplayCommand(moveDownCommand, recordedReplayVerticalMovementCommands, verticalMovementCommandTimer);
        }

        if (Input.GetKeyUp(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow))
        {
            horizontalInputQueue.Enqueue(noVerticalCommand);
            QueueReplayCommand(noVerticalCommand, recordedReplayVerticalMovementCommands, verticalMovementCommandTimer);
        }
    }


    private void ExcecuteSelectedCommand(Queue<Command> inputQueue, bool replay)
    {
        if (inputQueue.Count != 0)
        {
            Command selectedCommand = inputQueue.Dequeue();

            //For replays:
            if (!replay)
            {
                //QueueReplayCommand(selectedCommand, replayMovementCommands, movementCommandTimer);
            }


            selectedCommand.Excecute(playerController);
        }
    }

    private void QueueReplayCommand(Command command, Queue<Command> commandQueue, Queue<int> timerQueue)
    {
        commandQueue.Enqueue(command);
        timerQueue.Enqueue(Time.frameCount);
    }


    private void ExcecuteSelectedCommand(Queue<Command> inputQueue, bool actionCommand, bool replay)
    {
        if (inputQueue.Count != 0)
        {
            Command selectedCommand = inputQueue.Dequeue();

            if (!replay)
            {
                //QueueReplayCommand(selectedCommand, replayActionCommands, actionCommandTimer);
            }

            selectedCommand.Excecute(playerController);
        }
        else if (actionCommand)
        {
            playerController.activeActionCommand = PlayerController.PlayerActionCommands.Nothing;
        }
    }




    //The following code is for changing inputs

    public void SetJumpKey()
    {
        Debug.Log("Set Jump key");
        StartCoroutine(WaitForKeyPress(new JumpCommand()));
    }


    public void SetDashKey()
    {
        Debug.Log("Set Dash Key");
        StartCoroutine(WaitForKeyPress(new DashCommand()));
        //SetKeyToCommand(new DashCommand());
    }


    private IEnumerator WaitForKeyPress(Command command)
    {
        //Behövs verkligen denna? Vad används den ens till? Skapar den inte problem?
        while (Input.anyKey)
        {
            yield return new WaitForEndOfFrame();
        }

        while (!Input.anyKey)
        {
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Key pressed");

        SetKeyToCommand(command);

        yield return null;
    }


    private void SetKeyToCommand(Command command)
    {
        if (Input.GetKey(KeyCode.Z))
        {
            zButtonCommand = command;
            Debug.Log("command assigned to Z");
        }

        if (Input.GetKey(KeyCode.X))
        {
            xButtonCommand = command;
            Debug.Log("command assigned to X");
        }

        else
        {
            Debug.Log("Invalid Button");
        }
    }

    private IEnumerator HorizontalMovementReplayer()
    {
        int frameForThiscommand = 0;
        int frameForLastCommand = 0;
        int timer = 0;

        while (horizontalMovementCommandTimer.Count > 0)
        {

            frameForThiscommand = horizontalMovementCommandTimer.Peek();
            timer = frameForThiscommand - frameForLastCommand;

            while (timer > 0)
            {
                timer--;
                yield return 1;
                if (timer <= 0)
                {
                    //ExcecuteSelectedCommand(replayActionCommands, true);
                    //Command selectedCommand = recordedReplayMovementCommands.Dequeue();
                    //selectedCommand.Excecute(playerController);
                    replayHorizontalMovementCommands.Enqueue(recordedReplayHorizontalMovementCommands.Dequeue());
                }
            }
            frameForLastCommand = horizontalMovementCommandTimer.Dequeue();
        }
        yield return null;
    }

    private IEnumerator VerticalMovementReplayer()
    {
        int frameForThiscommand = 0;
        int frameForLastCommand = 0;
        int timer = 0;

        while (verticalMovementCommandTimer.Count > 0)
        {

            frameForThiscommand = verticalMovementCommandTimer.Peek();
            timer = frameForThiscommand - frameForLastCommand;

            while (timer > 0)
            {
                timer--;
                yield return 1;
                if (timer <= 0)
                {
                    //ExcecuteSelectedCommand(replayActionCommands, true);
                    //Command selectedCommand = recordedReplayMovementCommands.Dequeue();
                    //selectedCommand.Excecute(playerController);
                    replayVerticalMovementCommands.Enqueue(recordedReplayVerticalMovementCommands.Dequeue());
                }
            }
            frameForLastCommand = verticalMovementCommandTimer.Dequeue();
        }
        yield return null;
    }

    //private IEnumerator MovementReplayer()
    //{

    //    while (movementCommandTimer.Count > 0)
    //    {

    //        float timer = movementCommandTimer.Dequeue();
    //        yield return timer;
    //        Debug.Log("Movement timer is: " + timer);
    //        Command selectedCommand = replayMovementCommands.Dequeue();
    //        selectedCommand.Excecute(playerController);
    //    }

    //    yield return null;
    //}

    private IEnumerator ActionReplayer()
    {
        int frameForThiscommand = 0;
        int frameForLastCommand = 0;
        int timer = 0;
        Queue<Command> selectedCommand = new Queue<Command>();

        while (actionCommandTimer.Count > 0)
        {
            frameForThiscommand = actionCommandTimer.Peek();
            timer = frameForThiscommand - frameForLastCommand;

            while (timer > 0)
            {
                timer--;
                yield return 1;
                if (timer <= 0)
                {
                    //ExcecuteSelectedCommand(replayActionCommands, true, true);
                    //selectedCommand.Enqueue(recordedReplayActionCommands.Dequeue());
                    //selectedCommand.Excecute(playerController);

                    //ExcecuteSelectedCommand(selectedCommand, true, true);

                    replayActionCommands.Enqueue(recordedReplayActionCommands.Dequeue());

                }
            }
            frameForLastCommand = actionCommandTimer.Dequeue();
        }
        yield return null;
    }
}

