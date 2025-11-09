using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    private void OnRestartScene(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            Helper.ReloadScene();
        }
    }
}
