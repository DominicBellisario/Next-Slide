using System;
using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;

public enum PlayerState
{
    Normal,
    Impact,
    BounceBackNormal,
    BounceBackImpact,
    Stunned,
    Target
}

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public static event Action ImpactState;
    public static event Action NormalState;
    public static event Action StunState;
    public static event Action<int> BounceBackNormal;
    public static event Action<int> BounceBackImpact;
    public static event Action<int> BreakImpactObject;
    public static event Action FlipH;
    public static event Action HitTarget;
    public static event Action LaunchedUp;

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
    Action nextState;

    void OnEnable()
    {
        ObjectScale.ReachedMaxHeight += CheckIfPlayerIsLaunched;
        ObjectScale.ChangingHeight += CheckIfPlatformBelowIsChanging;
        ObjectMove.ReachedMaxHeight += CheckIfPlayerIsLaunched;
        ObjectMove.ChangingHeight += CheckIfPlatformBelowIsChanging;
        ObjectRotate.ChangingRotation += CheckIfRotatingPlatformIsChanging;
    }
    void OnDisable()
    {
        ObjectScale.ReachedMaxHeight -= CheckIfPlayerIsLaunched;
        ObjectScale.ChangingHeight -= CheckIfPlatformBelowIsChanging;
        ObjectMove.ReachedMaxHeight -= CheckIfPlayerIsLaunched;
        ObjectMove.ChangingHeight -= CheckIfPlatformBelowIsChanging;
        ObjectRotate.ChangingRotation -= CheckIfRotatingPlatformIsChanging;
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

    void Update()
    {
        // Handle non-physics things:

        switch (currentState)
        {
            case PlayerState.Stunned:
                stunTimer += Time.deltaTime;
                if (stunTimer >= currentStunTime)
                    nextState();
                break;
        }
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case PlayerState.Normal:
                rb.AddForce(NormalAlignedForce(accelNormal * facingRight * Vector2.right));
                rb.linearVelocity = new Vector2(
                    Mathf.Clamp(rb.linearVelocityX, -maxSpeedNormal, maxSpeedNormal),
                    rb.linearVelocityY
                );
                break;

            case PlayerState.Impact:
                rb.AddForce(NormalAlignedForce(accelImpact * facingRight * Vector2.right));
                rb.linearVelocity = new Vector2(
                    Mathf.Clamp(rb.linearVelocityX, -maxSpeedImpact, maxSpeedImpact),
                    rb.linearVelocityY
                );
                break;

            case PlayerState.BounceBackNormal:
                rb.linearVelocity = Vector2.zero;
                rb.position += new Vector2(0.1f * -facingRight, 0f);
                rb.AddForce(bounceBackForceNormal * new Vector2(-facingRight, normalGravity));
                BounceBackNormal?.Invoke(facingRight);
                StunPlayer(stunTimeNormal, SwitchToNormal);
                break;

            case PlayerState.BounceBackImpact:
                rb.linearVelocity = Vector2.zero;
                rb.position += new Vector2(0.1f * -facingRight, 0f);
                rb.AddForce(bounceBackForceImpact * new Vector2(-facingRight, normalGravity));
                BounceBackImpact?.Invoke(facingRight);
                StunPlayer(stunTimeImpact, SwitchToNormal);
                break;

            case PlayerState.Stunned:
                float decel = stunDecceleration * Time.fixedDeltaTime;

                float vx = rb.linearVelocityX;
                vx -= Mathf.Sign(vx) * decel;

                rb.linearVelocity = new Vector2(
                    Mathf.Clamp(vx, -maxSpeedImpact, maxSpeedImpact),
                    rb.linearVelocityY
                );
                break;
        }

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


    void OnCollisionEnter2D(Collision2D collision)
    {
        // if the side of the player is not hitting the object, return
        if (collision.collider == DownCircleCast(0.4f, centerCollider.includeLayers).collider) return;
        if (collision.otherCollider.name == "Center") return;
        if (collision.otherCollider.name == "Left" && facingRight == 1) return;
        if (collision.otherCollider.name == "Right" && facingRight == -1) return;

        // player side colliders hit the side of an impact object
        if (collision.collider.CompareTag("ImpactObject"))
        {
            switch (currentState)
            {
                // bounce back
                case PlayerState.Normal:
                    collision.collider.GetComponent<ObjectImpact>().HitObject();
                    currentState = PlayerState.BounceBackNormal;
                    break;
                // destroy the object
                case PlayerState.Impact:
                    collision.collider.GetComponent<ObjectImpact>().DestroyObject();
                    rb.linearVelocityX = (maxSpeedImpact - 2f) * facingRight;
                    BreakImpactObject?.Invoke(0);
                    break;
                default:
                    collision.collider.GetComponent<ObjectImpact>().HitObject();
                    currentState = PlayerState.BounceBackNormal;
                    break;
            }
        }
        // player side colliders hit the side of a normal object
        else
        {
            switch (currentState)
            {
                // bounce back a little
                case PlayerState.Normal:
                    currentState = PlayerState.BounceBackNormal;
                    break;
                // bounce back a lot
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
        if (collision.CompareTag("FlipHLeft"))
        {
            StartCoroutine(PerformFlipH(-1));
        }
        else if (collision.CompareTag("FlipHRight"))
        {
            StartCoroutine(PerformFlipH(1));
        }
        else if (collision.CompareTag("SpeedPanel"))
        {
            rb.linearVelocityX = maxSpeedImpact * facingRight;
            collision.gameObject.GetComponent<SpeedPanel>().PlayEffects();
            SwitchToImpact();
        }
        else if (collision.CompareTag("Target"))
        {
            currentState = PlayerState.Target;
            leftCollider.enabled = false;
            centerCollider.enabled = false;
            rightCollider.enabled = false;
            HitTarget?.Invoke();
        }
    }

    private void SwitchToNormal()
    {
        NormalState?.Invoke();
        currentState = PlayerState.Normal;
    }
    private void SwitchToImpact()
    {
        ImpactState?.Invoke();
        currentState = PlayerState.Impact;
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
            return adjustedForce;
        }
        return startForce;
    }

    private void CheckIfPlayerIsLaunched(GameObject gameObject)
    {
        RaycastHit2D hit = DownCircleCast(0.05f, rightCollider.includeLayers);
        if (!canLaunch || hit.collider == null) return;
        if (hit.collider.gameObject == gameObject)
        {
            canLaunch = false;
            centerCollider.enabled = false;
            StartCoroutine(Helper.DoThisAfterDelay(() => canLaunch = true, 0.5f)); // player cannot launch again for half a second
            StartCoroutine(Helper.DoThisAfterDelay(() => centerCollider.enabled = true, 0.5f));
            rb.linearVelocityY = 0f;
            rb.AddForce(new Vector2(0f, objectLaunchUpForce));
            LaunchedUp?.Invoke();
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

    private RaycastHit2D DownCircleCast(float offset, LayerMask layerMask)
    {
        return Physics2D.CircleCast(transform.position, 0.2f, Vector2.down, centerCollider.bounds.extents.y + offset - 0.2f, layerMask);
    }

    private IEnumerator PerformFlipH(int direction)
    {
        Vector2 startVelocity = rb.linearVelocity;
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        yield return new WaitForSeconds(0.5f);

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        facingRight = direction;
        rb.linearVelocity = startVelocity * Vector2.left;
        FlipH?.Invoke();
    }

    public void StunPlayer(float stunTime, Action _nextState)
    {
        stunTimer = 0;
        currentStunTime = stunTime;
        nextState = _nextState;
        StunState?.Invoke();
        currentState = PlayerState.Stunned;
    }

    // void OnDrawGizmos()
    // {
    //     Vector3 center = new Vector3(transform.position.x, transform.position.y - centerCollider.bounds.extents.y - 0.2f + 0.3f, 0f);
    //     Gizmos.DrawWireSphere(center, 0.3f);
    // }
}
