using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class InputHandler : MonoBehaviour
{
    private PlayerController playerController;
    private GameObject playerPrefab;
    [SerializeField] private bool keyBoard;

    Queue<Command> actionInputQueue = new Queue<Command>();
    Queue<Command> horizontalInputQueue = new Queue<Command>();
    Queue<Command> verticalInputQueue = new Queue<Command>();


    //Commands for controller
    Command aButtonCommand = new JumpCommand();
    Command aButtonHoldCommand = new JumpHoldCommand();
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

    PlayerIndex player;
    GamePadState state;
    private Vector2 stickInput;

    void Start()
    {
        if (playerPrefab == null)
            playerPrefab = GameObject.FindWithTag("Player");

        playerController = playerPrefab.GetComponent<PlayerController>();

        player = PlayerIndex.One;

    }


    private void Update()
    {
        //if(Input.anyKey)
        //{
        //    keyBoard = true;
        //}
        //if(state.Buttons.Start == ButtonState.Pressed)
        //{
        //    keyBoard = false;
        //}

        state = GamePad.GetState(player, GamePadDeadZone.Circular);

        HandleActionInput();
        ExcecuteSelectedCommand(actionInputQueue, true, false);
        if(keyBoard)
        {
            HandleKeyboardMovementInput();
        }
        else
        {
            HandleXboxMovementInput();
        }
        ExcecuteSelectedCommand(horizontalInputQueue, false);
        ExcecuteSelectedCommand(verticalInputQueue, false);

        
    }


    private bool aHasBeenPressed = false;
    private bool xHasBeenPressed = false;

    private void HandleActionInput()
    {

        if (Input.GetKeyDown(KeyCode.Z) || state.Buttons.A == ButtonState.Pressed && !aHasBeenPressed)
        {
            actionInputQueue.Enqueue(aButtonCommand);
            aHasBeenPressed = true;
        }

        else if(state.Buttons.A == ButtonState.Released) {
            aHasBeenPressed = false;
        }


        if (Input.GetKey(KeyCode.Z) || state.Buttons.A == ButtonState.Pressed)
        {
            actionInputQueue.Enqueue(aButtonHoldCommand);
        }

        if (Input.GetKeyDown(KeyCode.X) || state.Buttons.X == ButtonState.Pressed && !xHasBeenPressed)
        {
            actionInputQueue.Enqueue(xButtonCommand);
            xHasBeenPressed = true;
        }
        else if (state.Buttons.X == ButtonState.Released)
        {
            xHasBeenPressed = false;
        }
        //inputQueue.Enqueue(null);
    }

    private void HandleKeyboardMovementInput()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow))
        {
            horizontalInputQueue.Enqueue(moveRightCommand);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalInputQueue.Enqueue(moveLeftCommand);
        }

        if(Input.GetKeyUp(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalInputQueue.Enqueue(noHorizontalCommand);
        }


        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) && Input.GetKey(KeyCode.UpArrow))
        {
            verticalInputQueue.Enqueue(moveUpCommand);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow))
        {
            verticalInputQueue.Enqueue(moveDownCommand);
        }

        if (Input.GetKeyUp(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow))
        {
            horizontalInputQueue.Enqueue(noVerticalCommand);
        }
    }

    [SerializeField] private float horizontalDeadZone = 0.5f;
    [SerializeField] private float verticalDeadZone = 0.5f;
    private void HandleXboxMovementInput()
    {
        if(state.ThumbSticks.Left.X > horizontalDeadZone)
        {
            horizontalInputQueue.Enqueue(moveRightCommand);
        }
        else if(state.ThumbSticks.Left.X < -horizontalDeadZone)
        {
            horizontalInputQueue.Enqueue(moveLeftCommand);
        }
        else
        {
            horizontalInputQueue.Enqueue(noHorizontalCommand);
        }

        if (state.ThumbSticks.Left.Y > verticalDeadZone)
        {
            verticalInputQueue.Enqueue(moveUpCommand);
        }
        else if (state.ThumbSticks.Left.Y < -verticalDeadZone)
        {
            verticalInputQueue.Enqueue(moveDownCommand);
        }
        else
        {
            verticalInputQueue.Enqueue(noVerticalCommand);
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
        else if(actionCommand)
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
            aButtonCommand = command;
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
}
