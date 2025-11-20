using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [SerializeField] float impactBounceDuration;
    [SerializeField] float impactBounceMagnitude;

    private Vector3 startPos;

    void OnEnable()
    {
        PlayerMovement.BounceBackImpact += ImpactBounceShake;
        PlayerMovement.BreakImpactObject += ImpactBounceShake;
    }
    void OnDisable()
    {
        PlayerMovement.BounceBackImpact -= ImpactBounceShake;
        PlayerMovement.BreakImpactObject -= ImpactBounceShake;
    }

    private void ImpactBounceShake(int nan)
    {
        StartCoroutine(ShakeCoroutine(impactBounceDuration, impactBounceMagnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        startPos = transform.localPosition;
        float t = 0f;

        while (t < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, startPos.z);

            t += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = startPos;
    }
}

