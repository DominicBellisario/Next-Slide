using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class ObjectMoveLogic : MonoBehaviour
{
    [SerializeField] float maxMoveSpeed;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    bool moving;
    Vector3 startMousePos;
    float smoothedDeltaX;
    float smoothedDeltaY;
    Vector2 startPos;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        moving = false;
    }

    void Update()
    {
        // if the mouse is down and near one of this shape's edges
        if (Input.GetMouseButtonDown(0) && ObjectClicked())
        {
            // record start values
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startPos = rb.position;
        }

        // if an edge is selected and the mouse is held down
        if (moving && Input.GetMouseButton(0))
        {
            Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float targetDeltaX = currentMousePos.x - startMousePos.x;
            float targetDeltaY = currentMousePos.y - startMousePos.y;

            // Gradually move toward the target delta
            smoothedDeltaX = Mathf.MoveTowards(smoothedDeltaX, targetDeltaX, maxMoveSpeed * Time.deltaTime);
            smoothedDeltaY = Mathf.MoveTowards(smoothedDeltaY, targetDeltaY, maxMoveSpeed * Time.deltaTime);

            // Use these for resizing
            float deltaX = smoothedDeltaX;
            float deltaY = smoothedDeltaY;

            // update the main shape's size and position
            rb.MovePosition(new Vector2(startPos.x + smoothedDeltaX, startPos.y + smoothedDeltaY)); 
        }

        // if an edge was being resized and the mouse was released
        if (moving && Input.GetMouseButtonUp(0))
        {
            smoothedDeltaX = 0f;
            smoothedDeltaY = 0f;

            // change the color of the selected edge back to normal
            moving = false;
        }
    }

    /// <summary>
    /// checks if the mouse is close to an edge and sets whatever edge it is near
    /// </summary>
    /// <returns>wether or not the mouse is near an edge</returns>
    bool ObjectClicked()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Bounds bounds = spriteRenderer.bounds;

        if (bounds.Contains(mouseWorld))
        {
            moving = true;
            return true;
        }
        return false;
    }
}
