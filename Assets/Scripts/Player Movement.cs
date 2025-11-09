using System;
using System.Collections;
using System.Runtime.CompilerServices;
using NUnit.Framework.Internal.Commands;
using Unity.VisualScripting;
using UnityEngine;

public enum PlayerState
{
    Normal,
    Impact,
    BounceBackNormal,
    BounceBackImpact,
    Stunned
}

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public static event Action BounceBackNormal;
    public static event Action BounceBackImpact;
    public static event Action FlipH;
    public static event Action SpeedPanel;

    [Header("Colliders")]
    [SerializeField] Collider2D leftCollider;
    [SerializeField] Collider2D centerCollider;
    [SerializeField] Collider2D rightCollider;
    [Header("Normal")]
    [SerializeField] float accelNormal;
    [SerializeField] float maxSpeedNormal;
    [SerializeField] Vector2 bounceBackForceNormal;
    [Header("Impact")]
    [SerializeField] float accelImpact;
    [SerializeField] float maxSpeedImpact;
    [SerializeField] Vector2 bounceBackForceImpact;
    [Header("Stun")]
    [SerializeField] float stunDecceleration;
    [SerializeField] float stunTimeNormal;
    [SerializeField] float stunTimeImpact;
    [Header("Misc")]
    [SerializeField] float objectLaunchUpForce;

    Rigidbody2D rb;
    int facingRight;
    int normalGravity;
    private PlayerState currentState;
    bool canLaunch;
    float pendingVerticalOffset;
    float currentStunTime;
    float stunTimer;
    PlayerState nextState;

    void OnEnable()
    {
        ObjectScaleLogic.ReachedMaxHeight += CheckIfPlayerIsLaunched;
        ObjectScaleLogic.ChangingHeight += CheckIfPlatformBelowIsChanging;
        ObjectMoveLogic.ReachedMaxHeight += CheckIfPlayerIsLaunched;
        ObjectMoveLogic.ChangingHeight += CheckIfPlatformBelowIsChanging;
        ObjectRotateLogic.ChangingRotation += CheckIfRotatingPlatformIsChanging;
    }
    void OnDisable()
    {
        ObjectScaleLogic.ReachedMaxHeight -= CheckIfPlayerIsLaunched;
        ObjectScaleLogic.ChangingHeight -= CheckIfPlatformBelowIsChanging;
        ObjectMoveLogic.ReachedMaxHeight -= CheckIfPlayerIsLaunched;
        ObjectMoveLogic.ChangingHeight -= CheckIfPlatformBelowIsChanging;
        ObjectRotateLogic.ChangingRotation -= CheckIfRotatingPlatformIsChanging;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        facingRight = 1;
        normalGravity = 1;
        currentState = PlayerState.Normal;
        canLaunch = true;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            // player moves forward at a normal acceleration with a normal max speed
            case PlayerState.Normal:
                rb.AddForce(NormalAlignedForce(accelNormal * facingRight * Vector2.right));
                rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocityX, -maxSpeedNormal, maxSpeedNormal), rb.linearVelocityY);
                break;
            // player moves forward at a fast acceleration with a fast max speed
            case PlayerState.Impact:
                rb.AddForce(NormalAlignedForce(accelImpact * facingRight * Vector2.right));
                rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocityX, -maxSpeedImpact, maxSpeedImpact), rb.linearVelocityY);
                break;
            // player is launched back in the opposite direction
            case PlayerState.BounceBackNormal:
                Debug.Log("Bounce Back Normal");
                rb.linearVelocity = Vector2.zero;
                //move them manually back a bit (this is to get them out quick if the player quickly moves a block over them)
                rb.position += new Vector2(0.1f * -facingRight, 0f);
                rb.AddForce(bounceBackForceNormal * new Vector2(-facingRight, normalGravity));
                BounceBackNormal?.Invoke();

                StunPlayer(stunTimeNormal, PlayerState.Normal);
                break;
            // player is launched back a lot in the opposite direction
            case PlayerState.BounceBackImpact:
                Debug.Log("Bounce Back Impact");
                rb.linearVelocity = Vector2.zero;
                //move them manually back a bit (this is to get them out quick if the player quickly moves a block over them)
                rb.position += new Vector2(0.1f * -facingRight, 0f);
                rb.AddForce(bounceBackForceImpact * new Vector2(-facingRight, normalGravity));
                BounceBackImpact?.Invoke();

                StunPlayer(stunTimeImpact, PlayerState.Impact);
                break;
            // player does not try to move for a time
            case PlayerState.Stunned:
                if (stunTimer < currentStunTime)
                {
                    stunTimer += Time.deltaTime;
                    rb.AddForce(new Vector2(-Mathf.Sign(rb.linearVelocityX) * stunDecceleration, 0f));
                }
                else { currentState = nextState; }
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // bounce back if a side collider hits an object that is NOT the object the player is standing on
        if (collision.otherCollider != centerCollider && collision.collider != DownCircleCast(0.4f, centerCollider.includeLayers).collider)
        {
            switch (currentState)
            {
                case PlayerState.Normal:
                    currentState = PlayerState.BounceBackNormal;
                    break;
                case PlayerState.Impact:
                    currentState = PlayerState.BounceBackImpact;
                    break;
                default:
                    currentState = PlayerState.BounceBackNormal;
                    break;
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("FlipH"))
        {
            StartCoroutine(PerformFlipH());
        }
        else if (collision.CompareTag("SpeedPanel"))
        {
            rb.linearVelocityX = maxSpeedImpact * facingRight;
            currentState = PlayerState.Impact;
        }
    }

    private Vector2 NormalAlignedForce(Vector2 startForce)
    {
        RaycastHit2D hit = DownCircleCast(0.2f, centerCollider.includeLayers);
        if (hit.collider)
        {
            // Project the force perpendicular to the normal
            Vector2 tangent = new(hit.normal.y, -hit.normal.x);

            // Return movement along the slope
            Vector2 adjustedForce = Mathf.Sign(startForce.x) * startForce.magnitude * tangent.normalized;
            Debug.Log(adjustedForce);
            return adjustedForce;
        }
        Debug.Log(startForce);
        return startForce;
    }

    private void CheckIfPlayerIsLaunched(GameObject gameObject)
    {
        RaycastHit2D hit = DownCircleCast(0.05f, centerCollider.includeLayers);
        if (!canLaunch || hit.collider == null) return;
        if (hit.collider.gameObject == gameObject)
        {
            canLaunch = false;
            centerCollider.enabled = false;
            StartCoroutine(Helper.DoThisAfterDelay(() => canLaunch = true, 0.5f)); // player cannot launch again for half a second
            StartCoroutine(Helper.DoThisAfterDelay(() => centerCollider.enabled = true, 0.5f));
            rb.linearVelocityY = 0f;
            rb.AddForce(new Vector2(0f, objectLaunchUpForce));
            Debug.Log("jump");
        }
    }

    private void CheckIfRotatingPlatformIsChanging(GameObject platformObj, float deltaAngle)
    {
        RaycastHit2D hit = DownCircleCast(0.5f, centerCollider.includeLayers);
        if (hit.collider && hit.collider.gameObject == platformObj)
        {
            // Convert to radians (for Mathf trig)
            float radians = deltaAngle * Mathf.Deg2Rad;

            // Vector from pivot to player (in world space)
            Vector2 pivotToPlayer = (Vector2)(transform.position - platformObj.transform.position);

            // Compute where that point moves when the platform rotates by deltaAngle
            // Rotation direction in Unity is counter-clockwise positive
            Vector2 rotated = new Vector2(
                pivotToPlayer.x * Mathf.Cos(radians) - pivotToPlayer.y * Mathf.Sin(radians),
                pivotToPlayer.x * Mathf.Sin(radians) + pivotToPlayer.y * Mathf.Cos(radians)
            );

            // Displacement caused by rotation
            Vector2 delta = rotated - pivotToPlayer;

            // The height (Y) difference is what you care about
            float heightChange = delta.y;

            // Apply it
            pendingVerticalOffset += heightChange;
        }

    }

    private void CheckIfPlatformBelowIsChanging(GameObject platformObj, float heightChange)
    {
        RaycastHit2D hit = DownCircleCast(0.2f, centerCollider.includeLayers);
        if (hit.collider && hit.collider.gameObject == platformObj)
        {
            pendingVerticalOffset += heightChange;
        }
    }

    private void FixedUpdate()
    {
        // if there is a pending offet, move the player
        if (Mathf.Abs(pendingVerticalOffset) > Mathf.Epsilon)
        {
            Vector2 newPos = rb.position + new Vector2(0f, pendingVerticalOffset);
            rb.position = newPos;

            // small downward bias to keep contact solid
            rb.linearVelocityY = Mathf.Min(rb.linearVelocityY, -0.5f);

            pendingVerticalOffset = 0f;
        }
    }

    private RaycastHit2D DownCircleCast(float offset, LayerMask layerMask)
    {
        return Physics2D.CircleCast(transform.position, 0.3f, Vector2.down, centerCollider.bounds.extents.y + offset - 0.3f, layerMask);
    }

    private IEnumerator PerformFlipH()
    {
        Vector2 startVelocity = rb.linearVelocity;
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        yield return new WaitForSeconds(0.5f);

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        facingRight *= -1;
        rb.linearVelocity = startVelocity * Vector2.left;
        FlipH?.Invoke();
    }

    public void StunPlayer(float stunTime, PlayerState _nextState)
    {
        stunTimer = 0;
        currentStunTime = stunTime;
        nextState = _nextState;
        currentState = PlayerState.Stunned;
    }

    // void OnDrawGizmos()
    // {
    //     Vector3 center = new Vector3(transform.position.x, transform.position.y - centerCollider.bounds.extents.y - 0.2f + 0.3f, 0f);
    //     Gizmos.DrawWireSphere(center, 0.3f);
    // }
}
