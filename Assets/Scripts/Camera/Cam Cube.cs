using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CamCube : CameraMove
{
    [Header("Cube")]
    [SerializeField] Vector3 finalRotation;
    [SerializeField] float maxSize;
    [SerializeField] AnimationCurve sizeCurve;

    protected override void PlayCameraEffect()
    {
        base.PlayCameraEffect();
        StartCoroutine(Cube());
    }

    private IEnumerator Cube()
    {
        Vector3 pivot = new Vector3(0, 0, 9.9f);

        // Record starting values
        Vector3 startDirection = transform.position - pivot; // vector pointing from pivot → camera
        Quaternion startRotation = transform.rotation;
        float startSize = cam.orthographicSize;

        Quaternion targetRotation = Quaternion.Euler(finalRotation) * startRotation;
        Vector3 targetPosition = pivot + (Quaternion.Euler(finalRotation) * startDirection);
        Vector3 targetSlidePosition = targetPosition - (targetRotation * Vector3.forward * transform.position.z);
        Instantiate(borderPrefab, targetSlidePosition, targetRotation);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / effectTime;
            float curveT = tChangeCurve.Evaluate(t);

            // Rotate the direction vector smoothly
            Vector3 rotatedDirection = Quaternion.Slerp(
                Quaternion.identity,
                Quaternion.Euler(finalRotation),
                curveT
            ) * startDirection;

            // Update camera position
            transform.position = pivot + rotatedDirection;

            // make camera look at pivot
            transform.LookAt(pivot);

            //update size
            cam.orthographicSize = Mathf.Lerp(startSize, maxSize, sizeCurve.Evaluate(t));

            yield return null;
        }

        Helper.LoadNextSlide();
    }
}
