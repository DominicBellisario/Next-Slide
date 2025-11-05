using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class ObjectLogic : MonoBehaviour
{
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
    [SerializeField] bool[] editableEdges;
    [SerializeField] GameObject[] knobs;
    [SerializeField] GameObject[] bars;

    Vector3[] knobsStartPos;
    Vector3[] barsStartPos;
    Vector2[] barsStartSize;
    SpriteRenderer[] barsSpriteRenderer;
    SpriteRenderer spriteRenderer;
    BoxCollider2D col;
    Vector3 startMousePos;
    Vector2 startSize;
    Vector3 startPos;
    int edgeBeingResized;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        edgeBeingResized = 0;
        knobsStartPos = new Vector3[4];
        barsStartPos = new Vector3[4];
        barsStartSize = new Vector2[4];
        barsSpriteRenderer = new SpriteRenderer[4];
        for (int i = 0; i < 4; i++) { barsSpriteRenderer[i] = bars[i].GetComponent<SpriteRenderer>(); }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsNearEdge())
        {
            Debug.Log("START resizing");
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startSize = spriteRenderer.size;
            startPos = transform.position;
            for (int i = 0; i < 4; i++)
            {
                knobsStartPos[i] = knobs[i].transform.position;
                barsStartPos[i] = bars[i].transform.position;
                barsStartSize[i] = barsSpriteRenderer[i].size;
            }
        }

        if (edgeBeingResized != NONE && Input.GetMouseButton(0))
        {
            Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float deltaX = currentMousePos.x - startMousePos.x; // horizontal drag
            float deltaY = currentMousePos.y - startMousePos.y; // vertical drag
            if (edgeBeingResized == TOP)
            {
                spriteRenderer.size = new Vector2(startSize.x, startSize.y + deltaY);
                transform.position = new Vector3(startPos.x, startPos.y + (deltaY / 2), startPos.z);

                knobs[TOPLEFT].transform.position = new Vector3(knobsStartPos[TOPLEFT].x, knobsStartPos[TOPLEFT].y + deltaY, knobsStartPos[TOPLEFT].z);
                knobs[TOPRIGHT].transform.position = new Vector3(knobsStartPos[TOPRIGHT].x, knobsStartPos[TOPRIGHT].y + deltaY, knobsStartPos[TOPRIGHT].z);
                knobs[BOTTOMLEFT].transform.position = knobsStartPos[BOTTOMLEFT];
                knobs[BOTTOMRIGHT].transform.position = knobsStartPos[BOTTOMRIGHT];

                barsSpriteRenderer[LEFT].size = new Vector2(barsStartSize[LEFT].x, barsStartSize[LEFT].y + deltaY);
                barsSpriteRenderer[RIGHT].size = new Vector2(barsStartSize[RIGHT].x, barsStartSize[RIGHT].y + deltaY);

                bars[TOP].transform.position = new Vector3(barsStartPos[TOP].x, barsStartPos[TOP].y + deltaY, barsStartPos[TOP].z);
                bars[BOTTOM].transform.position = barsStartPos[BOTTOM];
                bars[LEFT].transform.position = new Vector3(barsStartPos[LEFT].x, barsStartPos[LEFT].y + (deltaY / 2), barsStartPos[LEFT].z);
                bars[RIGHT].transform.position = new Vector3(barsStartPos[RIGHT].x, barsStartPos[RIGHT].y + (deltaY / 2), barsStartPos[RIGHT].z);   
            }
            else if (edgeBeingResized == RIGHT)
            {
                spriteRenderer.size = new Vector2(startSize.x + deltaX, startSize.y);
                transform.position = new Vector3(startPos.x + (deltaX / 2), startPos.y, startPos.z);

                knobs[TOPRIGHT].transform.position = new Vector3(knobsStartPos[TOPRIGHT].x + deltaX, knobsStartPos[TOPRIGHT].y, knobsStartPos[TOPRIGHT].z);
                knobs[BOTTOMRIGHT].transform.position = new Vector3(knobsStartPos[BOTTOMRIGHT].x + deltaX, knobsStartPos[BOTTOMRIGHT].y, knobsStartPos[BOTTOMRIGHT].z);
                knobs[BOTTOMLEFT].transform.position = knobsStartPos[BOTTOMLEFT];
                knobs[TOPLEFT].transform.position = knobsStartPos[TOPLEFT];

                barsSpriteRenderer[TOP].size = new Vector2(barsStartSize[TOP].x + deltaX, barsStartSize[TOP].y);
                barsSpriteRenderer[BOTTOM].size = new Vector2(barsStartSize[BOTTOM].x + deltaX, barsStartSize[BOTTOM].y);

                bars[TOP].transform.position = new Vector3(barsStartPos[TOP].x + (deltaX / 2), barsStartPos[TOP].y, barsStartPos[TOP].z);
                bars[BOTTOM].transform.position = new Vector3(barsStartPos[BOTTOM].x + (deltaX / 2), barsStartPos[BOTTOM].y, barsStartPos[BOTTOM].z);
                bars[LEFT].transform.position = barsStartPos[LEFT];
                bars[RIGHT].transform.position = new Vector3(barsStartPos[RIGHT].x + deltaX, barsStartPos[RIGHT].y, barsStartPos[RIGHT].z);
            }
            else if (edgeBeingResized == BOTTOM)
            {
                spriteRenderer.size = new Vector2(startSize.x, startSize.y - deltaY);
                transform.position = new Vector3(startPos.x, startPos.y + (deltaY / 2), startPos.z);

                knobs[BOTTOMRIGHT].transform.position = new Vector3(knobsStartPos[BOTTOMRIGHT].x, knobsStartPos[BOTTOMRIGHT].y + deltaY, knobsStartPos[BOTTOMRIGHT].z);
                knobs[BOTTOMLEFT].transform.position = new Vector3(knobsStartPos[BOTTOMLEFT].x, knobsStartPos[BOTTOMLEFT].y + deltaY, knobsStartPos[BOTTOMLEFT].z);
                knobs[TOPLEFT].transform.position = knobsStartPos[TOPLEFT];
                knobs[TOPRIGHT].transform.position = knobsStartPos[TOPRIGHT];

                barsSpriteRenderer[LEFT].size = new Vector2(barsStartSize[LEFT].x, barsStartSize[LEFT].y - deltaY);
                barsSpriteRenderer[RIGHT].size = new Vector2(barsStartSize[RIGHT].x, barsStartSize[RIGHT].y - deltaY);

                bars[TOP].transform.position = barsStartPos[TOP];
                bars[BOTTOM].transform.position = new Vector3(barsStartPos[BOTTOM].x, barsStartPos[BOTTOM].y + deltaY, barsStartPos[BOTTOM].z);
                bars[LEFT].transform.position = new Vector3(barsStartPos[LEFT].x, barsStartPos[LEFT].y + (deltaY / 2), barsStartPos[LEFT].z);
                bars[RIGHT].transform.position = new Vector3(barsStartPos[RIGHT].x, barsStartPos[RIGHT].y + (deltaY / 2), barsStartPos[RIGHT].z);
            }
            else if (edgeBeingResized == LEFT)
            {
                spriteRenderer.size = new Vector2(startSize.x - deltaX, startSize.y);
                transform.position = new Vector3(startPos.x + (deltaX / 2), startPos.y, startPos.z);

                knobs[BOTTOMLEFT].transform.position = new Vector3(knobsStartPos[BOTTOMLEFT].x + deltaX, knobsStartPos[BOTTOMLEFT].y, knobsStartPos[BOTTOMLEFT].z);
                knobs[TOPLEFT].transform.position = new Vector3(knobsStartPos[TOPLEFT].x + deltaX, knobsStartPos[TOPLEFT].y, knobsStartPos[TOPLEFT].z);
                knobs[TOPRIGHT].transform.position = knobsStartPos[TOPRIGHT];
                knobs[BOTTOMRIGHT].transform.position = knobsStartPos[BOTTOMRIGHT];

                barsSpriteRenderer[TOP].size = new Vector2(barsStartSize[TOP].x - deltaX, barsStartSize[TOP].y);
                barsSpriteRenderer[BOTTOM].size = new Vector2(barsStartSize[BOTTOM].x - deltaX, barsStartSize[BOTTOM].y);

                bars[TOP].transform.position = new Vector3(barsStartPos[TOP].x + (deltaX / 2), barsStartPos[TOP].y, barsStartPos[TOP].z);
                bars[BOTTOM].transform.position = new Vector3(barsStartPos[BOTTOM].x + (deltaX / 2), barsStartPos[BOTTOM].y, barsStartPos[BOTTOM].z);
                bars[LEFT].transform.position = new Vector3(barsStartPos[LEFT].x + deltaX, barsStartPos[LEFT].y, barsStartPos[LEFT].z);
                bars[RIGHT].transform.position = barsStartPos[RIGHT];
            }
            col.size = spriteRenderer.size;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("STOP resizing");
            edgeBeingResized = NONE;
        }
    }

    bool IsNearEdge()
    {
        // simple check if mouse is close to shape edge (for demo)
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Bounds bounds = spriteRenderer.bounds;

        // close to top
        if (editableEdges[TOP] && Mathf.Abs(mouseWorld.y - bounds.max.y) < clickRange)
        {
            edgeBeingResized = TOP;
            return true;
        }
        // close to right
        else if (editableEdges[RIGHT] && Mathf.Abs(mouseWorld.x - bounds.max.x) < clickRange)
        {
            edgeBeingResized = RIGHT;
            return true;
        }
        // close to bottom
        else if (editableEdges[BOTTOM] && Mathf.Abs(mouseWorld.y - bounds.min.y) < clickRange)
        {
            edgeBeingResized = BOTTOM;
            return true;
        }
        // close to left
        else if (editableEdges[LEFT] && Mathf.Abs(mouseWorld.x - bounds.min.x) < clickRange)
        {
            edgeBeingResized = LEFT;
            return true;
        }
        return false;
    }
}

