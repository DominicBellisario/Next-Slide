using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class SpeedPanel : MonoBehaviour
{
    [SerializeField] Color pulseColor;
    [SerializeField] float pulseTime;
    [SerializeField] AnimationCurve pulseCurve;
    ParticleSystem pSystem;
    SpriteRenderer sprite;
    Color startColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        GetComponent<BoxCollider2D>().size = sprite.size;
        startColor = sprite.color;
        pSystem = GetComponentInChildren<ParticleSystem>();
    }

    public void PlayEffects() { StartCoroutine(PlayEffectsCoroutine()); }

    private IEnumerator PlayEffectsCoroutine()
    {
        pSystem.Play();
        float t = 0;
        while (t < 1)
        {
            sprite.color = Color.Lerp(startColor, pulseColor, pulseCurve.Evaluate(t));
            t += Time.deltaTime / pulseTime;
            yield return null;
        }
        sprite.color = startColor;
    }
}
