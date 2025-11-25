using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    public static event Action TargetToPlayer;
    public static event Action PauseButtonPressed;

    private void OnRestartScene(InputValue inputValue)
    {
        if (inputValue.isPressed) { Helper.ReloadScene(); }
    }

    private void OnTargetToPlayer(InputValue inputValue)
    {
        if (inputValue.isPressed) { TargetToPlayer?.Invoke(); }
    }

    private void OnPause(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            PauseButtonPressed?.Invoke();
        }
    }
}
