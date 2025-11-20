using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class ObjectImpact : MonoBehaviour
{
    Vector3 startPos;
    BoxCollider2D col;
    ParticleSystem pSystem;
    SpriteRenderer sprite;
    [SerializeField] float shakeDuration;
    [SerializeField] float shakeMagnitude;

    void Start()
    {
        startPos = transform.position;
        col = GetComponent<BoxCollider2D>();
        pSystem = GetComponentInChildren<ParticleSystem>();
        sprite = GetComponent<SpriteRenderer>();

        col.size = sprite.size;
        var shape = pSystem.shape;
        shape.scale = new Vector3(0f, sprite.size.y, sprite.size.x);
    }

    public void HitObject()
    {
        StartCoroutine(Shake());
    }

    public void DestroyObject()
    {
        pSystem.Play();
        pSystem.transform.parent = null;
        Destroy(gameObject);
    }
    
    private IEnumerator Shake()
    {
        float t = 0f;

        while (t < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeDuration;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = new Vector3(startPos.x + x, startPos.y + y, startPos.z);

            t += Time.deltaTime;
            yield return null;
        }

        transform.position = startPos;
    }
}
