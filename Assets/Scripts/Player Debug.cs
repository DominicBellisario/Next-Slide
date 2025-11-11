using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDebug : MonoBehaviour
{
    public static event Action<Vector3> TargetToPlayer;

    private void OnEnable()
    {
        PlayerInputs.TargetToPlayer += OnTargetToPlayer;
    }
    private void OnDisable()
    {
        PlayerInputs.TargetToPlayer -= OnTargetToPlayer;
    }

    private void OnTargetToPlayer()
    {
        TargetToPlayer?.Invoke(transform.position);
    }
}
