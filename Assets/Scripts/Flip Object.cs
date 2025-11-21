using UnityEngine;

public class FlipObject : MonoBehaviour
{
    [SerializeField] float flipDuration;
    public float FlipDuration { get { return flipDuration; } }

    [SerializeField] AnimationCurve flipCurve;
    public AnimationCurve FlipCurve { get { return flipCurve; } }
}
