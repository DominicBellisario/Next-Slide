using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Edge : MonoBehaviour
{
    [SerializeField] Color selectedColor;
    SpriteRenderer sprite;
    Color startColor;
    ParticleSystem ps;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        ps = GetComponentInChildren<ParticleSystem>();
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

    public void PlayParticle()
    {
        var shape = ps.shape;
        shape.radius = sprite.size.x / 2;

        ps.Play();
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
