using UnityEngine;

public class Afterimage : MonoBehaviour
{
    SpriteRenderer sr;
    Color color;
    [SerializeField] float lifetime = 0.3f;
    [SerializeField] float fadeSpeed = 5f;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        color = sr.color;
    }

    void Update()
    {
        //fade out and destroy
        color.a -= fadeSpeed * Time.deltaTime;
        sr.color = color;

        if (color.a <= 0f) { Destroy(gameObject); }
    }
}

