using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    private EventSystem eventSystem;

    MenuState currentState;
    MenuState returnedState;
    MenuState aboveState;

    public GameObject mainMenuObject;
    public GameObject optionsMenuObject;
    public GameObject audioMenuObject;
    public GameObject controllersMenuObject;
    public GameObject startGameMenuObject;

    public float longMenuTransitionTime = 2f;
    public float shortMenuTransitionTime = 0.1f;

    private void Start()
    {
        eventSystem = EventSystem.current;
        currentState = new MenuMainState();
        currentState.Enter(this);
    }

    private void Update()
    {
        returnedState = currentState.Update(this, Time.deltaTime);
    }



    public void MainMenu()
    {
        ChangeState(new MenuMainState());
        SetSelectedButton(mainMenuObject);
    }



    public void StartGameMenu()
    {
        ChangeState(new MenuStartGameState());
        SetSelectedButton(startGameMenuObject);
    }

    public void Options()
    {
        ChangeState(new MenuOptionsState());
        SetSelectedButton(optionsMenuObject);
    }

    public void Quit()
    {
        Application.Quit();
    }



    public void NewGame()
    {
        ChangeState(new MenuNewGameState());
    }

    public void Continue()
    {
        ChangeState(new MenuContinueState());
    }

    public void GoBack()
    {
        if (aboveState != null)
        {
            ServiceLocator.GetAudio().PlaySound("Pickup_Recharge", SoundType.normal);
            ServiceLocator.GetGamepadRumble().StartGamepadRumble(GamepadRumbleProvider.RumbleSize.small);
            currentState.Exit(this);
            currentState = aboveState;
            currentState.Enter(this);

            if (selectedInMenu2 != null)
            {
                eventSystem.SetSelectedGameObject(selectedInMenu2);
                selectedInMenu2 = null;
            }
            else
            {
                eventSystem.SetSelectedGameObject(selectedInMenu1);
                selectedInMenu1 = null;
            }
        }
    }



    public void AudioMenu()
    {
        ChangeState(new MenuOptionsAudioState());
        SetSelectedButton(audioMenuObject);
    }

    public void ControllerMenu()
    {
        ChangeState(new MenuOptionsControllerState());
        SetSelectedButton(controllersMenuObject);
    }



    private void ChangeState(MenuState menuState)
    {
        ServiceLocator.GetAudio().PlaySound("Pickup_Recharge", SoundType.menuSound);
        ServiceLocator.GetGamepadRumble().StartGamepadRumble(GamepadRumbleProvider.RumbleSize.small);
        SetMenuAboveButton();
        currentState.Exit(this);
        currentState = menuState;
        currentState.Enter(this);
    }



    public void SetAboveState(MenuState newAboveState)
    {
        aboveState = newAboveState;
    }

    private GameObject selectedInMenu1;
    private GameObject selectedInMenu2;
    private void SetMenuAboveButton()
    {
        if(selectedInMenu1 == null)
        {
            selectedInMenu1 = eventSystem.currentSelectedGameObject;
        }
        else
        {
            selectedInMenu2 = eventSystem.currentSelectedGameObject;
        }
    }

    private void SetSelectedButton(GameObject menuObject)
    {
        eventSystem.SetSelectedGameObject(menuObject.gameObject.transform.GetChild(0).gameObject);
    }
}
