using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerEffects : MonoBehaviour
{
    public static event Action FinishedTargetEffect;

    [SerializeField] SpriteRenderer sprite;
    [SerializeField] float targetSlowDownTime;
    [SerializeField] float targetWaitTime;
    [SerializeField] Color impactColor;
    [Header("Particles")]
    [SerializeField] ParticleSystem launchParticles;
    [SerializeField] ParticleSystem leftNormalLaunchParticles;
    [SerializeField] ParticleSystem rightNormalLaunchParticles;
    [SerializeField] ParticleSystem leftImpactLaunchParticles;
    [SerializeField] ParticleSystem rightImpactLaunchParticles;
    [SerializeField] ParticleSystem hardFallParticles;
    [Header("After Image")]
    [SerializeField] GameObject afterimagePrefab;
    [SerializeField] float spawnInterval = 0.05f;

    Rigidbody2D rb;
    Color startColor;
    bool playAfterimage = false;
    float afterimageTimer = 0;

    void OnEnable()
    {
        PlayerMovement.NormalState += NormalStateEffects;
        PlayerMovement.ImpactState += ImpactStateEffects;
        PlayerMovement.StunState += NormalStateEffects;
        PlayerMovement.HitTarget += PlayTargetEffect;
        PlayerMovement.LaunchedUp += PlayLaunchParticles;
        PlayerMovement.BounceBackNormal += PlayNormalBounceParticles;
        PlayerMovement.BounceBackImpact += PlayImpactBounceParticles;
        PlayerMovement.HardFall += PlayHardFallParticles;
    }
    void OnDisable()
    {
        PlayerMovement.NormalState -= NormalStateEffects;
        PlayerMovement.ImpactState -= ImpactStateEffects;
        PlayerMovement.StunState -= NormalStateEffects;
        PlayerMovement.HitTarget -= PlayTargetEffect;
        PlayerMovement.LaunchedUp -= PlayLaunchParticles;
        PlayerMovement.BounceBackNormal -= PlayNormalBounceParticles;
        PlayerMovement.BounceBackImpact -= PlayImpactBounceParticles;
        PlayerMovement.HardFall -= PlayHardFallParticles;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startColor = sprite.color;
    }

    private void ImpactStateEffects()
    {
        sprite.color = impactColor;
        afterimageTimer = 0f;
        playAfterimage = true;
    }
    private void NormalStateEffects()
    {
        sprite.color = startColor;
        playAfterimage = false;
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

    private void PlayNormalBounceParticles(int direction)
    {
        if (direction == 1) { rightNormalLaunchParticles.Play(); }
        else { leftNormalLaunchParticles.Play(); }
    }
    private void PlayImpactBounceParticles(int direction)
    {
        if (direction == 1) { rightImpactLaunchParticles.Play(); }
        else { leftImpactLaunchParticles.Play(); }
    }

    private void PlayHardFallParticles()
    {
        hardFallParticles.Play();
    }

    private void Update()
    {
        if (playAfterimage)
        {
            afterimageTimer -= Time.deltaTime;

            if (afterimageTimer <= 0f)
            {
                GameObject afterimage = Instantiate(afterimagePrefab, transform.position, Quaternion.identity);

                // Copy current sprite
                SpriteRenderer afterimageSR = afterimage.GetComponent<SpriteRenderer>();

                afterimageSR.sprite = sprite.sprite;
                afterimageSR.flipX = sprite.flipX;
                afterimageSR.flipY = sprite.flipY;
                afterimageSR.color = sprite.color;
                afterimage.transform.localScale = sprite.transform.localScale;

                afterimageTimer = spawnInterval;
            }
        }
    }
}
