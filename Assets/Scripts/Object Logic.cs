using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ObjectLogic : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    bool isResizing;
    Vector3 startMousePos;
    Vector2 startSize;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsNearEdge())
        {
            Debug.Log("START resizing");
            isResizing = true;
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startSize = spriteRenderer.size;
        }

        if (isResizing && Input.GetMouseButton(0))
        {
            
            Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float delta = currentMousePos.x - startMousePos.x; // horizontal drag
            spriteRenderer.size = new Vector2(startSize.x + delta, startSize.y);
            Debug.Log("CURRENTLY resizing: " + spriteRenderer.size);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("STOP resizing");
            isResizing = false;
        }
    }

    bool IsNearEdge()
    {
        // simple check if mouse is close to shape edge (for demo)
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Bounds bounds = spriteRenderer.bounds;
        return Mathf.Abs(mouseWorld.x - bounds.max.x) < 0.5f;
    }
}

