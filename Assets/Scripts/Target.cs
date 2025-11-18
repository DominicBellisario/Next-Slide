using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] ParticleSystem pSystem;
    [SerializeField] GameObject sprite;
    [SerializeField] AnimationCurve sizeCurve;
    [SerializeField] float sizeTime;

    private void OnEnable()
    {
        PlayerDebug.TargetToPlayer += TargetToPlayer;
        PlayerMovement.HitTarget += PlayHitEffects;
    }
    private void OnDisable()
    {
        PlayerDebug.TargetToPlayer -= TargetToPlayer;
        PlayerMovement.HitTarget -= PlayHitEffects;
    }

    private void TargetToPlayer(Vector3 pos)
    {
        transform.position = pos;
    }

    private void PlayHitEffects()
    {
        StartCoroutine(PlayHitEffectsCoroutine());
    }

    private IEnumerator PlayHitEffectsCoroutine()
    {
        pSystem.Play();
        float t = 0;
        while (t < 1)
        {
            float newScale = sizeCurve.Evaluate(t);
            sprite.transform.localScale = new Vector3(newScale, newScale, newScale);

            t += Time.deltaTime / sizeTime;
            yield return null;
        }
    }
}
