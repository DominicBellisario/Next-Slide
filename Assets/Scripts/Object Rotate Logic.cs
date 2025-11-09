using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class ObjectRotateLogic : MonoBehaviour
{
    public static event Action<GameObject, float> ChangingRotation;
    [SerializeField] EdgeLogic arrow;
    [SerializeField] float rotationSpeed = 360f; // degrees per second
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    bool rotating;
    float startAngle;
    float targetAngle;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rotating = false;
        GetComponent<BoxCollider2D>().size = spriteRenderer.size;

        // Make sure rotation is controlled by physics
        rb.freezeRotation = false;
    }

    void Update()
    {
        // Handle click and drag input here (non-physics)
        if (Input.GetMouseButtonDown(0) && ObjectClicked())
        {
            rotating = true;
            arrow.ChangeToSelectedColor();

            // Record the initial angle between mouse and object center
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mouseWorld - transform.position;
            startAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - rb.rotation;
        }

        if (rotating && Input.GetMouseButton(0))
        {
            // Compute the desired target angle based on mouse movement
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mouseWorld - transform.position;
            float currentAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            targetAngle = currentAngle - startAngle;
        }

        if (rotating && Input.GetMouseButtonUp(0))
        {
            rotating = false;
            arrow.ChangeToIdleColor();
        }
    }

    void FixedUpdate()
    {
        if (!rotating) return;

        float startAngle = rb.rotation;
        // Smoothly rotate toward the target angle using Rigidbody2D
        float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(newAngle);
        float deltaAngle = Mathf.DeltaAngle(startAngle, newAngle);
        if (deltaAngle != float.Epsilon) ChangingRotation?.Invoke(gameObject, Mathf.DeltaAngle(startAngle, newAngle));
    }

    bool ObjectClicked()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Bounds bounds = spriteRenderer.bounds;
        return mouseWorld.x > bounds.min.x && mouseWorld.x < bounds.max.x &&
               mouseWorld.y > bounds.min.y && mouseWorld.y < bounds.max.y;
    }
}
