using System;
using System.Collections;
using UnityEngine;

public class Helper : MonoBehaviour
{
    public static IEnumerator DoThisAfterDelay(Action onReset,float delay)
    {
        yield return new WaitForSeconds(delay);
        onReset?.Invoke();
    }
}
