using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class ObjectLogic : MonoBehaviour
{
    const int NONE = -1;
    const int TOP = 0;
    const int RIGHT = 1;
    const int BOTTOM = 2;
    const int LEFT = 3;

    [SerializeField] float clickRange;
    /// <summary>
    /// TOP, RIGHT, BOTTOM, LEFT
    /// </summary>
    [SerializeField] bool[] editableEdges;
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
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsNearEdge())
        {
            Debug.Log("START resizing");
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startSize = spriteRenderer.size;
            startPos = transform.position;
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
            }
            else if (edgeBeingResized == RIGHT)
            {
                spriteRenderer.size = new Vector2(startSize.x + deltaX, startSize.y);
                transform.position = new Vector3(startPos.x + (deltaX / 2), startPos.y, startPos.z);
            }
            else if (edgeBeingResized == BOTTOM)
            {
                spriteRenderer.size = new Vector2(startSize.x, startSize.y - deltaY);
                transform.position = new Vector3(startPos.x, startPos.y + (deltaY / 2), startPos.z);
            }
            else if (edgeBeingResized == LEFT)
            {
                spriteRenderer.size = new Vector2(startSize.x - deltaX, startSize.y);
                transform.position = new Vector3(startPos.x + (deltaX / 2), startPos.y, startPos.z);
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

