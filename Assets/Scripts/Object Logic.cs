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
    [SerializeField] GameObject[] edges;

    Vector3[] knobsStartPos;
    Vector3[] edgesStartPos;
    Vector2[] edgesStartSize;
    SpriteRenderer[] edgesSpriteRenderer;
    EdgeLogic[] edgesLogic;
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
        edgesStartPos = new Vector3[4];
        edgesStartSize = new Vector2[4];
        edgesSpriteRenderer = new SpriteRenderer[4];
        edgesLogic = new EdgeLogic[4];
        for (int i = 0; i < 4; i++)
        {
            edgesSpriteRenderer[i] = edges[i].GetComponent<SpriteRenderer>();
            edgesLogic[i] = edges[i].GetComponent<EdgeLogic>();
        }
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
                edgesStartPos[i] = edges[i].transform.position;
                edgesStartSize[i] = edgesSpriteRenderer[i].size;
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

                edgesSpriteRenderer[LEFT].size = new Vector2(edgesStartSize[LEFT].x, edgesStartSize[LEFT].y + deltaY);
                edgesSpriteRenderer[RIGHT].size = new Vector2(edgesStartSize[RIGHT].x, edgesStartSize[RIGHT].y + deltaY);

                edges[TOP].transform.position = new Vector3(edgesStartPos[TOP].x, edgesStartPos[TOP].y + deltaY, edgesStartPos[TOP].z);
                edges[BOTTOM].transform.position = edgesStartPos[BOTTOM];
                edges[LEFT].transform.position = new Vector3(edgesStartPos[LEFT].x, edgesStartPos[LEFT].y + (deltaY / 2), edgesStartPos[LEFT].z);
                edges[RIGHT].transform.position = new Vector3(edgesStartPos[RIGHT].x, edgesStartPos[RIGHT].y + (deltaY / 2), edgesStartPos[RIGHT].z);   
            }
            else if (edgeBeingResized == RIGHT)
            {
                spriteRenderer.size = new Vector2(startSize.x + deltaX, startSize.y);
                transform.position = new Vector3(startPos.x + (deltaX / 2), startPos.y, startPos.z);

                knobs[TOPRIGHT].transform.position = new Vector3(knobsStartPos[TOPRIGHT].x + deltaX, knobsStartPos[TOPRIGHT].y, knobsStartPos[TOPRIGHT].z);
                knobs[BOTTOMRIGHT].transform.position = new Vector3(knobsStartPos[BOTTOMRIGHT].x + deltaX, knobsStartPos[BOTTOMRIGHT].y, knobsStartPos[BOTTOMRIGHT].z);
                knobs[BOTTOMLEFT].transform.position = knobsStartPos[BOTTOMLEFT];
                knobs[TOPLEFT].transform.position = knobsStartPos[TOPLEFT];

                edgesSpriteRenderer[TOP].size = new Vector2(edgesStartSize[TOP].x + deltaX, edgesStartSize[TOP].y);
                edgesSpriteRenderer[BOTTOM].size = new Vector2(edgesStartSize[BOTTOM].x + deltaX, edgesStartSize[BOTTOM].y);

                edges[TOP].transform.position = new Vector3(edgesStartPos[TOP].x + (deltaX / 2), edgesStartPos[TOP].y, edgesStartPos[TOP].z);
                edges[BOTTOM].transform.position = new Vector3(edgesStartPos[BOTTOM].x + (deltaX / 2), edgesStartPos[BOTTOM].y, edgesStartPos[BOTTOM].z);
                edges[LEFT].transform.position = edgesStartPos[LEFT];
                edges[RIGHT].transform.position = new Vector3(edgesStartPos[RIGHT].x + deltaX, edgesStartPos[RIGHT].y, edgesStartPos[RIGHT].z);
            }
            else if (edgeBeingResized == BOTTOM)
            {
                spriteRenderer.size = new Vector2(startSize.x, startSize.y - deltaY);
                transform.position = new Vector3(startPos.x, startPos.y + (deltaY / 2), startPos.z);

                knobs[BOTTOMRIGHT].transform.position = new Vector3(knobsStartPos[BOTTOMRIGHT].x, knobsStartPos[BOTTOMRIGHT].y + deltaY, knobsStartPos[BOTTOMRIGHT].z);
                knobs[BOTTOMLEFT].transform.position = new Vector3(knobsStartPos[BOTTOMLEFT].x, knobsStartPos[BOTTOMLEFT].y + deltaY, knobsStartPos[BOTTOMLEFT].z);
                knobs[TOPLEFT].transform.position = knobsStartPos[TOPLEFT];
                knobs[TOPRIGHT].transform.position = knobsStartPos[TOPRIGHT];

                edgesSpriteRenderer[LEFT].size = new Vector2(edgesStartSize[LEFT].x, edgesStartSize[LEFT].y - deltaY);
                edgesSpriteRenderer[RIGHT].size = new Vector2(edgesStartSize[RIGHT].x, edgesStartSize[RIGHT].y - deltaY);

                edges[TOP].transform.position = edgesStartPos[TOP];
                edges[BOTTOM].transform.position = new Vector3(edgesStartPos[BOTTOM].x, edgesStartPos[BOTTOM].y + deltaY, edgesStartPos[BOTTOM].z);
                edges[LEFT].transform.position = new Vector3(edgesStartPos[LEFT].x, edgesStartPos[LEFT].y + (deltaY / 2), edgesStartPos[LEFT].z);
                edges[RIGHT].transform.position = new Vector3(edgesStartPos[RIGHT].x, edgesStartPos[RIGHT].y + (deltaY / 2), edgesStartPos[RIGHT].z);
            }
            else if (edgeBeingResized == LEFT)
            {
                spriteRenderer.size = new Vector2(startSize.x - deltaX, startSize.y);
                transform.position = new Vector3(startPos.x + (deltaX / 2), startPos.y, startPos.z);

                knobs[BOTTOMLEFT].transform.position = new Vector3(knobsStartPos[BOTTOMLEFT].x + deltaX, knobsStartPos[BOTTOMLEFT].y, knobsStartPos[BOTTOMLEFT].z);
                knobs[TOPLEFT].transform.position = new Vector3(knobsStartPos[TOPLEFT].x + deltaX, knobsStartPos[TOPLEFT].y, knobsStartPos[TOPLEFT].z);
                knobs[TOPRIGHT].transform.position = knobsStartPos[TOPRIGHT];
                knobs[BOTTOMRIGHT].transform.position = knobsStartPos[BOTTOMRIGHT];

                edgesSpriteRenderer[TOP].size = new Vector2(edgesStartSize[TOP].x - deltaX, edgesStartSize[TOP].y);
                edgesSpriteRenderer[BOTTOM].size = new Vector2(edgesStartSize[BOTTOM].x - deltaX, edgesStartSize[BOTTOM].y);

                edges[TOP].transform.position = new Vector3(edgesStartPos[TOP].x + (deltaX / 2), edgesStartPos[TOP].y, edgesStartPos[TOP].z);
                edges[BOTTOM].transform.position = new Vector3(edgesStartPos[BOTTOM].x + (deltaX / 2), edgesStartPos[BOTTOM].y, edgesStartPos[BOTTOM].z);
                edges[LEFT].transform.position = new Vector3(edgesStartPos[LEFT].x + deltaX, edgesStartPos[LEFT].y, edgesStartPos[LEFT].z);
                edges[RIGHT].transform.position = edgesStartPos[RIGHT];
            }
            col.size = spriteRenderer.size;
        }

        if (edgeBeingResized != NONE && Input.GetMouseButtonUp(0))
        {
            Debug.Log("STOP resizing");
            edgesLogic[edgeBeingResized].ChangeToIdleColor();
            edgeBeingResized = NONE;
        }
    }

    bool IsNearEdge()
    {
        // simple check if mouse is close to shape edge
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Bounds bounds = spriteRenderer.bounds;

        // close to top
        if (editableEdges[TOP] && Mathf.Abs(mouseWorld.y - bounds.max.y) < clickRange)
        {
            edgeBeingResized = TOP;
            edgesLogic[TOP].ChangeToSelectedColor();
            return true;
        }
        // close to right
        else if (editableEdges[RIGHT] && Mathf.Abs(mouseWorld.x - bounds.max.x) < clickRange)
        {
            edgeBeingResized = RIGHT;
            edgesLogic[RIGHT].ChangeToSelectedColor();
            return true;
        }
        // close to bottom
        else if (editableEdges[BOTTOM] && Mathf.Abs(mouseWorld.y - bounds.min.y) < clickRange)
        {
            edgeBeingResized = BOTTOM;
            edgesLogic[BOTTOM].ChangeToSelectedColor();
            return true;
        }
        // close to left
        else if (editableEdges[LEFT] && Mathf.Abs(mouseWorld.x - bounds.min.x) < clickRange)
        {
            edgeBeingResized = LEFT;
            edgesLogic[LEFT].ChangeToSelectedColor();
            return true;
        }
        return false;
    }
}

