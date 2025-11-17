using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class ObjectScale : MonoBehaviour
{
    public static event Action<GameObject> ReachedMaxHeight;
    public static event Action<GameObject, float> ChangingHeight;

    #region constants
    const int NONE = -1;
    const int TOP = 0;
    const int RIGHT = 1;
    const int BOTTOM = 2;
    const int LEFT = 3;
    const int TOPLEFT = 0;
    const int TOPRIGHT = 1;
    const int BOTTOMRIGHT = 2;
    const int BOTTOMLEFT = 3;
    #endregion

    [SerializeField] float clickRange;
    [SerializeField] float maxResizeSpeed;
    [SerializeField] float minWidth = 0.5f;
    [SerializeField] float maxWidth = 10f;
    [SerializeField] float minHeight = 0.5f;
    [SerializeField] float maxHeight = 10f;
    [SerializeField] bool[] editableEdges;
    [SerializeField] GameObject[] knobs;
    [SerializeField] GameObject[] edges;
    [SerializeField] GameObject[] borders;

    Rigidbody2D rb;

    Vector3[] knobsStartPos;

    Vector3[] edgesStartPos;
    Vector2[] edgesStartSize;
    SpriteRenderer[] edgesSpriteRenderer;
    Edge[] edgesLogic;

    Vector3 objectStartPos;
    SpriteRenderer[] bordersSpriteRenderer;
    Edge[] bordersLogic;
    float maxWidthExcludingStartWidth;
    float maxHeightExcludingStartHeight;

    SpriteRenderer spriteRenderer;
    BoxCollider2D col;
    Vector3 startMousePos;
    Vector2 startSize;
    Vector3 startPos;
    int edgeBeingResized;

    Vector2 targetShapePos;
    Vector2 targetSpriteSize;
    Vector3[] targetKnobPos = new Vector3[4];
    Vector3[] targetEdgePos = new Vector3[4];
    Vector2[] targetEdgeSize = new Vector2[4];
    Vector3[] targetBorderPos = new Vector3[4];
    Vector2[] targetBorderSize = new Vector2[4];
    float lastFrameHeight;

    float smoothedDeltaX;
    float smoothedDeltaY;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        edgeBeingResized = NONE;
        knobsStartPos = new Vector3[4];
        edgesStartPos = new Vector3[4];
        edgesStartSize = new Vector2[4];
        edgesSpriteRenderer = new SpriteRenderer[4];
        edgesLogic = new Edge[4];
        bordersSpriteRenderer = new SpriteRenderer[4];
        bordersLogic = new Edge[4];
        objectStartPos = transform.position;
        maxWidthExcludingStartWidth = maxWidth - (spriteRenderer.size.x / 2);
        maxHeightExcludingStartHeight = maxHeight - (spriteRenderer.size.y / 2);
        targetShapePos = transform.position;
        lastFrameHeight = spriteRenderer.size.y;

        for (int i = 0; i < 4; i++)
        {
            edgesSpriteRenderer[i] = edges[i].GetComponent<SpriteRenderer>();
            edgesLogic[i] = edges[i].GetComponent<Edge>();

            bordersSpriteRenderer[i] = borders[i].GetComponent<SpriteRenderer>();
            bordersLogic[i] = borders[i].GetComponent<Edge>();

            // if an edge is not editable, make it invisible
            if (!editableEdges[i])
            {
                edgesLogic[i].SetDisabled();
            }
        }

        // set initial sizes and positions
        Vector2 size = spriteRenderer.size;
        col.size = size;

        knobs[TOPLEFT].transform.position = new Vector3(-size.x / 2, size.y / 2, 0f);
        knobs[TOPRIGHT].transform.position = new Vector3(size.x / 2, size.y / 2, 0f);
        knobs[BOTTOMLEFT].transform.position = new Vector3(-size.x / 2, -size.y / 2, 0f);
        knobs[BOTTOMRIGHT].transform.position = new Vector3(size.x / 2, -size.y / 2, 0f);

        edgesSpriteRenderer[TOP].size = new Vector2(size.x, 0.1f);
        edgesSpriteRenderer[BOTTOM].size = new Vector2(size.x, 0.1f);
        edgesSpriteRenderer[LEFT].size = new Vector2(0.1f, size.y);
        edgesSpriteRenderer[RIGHT].size = new Vector2(0.1f, size.y);

        edges[TOP].transform.position = new Vector3(transform.position.x, size.y / 2, 0f);
        edges[BOTTOM].transform.position = new Vector3(transform.position.x, -size.y / 2, 0f);
        edges[LEFT].transform.position = new Vector3(-size.x / 2, transform.position.y, 0f);
        edges[RIGHT].transform.position = new Vector3(size.x / 2, transform.position.y, 0f);

        // Initialize target values
        targetSpriteSize = spriteRenderer.size;
        targetShapePos = transform.position;

        for (int i = 0; i < 4; i++)
        {
            targetKnobPos[i] = knobs[i].transform.position;
            targetEdgePos[i] = edges[i].transform.position;
            targetEdgeSize[i] = edgesSpriteRenderer[i].size;
            targetBorderPos[i] = borders[i].transform.position;
            targetBorderSize[i] = bordersSpriteRenderer[i].size;
        }
    }

    void Update()
    {
        // Begin resizing if the mouse is down and near an editable edge
        if (Input.GetMouseButtonDown(0) && IsNearEdge())
        {
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startSize = spriteRenderer.size;
            startPos = transform.position;

            for (int i = 0; i < 4; i++)
            {
                knobsStartPos[i] = knobs[i].transform.position;
                edgesStartPos[i] = edges[i].transform.position;
                edgesStartSize[i] = edgesSpriteRenderer[i].size;
            }
        }

        // While holding down the mouse and actively resizing
        if (edgeBeingResized != NONE && Input.GetMouseButton(0))
        {
            Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float targetDeltaX = currentMousePos.x - startMousePos.x;
            float targetDeltaY = currentMousePos.y - startMousePos.y;

            // Smooth the deltas for more natural movement
            smoothedDeltaX = Mathf.MoveTowards(smoothedDeltaX, targetDeltaX, maxResizeSpeed * Time.deltaTime);
            smoothedDeltaY = Mathf.MoveTowards(smoothedDeltaY, targetDeltaY, maxResizeSpeed * Time.deltaTime);

            float newWidth = startSize.x;
            float newHeight = startSize.y;

            // Handle edge-specific logic and events
            if (edgeBeingResized == RIGHT)
            {
                newWidth = Mathf.Clamp(startSize.x + smoothedDeltaX, minWidth, maxWidth);
                smoothedDeltaX = newWidth - startSize.x;
            }
            else if (edgeBeingResized == LEFT)
            {
                newWidth = Mathf.Clamp(startSize.x - smoothedDeltaX, minWidth, maxWidth);
                smoothedDeltaX = startSize.x - newWidth;
            }
            else if (edgeBeingResized == TOP)
            {
                newHeight = Mathf.Clamp(startSize.y + smoothedDeltaY, minHeight, maxHeight);
                smoothedDeltaY = newHeight - startSize.y;

                if (newHeight == maxHeight && newHeight != lastFrameHeight)
                {
                    ReachedMaxHeight?.Invoke(gameObject);
                    bordersLogic[TOP].PlayParticle();
                } 
                else if (newHeight != lastFrameHeight)
                    ChangingHeight?.Invoke(gameObject, newHeight - lastFrameHeight);

                lastFrameHeight = newHeight;
            }
            else if (edgeBeingResized == BOTTOM)
            {
                newHeight = Mathf.Clamp(startSize.y - smoothedDeltaY, minHeight, maxHeight);
                smoothedDeltaY = startSize.y - newHeight;
            }

            // Store the target size and position for physics/visuals to apply in FixedUpdate
            targetSpriteSize = new Vector2(startSize.x, startSize.y);
            targetShapePos = startPos;

            if (edgeBeingResized == TOP)
            {
                targetSpriteSize.y = startSize.y + smoothedDeltaY;
                targetShapePos.y = startPos.y + (smoothedDeltaY / 2f);
                borders[TOP].transform.position = new Vector3(rb.position.x, objectStartPos.y + maxHeightExcludingStartHeight, 0f);
            }
            else if (edgeBeingResized == BOTTOM)
            {
                targetSpriteSize.y = startSize.y - smoothedDeltaY;
                targetShapePos.y = startPos.y + (smoothedDeltaY / 2f);
                borders[BOTTOM].transform.position = new Vector3(rb.position.x, objectStartPos.y - maxHeightExcludingStartHeight, 0f);
            }
            else if (edgeBeingResized == RIGHT)
            {
                targetSpriteSize.x = startSize.x + smoothedDeltaX;
                targetShapePos.x = startPos.x + (smoothedDeltaX / 2f);
                borders[RIGHT].transform.position = new Vector3(objectStartPos.x + maxWidthExcludingStartWidth, rb.position.y, 0f);
            }
            else if (edgeBeingResized == LEFT)
            {
                targetSpriteSize.x = startSize.x - smoothedDeltaX;
                targetShapePos.x = startPos.x + (smoothedDeltaX / 2f);
                borders[LEFT].transform.position = new Vector3(objectStartPos.x - maxWidthExcludingStartWidth, rb.position.y, 0f);
            }
        }

        // Stop resizing when mouse released
        if (edgeBeingResized != NONE && Input.GetMouseButtonUp(0))
        {
            smoothedDeltaX = 0f;
            smoothedDeltaY = 0f;
            edgesLogic[edgeBeingResized].ChangeToIdleColor();
            bordersLogic[edgeBeingResized].SetDisabled();
            edgeBeingResized = NONE;
        }
    }

    void FixedUpdate()
    {
        // Smoothly apply target position and size
        rb.MovePosition(targetShapePos);
        spriteRenderer.size = targetSpriteSize;
        col.size = targetSpriteSize;

        // Update visuals (knobs, edges, borders)
        UpdateVisuals(rb.position);
    }

    void UpdateVisuals(Vector2 physicsPos)
    {
        Vector2 size = spriteRenderer.size;
        Vector3 pos = physicsPos;

        // Update collider
        col.size = size;
        col.offset = Vector2.zero;

        // Update knob positions relative to object center
        knobs[TOPLEFT].transform.position = pos + new Vector3(-size.x / 2f, size.y / 2f, 0f);
        knobs[TOPRIGHT].transform.position = pos + new Vector3(size.x / 2f, size.y / 2f, 0f);
        knobs[BOTTOMLEFT].transform.position = pos + new Vector3(-size.x / 2f, -size.y / 2f, 0f);
        knobs[BOTTOMRIGHT].transform.position = pos + new Vector3(size.x / 2f, -size.y / 2f, 0f);

        // Update edge sprite sizes and positions
        edgesSpriteRenderer[TOP].size = new Vector2(size.x, edgesSpriteRenderer[TOP].size.y);
        edgesSpriteRenderer[BOTTOM].size = new Vector2(size.x, edgesSpriteRenderer[BOTTOM].size.y);
        edgesSpriteRenderer[LEFT].size = new Vector2(edgesSpriteRenderer[LEFT].size.x, size.y);
        edgesSpriteRenderer[RIGHT].size = new Vector2(edgesSpriteRenderer[RIGHT].size.x, size.y);

        edges[TOP].transform.position = pos + new Vector3(0f, size.y / 2f, 0f);
        edges[BOTTOM].transform.position = pos + new Vector3(0f, -size.y / 2f, 0f);
        edges[LEFT].transform.position = pos + new Vector3(-size.x / 2f, 0f, 0f);
        edges[RIGHT].transform.position = pos + new Vector3(size.x / 2f, 0f, 0f);

        // Update border sizes
        bordersSpriteRenderer[TOP].size = new Vector2(size.x, bordersSpriteRenderer[TOP].size.y);
        bordersSpriteRenderer[BOTTOM].size = new Vector2(size.x, bordersSpriteRenderer[BOTTOM].size.y);
        bordersSpriteRenderer[LEFT].size = new Vector2(bordersSpriteRenderer[LEFT].size.x, size.y);
        bordersSpriteRenderer[RIGHT].size = new Vector2(bordersSpriteRenderer[RIGHT].size.x, size.y);
    }

    /// <summary>
    /// checks if the mouse is close to an edge and sets whatever edge it is near
    /// </summary>
    /// <returns>wether or not the mouse is near an edge</returns>
    bool IsNearEdge()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Bounds bounds = spriteRenderer.bounds;

        // top edge: near top AND within horizontal range
        if (editableEdges[TOP] &&
            Mathf.Abs(mouseWorld.y - bounds.max.y) < clickRange &&
            mouseWorld.x > bounds.min.x &&
            mouseWorld.x < bounds.max.x)
        {
            edgeBeingResized = TOP;
            edgesLogic[TOP].ChangeToSelectedColor();
            bordersLogic[TOP].SetEnabled();
            return true;
        }
        // right edge: near right AND within vertical range
        else if (editableEdges[RIGHT] &&
            Mathf.Abs(mouseWorld.x - bounds.max.x) < clickRange &&
            mouseWorld.y > bounds.min.y &&
            mouseWorld.y < bounds.max.y)
        {
            edgeBeingResized = RIGHT;
            edgesLogic[RIGHT].ChangeToSelectedColor();
            bordersLogic[RIGHT].SetEnabled();
            return true;
        }
        // bottom edge: near bottom AND within horizontal range
        else if (editableEdges[BOTTOM] &&
            Mathf.Abs(mouseWorld.y - bounds.min.y) < clickRange &&
            mouseWorld.x > bounds.min.x &&
            mouseWorld.x < bounds.max.x)
        {
            edgeBeingResized = BOTTOM;
            edgesLogic[BOTTOM].ChangeToSelectedColor();
            bordersLogic[BOTTOM].SetEnabled();
            return true;
        }
        // left edge: near left AND within vertical range
        else if (editableEdges[LEFT] &&
            Mathf.Abs(mouseWorld.x - bounds.min.x) < clickRange &&
            mouseWorld.y > bounds.min.y &&
            mouseWorld.y < bounds.max.y)
        {
            edgeBeingResized = LEFT;
            edgesLogic[LEFT].ChangeToSelectedColor();
            bordersLogic[LEFT].SetEnabled();
            return true;
        }
        return false;
    }
}

