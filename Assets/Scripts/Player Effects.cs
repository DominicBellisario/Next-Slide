using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerEffects : MonoBehaviour
{
    public static event Action FinishedTargetEffect;

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float targetSlowDownTime;
    [SerializeField] float targetWaitTime;
    [SerializeField] Color impactColor;
    [SerializeField] ParticleSystem launchParticles;
    [SerializeField] ParticleSystem leftLaunchParticles;
    [SerializeField] ParticleSystem rightLaunchParticles;
    Rigidbody2D rb;
    Color startColor;

    void OnEnable()
    {
        PlayerMovement.NormalState += NormalStateEffects;
        PlayerMovement.ImpactState += ImpactStateEffects;
        PlayerMovement.HitTarget += PlayTargetEffect;
        PlayerMovement.LaunchedUp += PlayLaunchParticles;
        PlayerMovement.BounceBackNormal += PlayBounceParticles;
    }
    void OnDisable()
    {
        PlayerMovement.NormalState -= NormalStateEffects;
        PlayerMovement.ImpactState -= ImpactStateEffects;
        PlayerMovement.HitTarget -= PlayTargetEffect;
        PlayerMovement.LaunchedUp -= PlayLaunchParticles;
        PlayerMovement.BounceBackNormal -= PlayBounceParticles;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startColor = spriteRenderer.color;
    }

    private void ImpactStateEffects()
    {
        spriteRenderer.color = impactColor;
    }
    private void NormalStateEffects()
    {
        spriteRenderer.color = startColor;
    }

    private void PlayTargetEffect() { StartCoroutine(TargetEffect()); }
    private IEnumerator TargetEffect()
    {
        rb.gravityScale = 0f;
        float t = 0;
        Vector2 startVelocity = rb.linearVelocity;
        while (t < 1)
        {
            rb.linearVelocity = Vector2.Lerp(startVelocity, Vector2.zero, t);
            t += Time.deltaTime / targetSlowDownTime;
            yield return null;
        }
        yield return new WaitForSeconds(targetWaitTime);
        FinishedTargetEffect?.Invoke();
    }

    private void PlayLaunchParticles()
    {
        launchParticles.Play();
    }

    private void PlayBounceParticles(int direction)
    {
        if (direction == 1) { rightLaunchParticles.Play(); }
        else { leftLaunchParticles.Play(); }
    }
}
