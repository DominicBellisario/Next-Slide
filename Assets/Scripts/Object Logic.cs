using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class ObjectLogic : MonoBehaviour
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
    EdgeLogic[] edgesLogic;

    Vector3 objectStartPos;
    SpriteRenderer[] bordersSpriteRenderer;
    EdgeLogic[] bordersLogic;
    float maxWidthExcludingStartWidth;
    float maxHeightExcludingStartHeight;

    SpriteRenderer spriteRenderer;
    BoxCollider2D col;
    Vector3 startMousePos;
    Vector2 startSize;
    Vector3 startPos;
    int edgeBeingResized;

    Vector2 targetPos;
    float lastFrameHeight;

    float smoothedDeltaX;
    float smoothedDeltaY;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        edgeBeingResized = 0;
        knobsStartPos = new Vector3[4];
        edgesStartPos = new Vector3[4];
        edgesStartSize = new Vector2[4];
        edgesSpriteRenderer = new SpriteRenderer[4];
        edgesLogic = new EdgeLogic[4];
        bordersSpriteRenderer = new SpriteRenderer[4];
        bordersLogic = new EdgeLogic[4];
        objectStartPos = transform.position;
        maxWidthExcludingStartWidth = maxWidth - (spriteRenderer.size.x / 2);
        maxHeightExcludingStartHeight = maxHeight - (spriteRenderer.size.y / 2);
        targetPos = transform.position;
        lastFrameHeight = spriteRenderer.size.y;
        for (int i = 0; i < 4; i++)
        {
            edgesSpriteRenderer[i] = edges[i].GetComponent<SpriteRenderer>();
            edgesLogic[i] = edges[i].GetComponent<EdgeLogic>();

            bordersSpriteRenderer[i] = borders[i].GetComponent<SpriteRenderer>();
            bordersLogic[i] = borders[i].GetComponent<EdgeLogic>();

            // if an edge is not editable, make it invisible
            if (!editableEdges[i])
            {
                edgesLogic[i].SetDisabled();
            }
        }

        // set inital sizes and positions
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
    }

    void Update()
    {
        // if the mouse is down and near one of this shape's edges
        if (Input.GetMouseButtonDown(0) && IsNearEdge()) // IsNearEdge determines what edge is being resized
        {
            // record start values
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

        // if an edge is selected and the mouse is held down
        if (edgeBeingResized != NONE && Input.GetMouseButton(0))
        {
            Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float targetDeltaX = currentMousePos.x - startMousePos.x;
            float targetDeltaY = currentMousePos.y - startMousePos.y;

            // Gradually move toward the target delta
            smoothedDeltaX = Mathf.MoveTowards(smoothedDeltaX, targetDeltaX, maxResizeSpeed * Time.deltaTime);
            smoothedDeltaY = Mathf.MoveTowards(smoothedDeltaY, targetDeltaY, maxResizeSpeed * Time.deltaTime);

            // Use these for resizing
            float deltaX = smoothedDeltaX;
            float deltaY = smoothedDeltaY;
            float newWidth;
            float newHeight;

            // Predict new size for each edge and clamp if necessary
            if (edgeBeingResized == RIGHT)
            {
                newWidth = Mathf.Clamp(startSize.x + deltaX, minWidth, maxWidth);
                smoothedDeltaX = newWidth - startSize.x;
            }
            else if (edgeBeingResized == LEFT)
            {
                newWidth = Mathf.Clamp(startSize.x - deltaX, minWidth, maxWidth);
                smoothedDeltaX = startSize.x - newWidth;
            }
            else if (edgeBeingResized == TOP)
            {
                newHeight = Mathf.Clamp(startSize.y + deltaY, minHeight, maxHeight);
                smoothedDeltaY = newHeight - startSize.y;
                if (newHeight == maxHeight && newHeight != lastFrameHeight)
                {
                    ReachedMaxHeight?.Invoke(gameObject);
                }
                else if (newHeight != lastFrameHeight)
                {
                    ChangingHeight?.Invoke(gameObject, newHeight - lastFrameHeight);
                }
                lastFrameHeight = newHeight;
            }
            else if (edgeBeingResized == BOTTOM)
            {
                newHeight = Mathf.Clamp(startSize.y - deltaY, minHeight, maxHeight);
                smoothedDeltaY = startSize.y - newHeight;
            }

            if (edgeBeingResized == TOP)
            {
                // update the main shape's size and position
                spriteRenderer.size = new Vector2(startSize.x, startSize.y + smoothedDeltaY);
                targetPos = new Vector3(startPos.x, startPos.y + (smoothedDeltaY / 2), startPos.z);
                // update the knobs position.  they should always stay in the corners
                knobs[TOPLEFT].transform.position = new Vector3(knobsStartPos[TOPLEFT].x, knobsStartPos[TOPLEFT].y + smoothedDeltaY, knobsStartPos[TOPLEFT].z);
                knobs[TOPRIGHT].transform.position = new Vector3(knobsStartPos[TOPRIGHT].x, knobsStartPos[TOPRIGHT].y + smoothedDeltaY, knobsStartPos[TOPRIGHT].z);
                knobs[BOTTOMLEFT].transform.position = knobsStartPos[BOTTOMLEFT];
                knobs[BOTTOMRIGHT].transform.position = knobsStartPos[BOTTOMRIGHT];
                // update the edges size.
                edgesSpriteRenderer[LEFT].size = new Vector2(edgesStartSize[LEFT].x, edgesStartSize[LEFT].y + smoothedDeltaY);
                edgesSpriteRenderer[RIGHT].size = new Vector2(edgesStartSize[RIGHT].x, edgesStartSize[RIGHT].y + smoothedDeltaY);
                // update the edges positon.  they should always stay on the edge of the shape.
                edges[TOP].transform.position = new Vector3(edgesStartPos[TOP].x, edgesStartPos[TOP].y + smoothedDeltaY, edgesStartPos[TOP].z);
                edges[BOTTOM].transform.position = edgesStartPos[BOTTOM];
                edges[LEFT].transform.position = new Vector3(edgesStartPos[LEFT].x, edgesStartPos[LEFT].y + (smoothedDeltaY / 2), edgesStartPos[LEFT].z);
                edges[RIGHT].transform.position = new Vector3(edgesStartPos[RIGHT].x, edgesStartPos[RIGHT].y + (smoothedDeltaY / 2), edgesStartPos[RIGHT].z);
                // update the borders size.
                bordersSpriteRenderer[TOP].size = new Vector2(edgesStartSize[TOP].x, edgesStartSize[TOP].y);
                // update the borders positon.  they should always stay at the max size
                borders[TOP].transform.position = new Vector3(transform.position.x, objectStartPos.y + maxHeightExcludingStartHeight, 0f);
            }
            else if (edgeBeingResized == RIGHT)
            {
                spriteRenderer.size = new Vector2(startSize.x + smoothedDeltaX, startSize.y);
                targetPos = new Vector3(startPos.x + (smoothedDeltaX / 2), startPos.y, startPos.z);

                knobs[TOPRIGHT].transform.position = new Vector3(knobsStartPos[TOPRIGHT].x + smoothedDeltaX, knobsStartPos[TOPRIGHT].y, knobsStartPos[TOPRIGHT].z);
                knobs[BOTTOMRIGHT].transform.position = new Vector3(knobsStartPos[BOTTOMRIGHT].x + smoothedDeltaX, knobsStartPos[BOTTOMRIGHT].y, knobsStartPos[BOTTOMRIGHT].z);
                knobs[BOTTOMLEFT].transform.position = knobsStartPos[BOTTOMLEFT];
                knobs[TOPLEFT].transform.position = knobsStartPos[TOPLEFT];

                edgesSpriteRenderer[TOP].size = new Vector2(edgesStartSize[TOP].x + smoothedDeltaX, edgesStartSize[TOP].y);
                edgesSpriteRenderer[BOTTOM].size = new Vector2(edgesStartSize[BOTTOM].x + smoothedDeltaX, edgesStartSize[BOTTOM].y);

                edges[TOP].transform.position = new Vector3(edgesStartPos[TOP].x + (smoothedDeltaX / 2), edgesStartPos[TOP].y, edgesStartPos[TOP].z);
                edges[BOTTOM].transform.position = new Vector3(edgesStartPos[BOTTOM].x + (smoothedDeltaX / 2), edgesStartPos[BOTTOM].y, edgesStartPos[BOTTOM].z);
                edges[LEFT].transform.position = edgesStartPos[LEFT];
                edges[RIGHT].transform.position = new Vector3(edgesStartPos[RIGHT].x + smoothedDeltaX, edgesStartPos[RIGHT].y, edgesStartPos[RIGHT].z);

                bordersSpriteRenderer[RIGHT].size = new Vector2(edgesStartSize[RIGHT].x, edgesStartSize[RIGHT].y);

                borders[RIGHT].transform.position = new Vector3(objectStartPos.x + maxWidthExcludingStartWidth, transform.position.y, 0f);
            }
            else if (edgeBeingResized == BOTTOM)
            {
                spriteRenderer.size = new Vector2(startSize.x, startSize.y - smoothedDeltaY);
                targetPos = new Vector3(startPos.x, startPos.y + (smoothedDeltaY / 2), startPos.z);

                knobs[BOTTOMRIGHT].transform.position = new Vector3(knobsStartPos[BOTTOMRIGHT].x, knobsStartPos[BOTTOMRIGHT].y + smoothedDeltaY, knobsStartPos[BOTTOMRIGHT].z);
                knobs[BOTTOMLEFT].transform.position = new Vector3(knobsStartPos[BOTTOMLEFT].x, knobsStartPos[BOTTOMLEFT].y + smoothedDeltaY, knobsStartPos[BOTTOMLEFT].z);
                knobs[TOPLEFT].transform.position = knobsStartPos[TOPLEFT];
                knobs[TOPRIGHT].transform.position = knobsStartPos[TOPRIGHT];

                edgesSpriteRenderer[LEFT].size = new Vector2(edgesStartSize[LEFT].x, edgesStartSize[LEFT].y - smoothedDeltaY);
                edgesSpriteRenderer[RIGHT].size = new Vector2(edgesStartSize[RIGHT].x, edgesStartSize[RIGHT].y - smoothedDeltaY);

                edges[TOP].transform.position = edgesStartPos[TOP];
                edges[BOTTOM].transform.position = new Vector3(edgesStartPos[BOTTOM].x, edgesStartPos[BOTTOM].y + smoothedDeltaY, edgesStartPos[BOTTOM].z);
                edges[LEFT].transform.position = new Vector3(edgesStartPos[LEFT].x, edgesStartPos[LEFT].y + (smoothedDeltaY / 2), edgesStartPos[LEFT].z);
                edges[RIGHT].transform.position = new Vector3(edgesStartPos[RIGHT].x, edgesStartPos[RIGHT].y + (smoothedDeltaY / 2), edgesStartPos[RIGHT].z);

                bordersSpriteRenderer[BOTTOM].size = new Vector2(edgesStartSize[BOTTOM].x, edgesStartSize[BOTTOM].y);

                borders[BOTTOM].transform.position = new Vector3(transform.position.x, objectStartPos.y - maxHeightExcludingStartHeight, 0f);
            }
            else if (edgeBeingResized == LEFT)
            {
                spriteRenderer.size = new Vector2(startSize.x - smoothedDeltaX, startSize.y);
                targetPos = new Vector3(startPos.x + (smoothedDeltaX / 2), startPos.y, startPos.z);

                knobs[BOTTOMLEFT].transform.position = new Vector3(knobsStartPos[BOTTOMLEFT].x + smoothedDeltaX, knobsStartPos[BOTTOMLEFT].y, knobsStartPos[BOTTOMLEFT].z);
                knobs[TOPLEFT].transform.position = new Vector3(knobsStartPos[TOPLEFT].x + smoothedDeltaX, knobsStartPos[TOPLEFT].y, knobsStartPos[TOPLEFT].z);
                knobs[TOPRIGHT].transform.position = knobsStartPos[TOPRIGHT];
                knobs[BOTTOMRIGHT].transform.position = knobsStartPos[BOTTOMRIGHT];

                edgesSpriteRenderer[TOP].size = new Vector2(edgesStartSize[TOP].x - smoothedDeltaX, edgesStartSize[TOP].y);
                edgesSpriteRenderer[BOTTOM].size = new Vector2(edgesStartSize[BOTTOM].x - smoothedDeltaX, edgesStartSize[BOTTOM].y);

                edges[TOP].transform.position = new Vector3(edgesStartPos[TOP].x + (smoothedDeltaX / 2), edgesStartPos[TOP].y, edgesStartPos[TOP].z);
                edges[BOTTOM].transform.position = new Vector3(edgesStartPos[BOTTOM].x + (smoothedDeltaX / 2), edgesStartPos[BOTTOM].y, edgesStartPos[BOTTOM].z);
                edges[LEFT].transform.position = new Vector3(edgesStartPos[LEFT].x + smoothedDeltaX, edgesStartPos[LEFT].y, edgesStartPos[LEFT].z);
                edges[RIGHT].transform.position = edgesStartPos[RIGHT];

                bordersSpriteRenderer[LEFT].size = new Vector2(edgesStartSize[LEFT].x, edgesStartSize[LEFT].y);

                borders[LEFT].transform.position = new Vector3(objectStartPos.x - maxWidthExcludingStartWidth, transform.position.y, 0f);
            }
        }

        // if an edge was being resized and the mouse was released
        if (edgeBeingResized != NONE && Input.GetMouseButtonUp(0))
        {
            smoothedDeltaX = 0f;
            smoothedDeltaY = 0f;
            // chnage the color of the selected edge back to normal
            edgesLogic[edgeBeingResized].ChangeToIdleColor();
            bordersLogic[edgeBeingResized].SetDisabled();
            edgeBeingResized = NONE;
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(targetPos);
        col.size = spriteRenderer.size;
    }

    /// <summary>
    /// checks if the mouse is close to an edge and sets whatever edge it is near
    /// </summary>
    /// <returns>wether or not the mouse is near an edge</returns>
    bool IsNearEdge()
    {

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Bounds bounds = spriteRenderer.bounds;

        // close to top
        if (editableEdges[TOP] && Mathf.Abs(mouseWorld.y - bounds.max.y) < clickRange)
        {
            edgeBeingResized = TOP;
            edgesLogic[TOP].ChangeToSelectedColor();
            bordersLogic[TOP].SetEnabled();
            return true;
        }
        // close to right
        else if (editableEdges[RIGHT] && Mathf.Abs(mouseWorld.x - bounds.max.x) < clickRange)
        {
            edgeBeingResized = RIGHT;
            edgesLogic[RIGHT].ChangeToSelectedColor();
            bordersLogic[RIGHT].SetEnabled();
            return true;
        }
        // close to bottom
        else if (editableEdges[BOTTOM] && Mathf.Abs(mouseWorld.y - bounds.min.y) < clickRange)
        {
            edgeBeingResized = BOTTOM;
            edgesLogic[BOTTOM].ChangeToSelectedColor();
            bordersLogic[BOTTOM].SetEnabled();
            return true;
        }
        // close to left
        else if (editableEdges[LEFT] && Mathf.Abs(mouseWorld.x - bounds.min.x) < clickRange)
        {
            edgeBeingResized = LEFT;
            edgesLogic[LEFT].ChangeToSelectedColor();
            bordersLogic[LEFT].SetEnabled();
            return true;
        }
        return false;
    }
}

