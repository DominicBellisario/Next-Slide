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
    BounceBackImpact
}

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public static event Action BounceBackNormal;
    public static event Action BounceBackImpact;
    public static event Action FlipH;
    public static event Action SpeedPanel;

    [SerializeField] Collider2D leftCollider;
    [SerializeField] Collider2D centerCollider;
    [SerializeField] Collider2D rightCollider;

    [SerializeField] float accelNormal;
    [SerializeField] float maxSpeedNormal;
    [SerializeField] float accelImpact;
    [SerializeField] float maxSpeedImpact;
    [SerializeField] Vector2 bounceBackForceNormal;
    [SerializeField] Vector2 bounceBackForceImpact;
    [SerializeField] float objectLaunchUpForce;


    Rigidbody2D rb;
    int facingRight;
    int normalGravity;
    private PlayerState currentState;
    bool canLaunch;
    float pendingVerticalOffset;

    void OnEnable()
    {
        ObjectScaleLogic.ReachedMaxHeight += CheckIfPlayerIsLaunched;
        ObjectScaleLogic.ChangingHeight += CheckIfPlatformBelowIsChanging;
        ObjectMoveLogic.ReachedMaxHeight += CheckIfPlayerIsLaunched;
        ObjectMoveLogic.ChangingHeight += CheckIfPlatformBelowIsChanging;
        ObjectRotateLogic.ChangingRotation += CheckIfPlatformBelowIsChanging;
    }
    void OnDisable()
    {
        ObjectScaleLogic.ReachedMaxHeight -= CheckIfPlayerIsLaunched;
        ObjectScaleLogic.ChangingHeight -= CheckIfPlatformBelowIsChanging;
        ObjectMoveLogic.ReachedMaxHeight -= CheckIfPlayerIsLaunched;
        ObjectMoveLogic.ChangingHeight -= CheckIfPlatformBelowIsChanging;
        ObjectRotateLogic.ChangingRotation -= CheckIfPlatformBelowIsChanging;
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
                rb.AddForce(accelNormal * facingRight * Time.deltaTime * Vector2.right);
                rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocityX, -maxSpeedNormal, maxSpeedNormal), rb.linearVelocityY);
                break;
            // player moves forward at a fast acceleration with a fast max speed
            case PlayerState.Impact:
                rb.AddForce(accelImpact * facingRight * Time.deltaTime * Vector2.right);
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

                currentState = PlayerState.Normal;
                break;
            // player is launched back a lot in the opposite direction
            case PlayerState.BounceBackImpact:
                Debug.Log("Bounce Back Impact");
                rb.linearVelocity = Vector2.zero;
                //move them manually back a bit (this is to get them out quick if the player quickly moves a block over them)
                rb.position += new Vector2(0.1f * -facingRight, 0f);
                rb.AddForce(bounceBackForceImpact * new Vector2(-facingRight, normalGravity));
                BounceBackImpact?.Invoke();

                currentState = PlayerState.Normal;
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

    // void OnDrawGizmos()
    // {
    //     Vector3 center = new Vector3(transform.position.x, transform.position.y - centerCollider.bounds.extents.y - 0.2f + 0.3f, 0f);
    //     Gizmos.DrawWireSphere(center, 0.3f);
    // }
}
