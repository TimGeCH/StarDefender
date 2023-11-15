using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Player Input")]
public class PlayerInput : ScriptableObject, InputActions.IGameplayActions, InputActions.IPauseMenuActions, InputActions.IGameOverScreenActions, InputActions.IMenuClickActions
{
    public event UnityAction<Vector2> onMove = delegate { };
    public event UnityAction onStopMove = delegate { };
    public event UnityAction onFire = delegate { };
    public event UnityAction onStopFire = delegate { };
    public event UnityAction onDodge = delegate { };
    public event UnityAction onOverDrive = delegate { };
    public event UnityAction onPause = delegate { };
    public event UnityAction onUnpause = delegate { };
    public event UnityAction onLaunchMissile = delegate { };
    public event UnityAction onConfirmGameOver = delegate { };
    public event UnityAction onme = delegate { };
    public event UnityAction onConfrimByController = delegate { };


    InputActions inputActions;

    public bool IsFiring { get; private set; }//ÐÂÌí

    void OnEnable()
    {
        inputActions = new InputActions();

        inputActions.Gameplay.SetCallbacks(this);
        inputActions.PauseMenu.SetCallbacks(this);
        inputActions.GameOverScreen.SetCallbacks(this);
        inputActions.MenuClick.SetCallbacks(this);
    }

    void OnDisable()
    {
        DisableAllInputs();
    }

    void SwitchActionMap(InputActionMap actionMap, bool isUIInput)
    {
        inputActions.Disable();
        actionMap.Enable();

        if (isUIInput)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void SwithcToDynamicUpdateMode()
    {
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
    }

    public void SwithcToFixUpdateMode()
    {
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
    }

    public void DisableAllInputs()
    {
        inputActions.Disable();
    }

    public void EnableGameplayInput()
    {
        SwitchActionMap(inputActions.Gameplay, false);
    }

    public void EnablePauseMenuInput()
    {
        SwitchActionMap(inputActions.PauseMenu, true);
    }

    public void EnableGameOverScreenInput()
    {
        SwitchActionMap(inputActions.GameOverScreen, false);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onMove.Invoke(context.ReadValue<Vector2>());
        }
        if (context.canceled)
        {
            onStopMove.Invoke();
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsFiring = true; // ÐÂÌí
            onFire.Invoke();
        }
        if (context.canceled)
        {
            IsFiring = false; // ÐÂÌí
            onStopFire.Invoke();
        }
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        onDodge.Invoke();
    }

    public void OnOverDrive(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onOverDrive.Invoke();
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onPause.Invoke();
        }
    }

    public void OnUnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onUnpause.Invoke();
        }
    }

    public void OnLaunchMissile(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onLaunchMissile.Invoke();
        }
    }

    public void OnConfirmGameOver(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onConfirmGameOver.Invoke();
        }
    }

    public void OnConfrimByController(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onConfrimByController.Invoke();
        }
    }
}
