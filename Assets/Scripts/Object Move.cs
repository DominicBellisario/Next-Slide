using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class ObjectMove : MonoBehaviour
{
    public static event Action<GameObject> ReachedMaxHeight;
    public static event Action<GameObject, float> ChangingHeight;
    const int TOP = 0;
    const int RIGHT = 1;
    const int BOTTOM = 2;
    const int LEFT = 3;

    [SerializeField] float maxMoveSpeed;
    [SerializeField] Vector2 topLeft;
    [SerializeField] Vector2 bottomRight;
    [SerializeField] GameObject[] borders;
    [SerializeField] ParticleSystem topParticles;
    SpriteRenderer spriteRenderer;
    SpriteRenderer[] borderSprites;
    Edge[] bordersLogic;
    Rigidbody2D rb;
    bool moving;
    Vector3 startMousePos;
    float smoothedDeltaX;
    float smoothedDeltaY;
    Vector2 startPos;
    float lastFrameHeight;
    Vector3 parentStartPos;
    bool hitTop;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        moving = false;
        borderSprites = new SpriteRenderer[4];
        bordersLogic = new Edge[4];
        parentStartPos = GetComponentInParent<Transform>().position;
        hitTop = false;
        for (int i = 0; i < 4; i++)
        {
            borderSprites[i] = borders[i].GetComponent<SpriteRenderer>();
            bordersLogic[i] = borders[i].GetComponent<Edge>();
        }
        borders[TOP].transform.localPosition = new Vector3((bottomRight.x + topLeft.x) / 2, topLeft.y, 0f);
        borders[RIGHT].transform.localPosition = new Vector3(bottomRight.x, (bottomRight.y + topLeft.y) / 2, 0f);
        borders[BOTTOM].transform.localPosition = new Vector3((bottomRight.x + topLeft.x) / 2, bottomRight.y, 0f);
        borders[LEFT].transform.localPosition = new Vector3(topLeft.x, (bottomRight.y + topLeft.y) / 2, 0f);
        borderSprites[TOP].size = new Vector2(Mathf.Abs(bottomRight.x - topLeft.x), borderSprites[TOP].size.y);
        borderSprites[RIGHT].size = new Vector2(borderSprites[RIGHT].size.x, Mathf.Abs(bottomRight.y - topLeft.y));
        borderSprites[BOTTOM].size = new Vector2(Mathf.Abs(bottomRight.x - topLeft.x), borderSprites[BOTTOM].size.y);
        borderSprites[LEFT].size = new Vector2(borderSprites[LEFT].size.x, Mathf.Abs(bottomRight.y - topLeft.y));
        
        topParticles.transform.localPosition = new Vector3(0f, spriteRenderer.size.y / 2, 0f);
        var shape = topParticles.shape;
        shape.radius = spriteRenderer.size.x / 2;
    }

    void Update()
    {
        // if the mouse is down and the object is clicked
        if (Input.GetMouseButtonDown(0) && ObjectClicked())
        {
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startPos = rb.position;
            lastFrameHeight = startPos.y;
            for (int i = 0; i < 4; i++)
            {
                bordersLogic[i].SetEnabled();
            }
        }

        // while moving
        if (moving && Input.GetMouseButton(0))
        {
            Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float targetDeltaX = currentMousePos.x - startMousePos.x;
            float targetDeltaY = currentMousePos.y - startMousePos.y;

            smoothedDeltaX = Mathf.MoveTowards(smoothedDeltaX, targetDeltaX, maxMoveSpeed * Time.deltaTime);
            smoothedDeltaY = Mathf.MoveTowards(smoothedDeltaY, targetDeltaY, maxMoveSpeed * Time.deltaTime);

            // Calculate new unclamped position
            Vector2 newPos = new Vector2(startPos.x + smoothedDeltaX, startPos.y + smoothedDeltaY);

            // Get half-size of object
            Vector2 halfSize = spriteRenderer.bounds.size / 2f;

            // clamp relative to parent
            newPos.x = Mathf.Clamp(newPos.x, parentStartPos.x + topLeft.x + halfSize.x, parentStartPos.x + bottomRight.x - halfSize.x);
            newPos.y = Mathf.Clamp(newPos.y, parentStartPos.y + bottomRight.y + halfSize.y, parentStartPos.y + topLeft.y - halfSize.y);

            // Move the object
            rb.MovePosition(newPos);

            ChangingHeight?.Invoke(gameObject, newPos.y - lastFrameHeight);
            lastFrameHeight = newPos.y;

            // if the object hits the top of the border, do jump thing
            bool atTop = Mathf.Abs((newPos.y + halfSize.y) - (parentStartPos.y + topLeft.y)) <= 0.00001f;
            if (!hitTop && atTop)
            {
                hitTop = true;
                ReachedMaxHeight?.Invoke(gameObject);
                topParticles.Play();
            }
            else if (!atTop)
            {
                hitTop = false;
            }
        }

        // stop moving when mouse released
        if (moving && Input.GetMouseButtonUp(0))
        {
            smoothedDeltaX = 0f;
            smoothedDeltaY = 0f;
            moving = false;
            for (int i = 0; i < 4; i++)
            {
                bordersLogic[i].SetDisabled();
            }
        }
    }

    bool ObjectClicked()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Bounds bounds = spriteRenderer.bounds;

        if (mouseWorld.x > bounds.min.x && mouseWorld.x < bounds.max.x && mouseWorld.y > bounds.min.y && mouseWorld.y < bounds.max.y)
        {
            moving = true;
            return true;
        }
        return false;
    }
}
