using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float normalAcceleration;
    [SerializeField] float maxNormalSoeed;

    Rigidbody2D rb;
    float currentAcceleration;
    float currentMaxSpeed;
    int facingRight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentAcceleration = normalAcceleration;
        currentMaxSpeed = maxNormalSoeed;
        facingRight = 1;
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(normalAcceleration * Time.deltaTime * Vector2.right);

        rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocityX, -currentMaxSpeed, currentMaxSpeed), rb.linearVelocityY);
    }
}
