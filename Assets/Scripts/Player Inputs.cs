using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    public static event Action TargetToPlayer;
    public static event Action Paused;
    public static event Action Unpaused;

    bool isPaused;
    bool canPause = true;

    void OnEnable()
    {
        PauseMenu.FinishedPauseAnimation += ReactivatePause;
    }
    void OnDisable()
    {
        PauseMenu.FinishedPauseAnimation -= ReactivatePause;
    }

    private void OnRestartScene(InputValue inputValue)
    {
        if (inputValue.isPressed) { Helper.ReloadScene(); }
    }

    private void OnTargetToPlayer(InputValue inputValue)
    {
        if (inputValue.isPressed) { TargetToPlayer?.Invoke(); }
    }

    private void ReactivatePause() { canPause = true; }
    private void OnPause(InputValue inputValue)
    {
        if (inputValue.isPressed && canPause)
        {
            if (isPaused)
            {
                isPaused = false;
                Unpaused?.Invoke();
            }
            else
            {
                isPaused = true;
                Paused?.Invoke();
            }
            canPause = false;
        }
    }
}
