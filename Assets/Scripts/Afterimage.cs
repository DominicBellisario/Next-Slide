using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Afterimage : MonoBehaviour
{
    SpriteRenderer sr;
    Color color;
    [SerializeField] float fadeTime;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        color = sr.color;
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float t = 0;
        while (t < 1)
        {
            color.a = Mathf.Lerp(1f, 0f, t);
            sr.color = color;
            t += Time.deltaTime / fadeTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}

