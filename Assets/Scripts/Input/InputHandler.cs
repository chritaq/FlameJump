using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class InputHandler : MonoBehaviour
{
    
    private PlayerController playerController;
    private GameObject playerPrefab;
    [SerializeField] private bool keyboard;

    private Queue<Command> actionInputQueue = new Queue<Command>();
    private Queue<Command> horizontalInputQueue = new Queue<Command>();
    private Queue<Command> verticalInputQueue = new Queue<Command>();
    private Queue<Command> miscCommandQueue = new Queue<Command>();


    //GAMEPAD COMMANDS
    private Command aButtonCommand = new JumpCommand();
    private Command aButtonHoldCommand = new JumpHoldCommand();
    private Command xButtonCommand = new DashCommand();
    private Command xButtonHoldCommand = new NullCommand();
    private Command bButtonCommand = new NullCommand();
    private Command bButtonHoldCommand = new NullCommand();
    private Command yButtonCommand = new NullCommand();
    private Command yButtonHoldCommand = new NullCommand();
    private Command leftShoulderButtonCommand = new NullCommand();
    private Command leftShoulderButtonHoldCommand = new NullCommand();
    private Command rightShoulderButtonCommand = new NullCommand();
    private Command rightShoulderButtonHoldCommand = new NullCommand();

    //Borde ändras
    private Command restartCommand = new RestartCommand();

    //KEYBOARD KEYCODES AND COMMANDS
    private KeyCode jumpKey = KeyCode.Z;
    private KeyCode dashKey = KeyCode.X;
    private KeyCode upKey = KeyCode.UpArrow;
    private KeyCode downKey = KeyCode.DownArrow;
    private KeyCode leftKey = KeyCode.LeftArrow;
    private KeyCode rightKey = KeyCode.RightArrow;

    private Command jumpCommand = new JumpCommand();
    private Command jumpHoldCommand = new JumpHoldCommand();
    private Command dashCommand = new DashCommand();


    //Movement Commands (Både för keyboard och gamepad)
    private Command moveUpCommand = new MoveUpCommand();
    private Command moveDownCommand = new MoveDownCommand();
    private Command moveLeftCommand = new MoveLeftCommand();
    private Command moveRightCommand = new MoveRightCommand();
    private Command noHorizontalCommand = new NoHorizontalCommand();
    private Command noVerticalCommand = new NoVerticalCommand();


    //Misc Commands
    private Command interactCommand = new InteractCommand();

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

    private void OnLevelWasLoaded(int level)
    {
        playerPrefab = GameObject.FindWithTag("Player");
        playerController = playerPrefab.GetComponent<PlayerController>();
        
        
    }


    private void Update()
    {

        if (playerController.activeActionCommand != PlayerController.PlayerActionCommands.Exit)
        {
            state = GamePad.GetState(player, GamePadDeadZone.Circular);

            ToggleKeyboardAndGamepad();

            if (keyboard)
            {
                HandleKeyboardActionInput();
                HandleKeyboardMovementInput();
                HandleKeyboardMiscInput();
            }
            else
            {
                HandleGamepadActionInputs();
                HandleGamepadMovementInput();
                HandleGamepadMiscInput();
            }

            ExcecuteSelectedCommand(actionInputQueue, CommandType.actionCommand);
            ExcecuteSelectedCommand(horizontalInputQueue);
            ExcecuteSelectedCommand(verticalInputQueue);
            ExcecuteSelectedCommand(miscCommandQueue, CommandType.miscCommand);
        }

        
    }


    //Gör om så den kollar alla knappar? if input.anyKey först och sen den långa checken??
    private void ToggleKeyboardAndGamepad()
    {
        if (Input.GetKeyDown(jumpKey))
        {
            keyboard = true;
        }

        if (state.Buttons.Start == ButtonState.Pressed)
        {
            keyboard = false;
        }
    }


    private void HandleKeyboardActionInput()
    {

        if (Input.GetKeyDown(jumpKey))
        {
            actionInputQueue.Enqueue(jumpCommand);

        }

        if (Input.GetKey(jumpKey))
        {
            actionInputQueue.Enqueue(jumpHoldCommand);
        }

        if (Input.GetKeyDown(dashKey))
        {
            actionInputQueue.Enqueue(dashCommand);
        }
        
    }


    //Bools som används för att skilja mellan getKey och getKeyDown på gamepad, då detta inte fanns inbyggt.
    private bool aHasBeenPressed = false;
    private bool bHasBeenPressed = false;
    private bool xHasBeenPressed = false;
    private bool yHasBeenPressed = false;
    private bool rightShoulderHasBeenPressed = false;
    private bool leftShoulderHasBeenPressed = false;

    private void HandleGamepadActionInputs()
    {
        aHasBeenPressed = CheckButtonPressAndHold(state.Buttons.A, aHasBeenPressed, aButtonCommand, aButtonHoldCommand);
        xHasBeenPressed = CheckButtonPressAndHold(state.Buttons.X, xHasBeenPressed, xButtonCommand, xButtonHoldCommand);
        yHasBeenPressed = CheckButtonPressAndHold(state.Buttons.Y, yHasBeenPressed, yButtonCommand, yButtonHoldCommand);
        bHasBeenPressed = CheckButtonPressAndHold(state.Buttons.B, bHasBeenPressed, bButtonCommand, bButtonHoldCommand);
        rightShoulderHasBeenPressed = CheckButtonPressAndHold(state.Buttons.RightShoulder, rightShoulderHasBeenPressed, rightShoulderButtonCommand, rightShoulderButtonHoldCommand);
        leftShoulderHasBeenPressed = CheckButtonPressAndHold(state.Buttons.LeftShoulder, leftShoulderHasBeenPressed, leftShoulderButtonCommand, leftShoulderButtonHoldCommand);
    }

    private bool CheckButtonPressAndHold(ButtonState button, bool hasBeenPressed, Command buttonCommand, Command buttonHoldCommand)
    {
        if (button == ButtonState.Pressed && !hasBeenPressed)
        {
            actionInputQueue.Enqueue(buttonCommand);
            hasBeenPressed = true;
        }

        else if (button == ButtonState.Released)
        {
            hasBeenPressed = false;
        }

        if (button == ButtonState.Pressed)
        {
            actionInputQueue.Enqueue(buttonHoldCommand);
        }

        return hasBeenPressed;
    }


    private void HandleKeyboardMovementInput()
    {
        if (Input.GetKeyDown(rightKey) || Input.GetKeyUp(leftKey) && Input.GetKey(rightKey))
        {
            horizontalInputQueue.Enqueue(moveRightCommand);
        }

        if (Input.GetKeyDown(leftKey) || Input.GetKeyUp(rightKey) && Input.GetKey(leftKey))
        {
            horizontalInputQueue.Enqueue(moveLeftCommand);
        }

        if (Input.GetKeyUp(leftKey) && !Input.GetKey(rightKey) || Input.GetKeyUp(rightKey) && !Input.GetKey(leftKey))
        {
            horizontalInputQueue.Enqueue(noHorizontalCommand);
        }


        if (Input.GetKeyDown(upKey) || Input.GetKeyUp(downKey) && Input.GetKey(upKey))
        {
            verticalInputQueue.Enqueue(moveUpCommand);
        }

        if (Input.GetKeyDown(downKey) || Input.GetKeyUp(upKey) && Input.GetKey(downKey))
        {
            verticalInputQueue.Enqueue(moveDownCommand);
        }

        if (Input.GetKeyUp(upKey) && !Input.GetKey(downKey) || Input.GetKeyUp(downKey) && !Input.GetKey(upKey))
        {
            horizontalInputQueue.Enqueue(noVerticalCommand);
        }
    }


    [SerializeField] private float horizontalDeadZone = 0.5f;
    [SerializeField] private float verticalDeadZone = 0.5f;
    private void HandleGamepadMovementInput()
    {
        if (state.ThumbSticks.Left.X > horizontalDeadZone)
        {
            horizontalInputQueue.Enqueue(moveRightCommand);
        }
        else if (state.ThumbSticks.Left.X < -horizontalDeadZone)
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

    private void HandleKeyboardMiscInput()
    {
        if (Input.GetKeyDown(upKey))
        {
            miscCommandQueue.Enqueue(interactCommand);
            //miscCommand = interactCommand;
            //verticalInputQueue.Enqueue(moveUpCommand);
        }
    }

    private void HandleGamepadMiscInput()
    {
        if (state.ThumbSticks.Left.Y > verticalDeadZone)
        {
            miscCommandQueue.Enqueue(interactCommand);
            //verticalInputQueue.Enqueue(moveUpCommand);
        }
    }


    private enum CommandType
    {
        verticalCommand,
        horizontalCommand,
        actionCommand,
        miscCommand
    }

    private void ExcecuteSelectedCommand(Queue<Command> inputQueue)
    {
        if (inputQueue.Count > 0)
        {
            Command selectedCommand = inputQueue.Dequeue();
            selectedCommand.Excecute(playerController);
        }
    }

    private void ExcecuteSelectedCommand(Queue<Command> inputQueue, CommandType commandTypeToExcecute)
    {
        if (inputQueue.Count == 0)
        {
            if(commandTypeToExcecute == CommandType.actionCommand)
            {
                playerController.activeActionCommand = PlayerController.PlayerActionCommands.Nothing;
            }
            else if(commandTypeToExcecute == CommandType.miscCommand)
            {
                playerController.activeMiscCommand = PlayerController.PlayerMiscCommands.Nothing;
            }
            
        }
        else if(inputQueue.Count > 0)
        {
            ExcecuteSelectedCommand(inputQueue);
        }
    }



    //REMAP BUTTONS

    //The following code is for changing gamepad inputs:
    public void SetGamepadCommandToPressedButton(Command command)
    {
        StartCoroutine(WaitForKeyPress(command));
    }

    private IEnumerator WaitForKeyPress(Command command)
    {
        while (Input.anyKey)
        {
            yield return new WaitForEndOfFrame();
        }

        while (!Input.anyKey)
        {
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Key pressed");

        SetButtonToCommand(command);

        yield return null;
    }

    private void SetButtonToCommand(Command command)
    {
        if (state.Buttons.A == ButtonState.Pressed)
        {
            aButtonCommand = command;
            Debug.Log("command assigned to A");
        }

        if (state.Buttons.B == ButtonState.Pressed)
        {
            xButtonCommand = command;
            Debug.Log("command assigned to X");
        }

        else
        {
            Debug.Log("Invalid Button");
        }
    }


    //The following code is for changing keyboard inputs:
    public void ChangeKeyboardKey(KeyCode keyToChange)
    {
        StartCoroutine(WaitForKeyPress(keyToChange));
    }

    private IEnumerator WaitForKeyPress(KeyCode keyToChange)
    {
        while (Input.anyKey)
        {
            yield return new WaitForEndOfFrame();
        }

        while (!Input.anyKey)
        {
            yield return new WaitForEndOfFrame();
        }

        if(keyToChange == jumpKey)
        {
            jumpKey = DetectPressedKeyOrButton(jumpKey);
        }
        else if(keyToChange == dashKey)
        {
            dashKey = DetectPressedKeyOrButton(dashKey);
        }
        else if (keyToChange == upKey)
        {
            upKey = DetectPressedKeyOrButton(upKey);
        }
        else if (keyToChange == downKey)
        {
            downKey = DetectPressedKeyOrButton(downKey);
        }
        else if (keyToChange == rightKey)
        {
            rightKey = DetectPressedKeyOrButton(rightKey);
        }
        else if (keyToChange == leftKey)
        {
            leftKey = DetectPressedKeyOrButton(leftKey);
        }

        Debug.Log("Invalid key");
        yield return keyToChange;
    }


    public KeyCode DetectPressedKeyOrButton(KeyCode originalKey)
    {
        foreach (KeyCode newKey in Enum.GetValues(typeof(KeyCode)))
        {
            
            //Debug.Log(Input.GetKey(newKey));
            if (Input.GetKey(newKey))
            {
                Debug.Log(newKey);
                return newKey;
            }
                
            
        }
        Debug.Log("No button was returned");
        return originalKey;
    }

    //private
}
