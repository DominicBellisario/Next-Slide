using UnityEngine;

public class Target : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerDebug.TargetToPlayer += TargetToPlayer;
    }
    private void OnDisable()
    {
        PlayerDebug.TargetToPlayer -= TargetToPlayer;
    }

    private void TargetToPlayer(Vector3 pos)
    {
        transform.position = pos;
    }
}
