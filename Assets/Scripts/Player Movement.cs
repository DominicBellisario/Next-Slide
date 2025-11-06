using System;
using NUnit.Framework.Internal.Commands;
using Unity.VisualScripting;
using UnityEngine;

public enum PlayerState
{
    Walking,
    BounceBack
}

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public static event Action BounceBack;
    public static event Action FlipH;

    [SerializeField] Collider2D leftCollider;
    [SerializeField] Collider2D centerCollider;
    [SerializeField] Collider2D rightCollider;

    [SerializeField] float normalAcceleration;
    [SerializeField] float maxNormalSoeed;
    [SerializeField] Vector2 bounceBackForce;
    [SerializeField] float objectLaunchUpForce;


    Rigidbody2D rb;
    float currentAcceleration;
    float currentMaxSpeed;
    int facingRight;
    int normalGravity;
    private PlayerState currentState;
    bool canLaunch;
    float pendingVerticalOffset;

    void OnEnable()
    {
        ObjectLogic.ReachedMaxHeight += CheckIfPlayerIsLaunched;
        ObjectLogic.ChangingHeight += CheckIfPlatformBelowIsChanging;
    }
    void OnDisable()
    {
        ObjectLogic.ReachedMaxHeight -= CheckIfPlayerIsLaunched;
        ObjectLogic.ChangingHeight -= CheckIfPlatformBelowIsChanging;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentAcceleration = normalAcceleration;
        currentMaxSpeed = maxNormalSoeed;
        facingRight = 1;
        normalGravity = 1;
        currentState = PlayerState.Walking;
        canLaunch = true;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            // player moves forward at a constant acceleration with a max speed
            case PlayerState.Walking:
                rb.AddForce(currentAcceleration * facingRight * Time.deltaTime * Vector2.right);
                rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocityX, -currentMaxSpeed, currentMaxSpeed), rb.linearVelocityY);
                break;
            // player is launched back in the opposite direction
            case PlayerState.BounceBack:
                Debug.Log("Bounce Back");
                rb.linearVelocity = Vector2.zero;
                //move them manually back a bit (this is to get them out quick if the player quickly moves a block over them)
                rb.position += new Vector2(0.1f * -facingRight, 0f);
                rb.AddForce(bounceBackForce * new Vector2(-facingRight, normalGravity));
                BounceBack?.Invoke();

                currentState = PlayerState.Walking;
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // bounce back if a side collider hits an object that is NOT the object the player is standing on
        if (collision.otherCollider != centerCollider && collision.collider != DownRaycast(0.4f, centerCollider.includeLayers).collider) currentState = PlayerState.BounceBack;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("FlipH"))
        {
            facingRight *= -1;
            FlipH?.Invoke();
        }
    }

    private void CheckIfPlayerIsLaunched(GameObject gameObject)
    {
        RaycastHit2D hit = DownRaycast(0.05f, centerCollider.includeLayers);
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
        RaycastHit2D hit = DownRaycast(0.2f, centerCollider.includeLayers);
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

    private RaycastHit2D DownRaycast(float offset, LayerMask layerMask)
    {
        return Physics2D.Raycast(transform.position, -Vector2.up, centerCollider.bounds.extents.y + offset, layerMask);
    }
}
