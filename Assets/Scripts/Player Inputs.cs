using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    public static event Action TargetToPlayer;
    private void OnRestartScene(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            Helper.ReloadScene();
        }
    }

    private void OnTargetToPlayer(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            TargetToPlayer?.Invoke();
        }
    }
}
