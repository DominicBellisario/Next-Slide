using System;
using UnityEngine;

public class FlipObject : MonoBehaviour
{
    public static event Action StartFlip;
    public static event Action EndFlip;

    [SerializeField] float flipDuration;
    public float FlipDuration
    {
        get
        {
            StartFlip?.Invoke();
            StartCoroutine(Helper.DoThisAfterDelay(InvokeEndFlip, flipDuration));
            return flipDuration;
        }
    }
    [SerializeField] AnimationCurve flipCurve;
    public AnimationCurve FlipCurve { get { return flipCurve; } }
    

    private void InvokeEndFlip() { EndFlip?.Invoke(); }
}
