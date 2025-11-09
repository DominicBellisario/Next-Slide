using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Edge : MonoBehaviour
{
    [SerializeField] Color selectedColor;
    SpriteRenderer sprite;
    Color startColor;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        startColor = sprite.color;
    }

    public void ChangeToSelectedColor()
    {
        sprite.color = selectedColor;
    }

    public void ChangeToIdleColor()
    {
        sprite.color = startColor;
    }

    public void SetDisabled()
    {
        sprite.enabled = false;
    }
    
    public void SetEnabled()
    {
        sprite.enabled = true;
    }
}
