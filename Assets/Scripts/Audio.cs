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
    [SerializeField] AudioClip bounceUp;
    [Header("Objects")]
    [SerializeField] AudioSource objectsSource;
    [SerializeField] AudioClip objectClick;
    [SerializeField] AudioClip objectUnClick;
    [SerializeField] AudioClip rotateObjectMoveRight;
    [SerializeField] AudioClip rotateObjectMoveLeft;
    [SerializeField] AudioClip[] borderHits;
    [Header ("Speed Panel")]
    [SerializeField] AudioSource speedPanelSource;
    [SerializeField] AudioClip hitSpeedPanel;
    [Header ("Level Transition")]
    [SerializeField] AudioSource levelTransitionSource;
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
        ObjectScale.Clicked += PlayObjectClick;
        ObjectMove.Clicked += PlayObjectClick;
        ObjectRotate.Clicked += PlayObjectClick;
        ObjectScale.UnClicked += PlayObjectUnClick;
        ObjectMove.UnClicked += PlayObjectUnClick;
        ObjectRotate.UnClicked += PlayObjectUnClick;
        ObjectRotate.RotatedAGoodAmount += PlayRotateObjectMove;
        PlayerMovement.LaunchedUp += PlayLaunchSound;
        ObjectMove.HitBorder += PlayBorderHitSound;
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
        ObjectScale.Clicked -= PlayObjectClick;
        ObjectMove.Clicked -= PlayObjectClick;
        ObjectRotate.Clicked -= PlayObjectClick;
        ObjectScale.UnClicked -= PlayObjectUnClick;
        ObjectMove.UnClicked -= PlayObjectUnClick;
        ObjectRotate.UnClicked -= PlayObjectUnClick;
        ObjectRotate.RotatedAGoodAmount -= PlayRotateObjectMove;
        PlayerMovement.LaunchedUp -= PlayLaunchSound;
        ObjectMove.HitBorder -= PlayBorderHitSound;
    }

    void PlayBounceBackNormal(int nan) { bounceSource.PlayOneShot(bounceBackNormal); }

    void PlayBounceBackImpact(int nan) { bounceSource.PlayOneShot(bounceBackImpact); }

    void PlaySpeedPanel() { speedPanelSource.PlayOneShot(hitSpeedPanel); }

    void PlayTarget() { levelTransitionSource.PlayOneShot(hitTarget); }

    void PlayGround() { bounceSource.PlayOneShot(groundImpact); }

    void PlayDeathBefore() { pEventsSource.PlayOneShot(deathBefore); }

    void PlayDeathAfter() { pEventsSource.PlayOneShot(deathAfter); }

    void PlayObjectClick() { objectsSource.PlayOneShot(objectClick); }

    void PlayObjectUnClick() { objectsSource.PlayOneShot(objectUnClick); }

    void PlayRotateObjectMove(bool rotatingRight) 
    { 
        if (rotatingRight) objectsSource.PlayOneShot(rotateObjectMoveRight); 
        else objectsSource.PlayOneShot(rotateObjectMoveLeft);
    }

    void PlayLaunchSound() { pEventsSource.PlayOneShot(bounceUp); }

    void PlayBorderHitSound()
    {
        objectsSource.PlayOneShot(borderHits[Random.Range(0,borderHits.Length)]);
    }
}
