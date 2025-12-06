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
    [SerializeField] ParticleSystem deathParticles;
    [Header("After Image")]
    [SerializeField] GameObject afterimagePrefab;
    [SerializeField] float spawnInterval = 0.05f;
    [Header("Death")]
    [SerializeField] Sprite deadSprite;
    [SerializeField] float beforeShatterTime;
    [SerializeField] float afterShatterTime;

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
        PlayerMovement.HitGroundHard += PlayHardFallParticles;
        PlayerMovement.HitGroundHard += StopLaunchParticles;
        PlayerMovement.HitGroundSoft += StopLaunchParticles;
        PlayerMovement.SquishV += PlaySquishEffect;
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
        PlayerMovement.HitGroundHard -= PlayHardFallParticles;
        PlayerMovement.HitGroundHard -= StopLaunchParticles;
        PlayerMovement.HitGroundSoft -= StopLaunchParticles;
        PlayerMovement.SquishV -= PlaySquishEffect;
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
        StopLaunchParticles();
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

    private void PlaySquishEffect() 
    { 
        StartCoroutine(SquishEffect());
    }

    private IEnumerator SquishEffect()
    {
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        sprite.color = Color.red;
        sprite.sprite = deadSprite;
        yield return new WaitForSeconds(beforeShatterTime);

        sprite.enabled = false;
        deathParticles.Play();
        yield return new WaitForSeconds(afterShatterTime);

        Helper.ReloadScene();
    } 

    private void PlayLaunchParticles()
    {
        launchParticles.Play();
    }
    private void StopLaunchParticles()
    {
        launchParticles.Stop();
    }

    private void PlayNormalBounceParticles(int direction)
    {
        StopLaunchParticles();
        if (direction == 1) { rightNormalLaunchParticles.Play(); }
        else { leftNormalLaunchParticles.Play(); }
    }
    private void PlayImpactBounceParticles(int direction)
    {
        StopLaunchParticles();
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
