using UnityEngine;

public class Audio : MonoBehaviour
{
    [Header ("Bounce")]
    [SerializeField] AudioSource bounceSource;
    [SerializeField] AudioClip bounceBackNormal;
    [SerializeField] AudioClip bounceBackImpact;
    [SerializeField] AudioClip groundImpact;
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
    }
    void OnDisable()
    {
        PlayerMovement.BounceBackNormal -= PlayBounceBackNormal;
        PlayerMovement.BounceBackImpact -= PlayBounceBackImpact;
        PlayerMovement.ImpactState -= PlaySpeedPanel;
        PlayerMovement.HitTarget -= PlayTarget;
        PlayerMovement.HitGroundHard -= PlayGround;
    }

    void PlayBounceBackNormal(int nan) { bounceSource.PlayOneShot(bounceBackNormal); }

    void PlayBounceBackImpact(int nan) { bounceSource.PlayOneShot(bounceBackImpact); }

    void PlaySpeedPanel() { speedPanel.PlayOneShot(hitSpeedPanel); }

    void PlayTarget() { levelTransition.PlayOneShot(hitTarget); }

    void PlayGround() { bounceSource.PlayOneShot(groundImpact); }
}
