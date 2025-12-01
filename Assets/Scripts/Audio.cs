using UnityEngine;

public class Audio : MonoBehaviour
{
    [Header ("Bounce")]
    [SerializeField] AudioSource bounceSource;
    [SerializeField] AudioClip bounceBackNormal;
    [SerializeField] AudioClip bounceBackImpact;
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
    }
    void OnDisable()
    {
        PlayerMovement.BounceBackNormal -= PlayBounceBackNormal;
        PlayerMovement.BounceBackImpact -= PlayBounceBackImpact;
        PlayerMovement.ImpactState -= PlaySpeedPanel;
        PlayerMovement.HitTarget -= PlayTarget;
    }

    void PlayBounceBackNormal(int nan) { bounceSource.PlayOneShot(bounceBackNormal); }

    void PlayBounceBackImpact(int nan) { bounceSource.PlayOneShot(bounceBackImpact); }

    void PlaySpeedPanel() { speedPanel.PlayOneShot(hitSpeedPanel); }

    void PlayTarget() { levelTransition.PlayOneShot(hitTarget); }
}
