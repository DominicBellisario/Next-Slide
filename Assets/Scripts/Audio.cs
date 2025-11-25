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

    void OnEnable()
    {
        PlayerMovement.BounceBackNormal += PlayBounceBackNormal;
        PlayerMovement.BounceBackImpact += PlayBounceBackImpact;
        PlayerMovement.ImpactState += PlaySpeedPanel;
    }
    void OnDisable()
    {
        PlayerMovement.BounceBackNormal -= PlayBounceBackNormal;
        PlayerMovement.BounceBackImpact -= PlayBounceBackImpact;
        PlayerMovement.ImpactState -= PlaySpeedPanel;
    }

    void PlayBounceBackNormal(int nan) { bounceSource.PlayOneShot(bounceBackNormal); }

    void PlayBounceBackImpact(int nan) { bounceSource.PlayOneShot(bounceBackImpact); }

    void PlaySpeedPanel() { speedPanel.PlayOneShot(hitSpeedPanel); }
}
