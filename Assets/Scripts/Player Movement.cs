using System;
using NUnit.Framework.Internal.Commands;
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

    void OnEnable()
    {
        ObjectLogic.ReachedMaxHeight += CheckIfPlayerIsLaunched;
    }
    void OnDisable()
    {
        ObjectLogic.ReachedMaxHeight -= CheckIfPlayerIsLaunched;
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
                rb.linearVelocity = Vector2.zero;
                //move them manually back a bit (this is to get them out quick if the player quickly moves a block over them)
                transform.position += new Vector3(100f * Time.deltaTime * -facingRight, 0f, 0f);
                rb.AddForce(bounceBackForce * new Vector2(-facingRight, normalGravity));
                BounceBack?.Invoke();

                currentState = PlayerState.Walking;
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // bounce back if a side collider hits an object
        if (collision.otherCollider != centerCollider) currentState = PlayerState.BounceBack;
    }

    private bool IsGrounded()
    {
        float raycastDistance = centerCollider.bounds.extents.y + 0.05f;
        if (Physics2D.Raycast(transform.position, -Vector2.up, raycastDistance, centerCollider.includeLayers))
        {
            Debug.Log("grounded");
            return true;
        }
        return false;
    }

    private void CheckIfPlayerIsLaunched(GameObject gameObject)
    {
        float raycastDistance = centerCollider.bounds.extents.y + 0.05f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, raycastDistance, centerCollider.includeLayers);
        if (hit.collider == null) return;
        if (hit.collider.gameObject == gameObject)
        {
            rb.AddForce(new Vector2(objectLaunchUpForce, objectLaunchUpForce));
            Debug.Log("jump");
        }
    }
}
