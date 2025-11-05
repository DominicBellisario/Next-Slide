using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ObjectLogic : MonoBehaviour
{
    const int NONE = 0;
    const int TOP = 1;
    const int RIGHT = 2;
    const int BOTTOM = 3;
    const int LEFT = 4;

    [SerializeField] float clickRange;
    SpriteRenderer spriteRenderer;
    Vector3 startMousePos;
    Vector2 startSize;
    int edgeBeingResized;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        edgeBeingResized = 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsNearEdge())
        {
            Debug.Log("START resizing");
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startSize = spriteRenderer.size;
        }

        if (edgeBeingResized != NONE && Input.GetMouseButton(0))
        {
            Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float deltaX = currentMousePos.x - startMousePos.x; // horizontal drag
            float deltaY = currentMousePos.y - startMousePos.y; // vertical drag
            if (edgeBeingResized == TOP)
            {
                spriteRenderer.size = new Vector2(startSize.x, startSize.y + deltaY);
            }
            else if (edgeBeingResized == RIGHT)
            {
                spriteRenderer.size = new Vector2(startSize.x + deltaX, startSize.y);
            }
            else if (edgeBeingResized == BOTTOM)
            {
                spriteRenderer.size = new Vector2(startSize.x, startSize.y - deltaY);
            }
            else if (edgeBeingResized == LEFT)
            {
                spriteRenderer.size = new Vector2(startSize.x - deltaX, startSize.y);
            }

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
        if (Mathf.Abs(mouseWorld.y - bounds.max.y) < clickRange)
        {
            edgeBeingResized = TOP;
            return true;
        }
        // close to right
        else if (Mathf.Abs(mouseWorld.x - bounds.max.x) < clickRange)
        {
            edgeBeingResized = RIGHT;
            return true;
        }
        // close to bottom
        else if (Mathf.Abs(mouseWorld.y - bounds.min.y) < clickRange)
        {
            edgeBeingResized = BOTTOM;
            return true;
        }
        // close to left
        else if (Mathf.Abs(mouseWorld.x - bounds.min.x) < clickRange)
        {
            edgeBeingResized = LEFT;
            return true;
        }
        return false;
    }
}

