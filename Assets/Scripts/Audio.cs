using UnityEngine;

public class Audio : MonoBehaviour
{
    [Header ("Bounce")]
    [SerializeField] AudioSource bounceSource;
    [SerializeField] AudioClip bounceBackNormal;
    [SerializeField] AudioClip bounceBackImpact;
    [SerializeField] AudioClip groundImpact;
    [Header ("Player Events")]
    [SerializeField] AudioSource pEventsSource;
    [SerializeField] AudioClip deathBefore;
    [SerializeField] AudioClip deathAfter;
    [Header ("Speed Panel")]
    [SerializeField] AudioSource speedPanel;
    [SerializeField] AudioClip hitSpeedPanel;
    [Header ("Level Transition")]
    [SerializeField] AudioSource levelTransition;
    [SerializeField] AudioClip hitTarget;

    void OnEnable()
    {
        PlayerMovement.BounceBackNormal += PlayBounceBackNormal;
        PlayerMovement.BounceBackImpact += PlayBounceBackImpact;
        PlayerMovement.ImpactState += PlaySpeedPanel;
        PlayerMovement.HitTarget += PlayTarget;
        PlayerMovement.HitGroundHard += PlayGround;
        PlayerMovement.SquishV += PlayDeathBefore;
        PlayerEffects.ShatterPlayer += PlayDeathAfter;
    }
    void OnDisable()
    {
        PlayerMovement.BounceBackNormal -= PlayBounceBackNormal;
        PlayerMovement.BounceBackImpact -= PlayBounceBackImpact;
        PlayerMovement.ImpactState -= PlaySpeedPanel;
        PlayerMovement.HitTarget -= PlayTarget;
        PlayerMovement.HitGroundHard -= PlayGround;
        PlayerMovement.SquishV -= PlayDeathBefore;
        PlayerEffects.ShatterPlayer -= PlayDeathAfter;
    }

    void PlayBounceBackNormal(int nan) { bounceSource.PlayOneShot(bounceBackNormal); }

    void PlayBounceBackImpact(int nan) { bounceSource.PlayOneShot(bounceBackImpact); }

    void PlaySpeedPanel() { speedPanel.PlayOneShot(hitSpeedPanel); }

    void PlayTarget() { levelTransition.PlayOneShot(hitTarget); }

    void PlayGround() { bounceSource.PlayOneShot(groundImpact); }

    void PlayDeathBefore() { pEventsSource.PlayOneShot(deathBefore); }

    void PlayDeathAfter() { pEventsSource.PlayOneShot(deathAfter); }
}
