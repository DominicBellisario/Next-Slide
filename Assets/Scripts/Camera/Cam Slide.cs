using System.Collections;
using UnityEngine;

public class CamSlide : CameraMove
{
    [Header("Slide")]
    [SerializeField] Vector3 slideDistance = new(20, 0, 0);

    protected override void PlayCameraEffect()
    {
        base.PlayCameraEffect();
        StartCoroutine(Slide());
    }
    private IEnumerator Slide()
    {
        float t = 0f;
        Vector3 finalPos = startPos + slideDistance;
        while (t < 1)
        {
            transform.position = Vector3.Lerp(startPos, finalPos, tChangeCurve.Evaluate(t));
            t += Time.deltaTime / effectTime;
            yield return null;
        }
        transform.position = finalPos;
        Helper.LoadNextSlide();
    }
}
