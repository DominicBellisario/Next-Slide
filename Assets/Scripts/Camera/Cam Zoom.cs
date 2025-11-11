using System.Collections;
using UnityEngine;

public class CamZoom : CameraMove
{
    [Header("Zoom")]
    [SerializeField] float finalSize = 5;

    protected override void PlayCameraEffect()
    {
        base.PlayCameraEffect();
        StartCoroutine(Zoom());
    }

    private IEnumerator Zoom()
    {
        float t = 0f;
        while (t < 1)
        {
            cam.orthographicSize = Mathf.LerpUnclamped(startSize, finalSize, tChangeCurve.Evaluate(t));
            t += Time.deltaTime / effectTime;
            yield return null;
        }
        cam.orthographicSize = finalSize;
        Helper.LoadNextSlide();
    }
}
