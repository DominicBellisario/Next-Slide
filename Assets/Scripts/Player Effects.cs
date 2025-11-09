using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerEffects : MonoBehaviour
{
    [SerializeField] Color impactColor;
    SpriteRenderer spriteRenderer;
    Color startColor;

    void OnEnable()
    {
        PlayerMovement.NormalState += NormalStateEffects;
        PlayerMovement.ImpactState += ImpactStateEffects;
    }
    void OnDisable()
    {
        PlayerMovement.NormalState -= NormalStateEffects;
        PlayerMovement.ImpactState -= ImpactStateEffects;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
    }

    private void ImpactStateEffects()
    {
        spriteRenderer.color = impactColor;
    }
    private void NormalStateEffects()
    {
        spriteRenderer.color = startColor;
    }
}
