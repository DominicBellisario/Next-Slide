using System.Collections;
using UnityEngine;

public class FlipLogic : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] GameObject topLeft;
    [SerializeField] GameObject topRight;
    [SerializeField] GameObject bottomLeft;
    [SerializeField] GameObject bottomRight;
    void OnEnable()
    {
        PlayerMovement.FlipH += PlayFlipEffects;
    }
    void OnDisable()
    {
        PlayerMovement.FlipH -= PlayFlipEffects;
    }

    private void PlayFlipEffects(float flipTime, AnimationCurve flipCurve)
    {
        StartCoroutine(PlayFlipEffectsCoroutine(flipTime, flipCurve));
    }

    private IEnumerator PlayFlipEffectsCoroutine(float flipTime, AnimationCurve flipCurve)
    {
        Vector2 startSize = sprite.size;
        Vector2 endSize = new(0f, sprite.size.y);

        topLeft.SetActive(true);
        topRight.SetActive(true);
        bottomLeft.SetActive(true);
        bottomRight.SetActive(true);
        
        float t = 0f;
        while (t < 0.5f)
        {
            sprite.size = Vector2.Lerp(startSize, endSize, flipCurve.Evaluate(t));

            topLeft.transform.localPosition = new Vector2(-sprite.size.x, sprite.size.y) / 2;
            topRight.transform.localPosition = new Vector2(sprite.size.x, sprite.size.y) / 2;
            bottomLeft.transform.localPosition = new Vector2(-sprite.size.x, -sprite.size.y) / 2;
            bottomRight.transform.localPosition = new Vector2(sprite.size.x, -sprite.size.y) / 2;

            t += Time.deltaTime / flipTime;
            yield return null;
        }
        sprite.flipX = !sprite.flipX;
        while (t < 1f)
        {
            sprite.size = Vector2.Lerp(startSize, endSize, flipCurve.Evaluate(t));

            topLeft.transform.localPosition = new Vector2(-sprite.size.x, sprite.size.y) / 2;
            topRight.transform.localPosition = new Vector2(sprite.size.x, sprite.size.y) / 2;
            bottomLeft.transform.localPosition = new Vector2(-sprite.size.x, -sprite.size.y) / 2;
            bottomRight.transform.localPosition = new Vector2(sprite.size.x, -sprite.size.y) / 2;

            t += Time.deltaTime / flipTime;
            yield return null;
        }
        sprite.size = startSize;
        topLeft.transform.localPosition = new Vector2(-sprite.size.x, sprite.size.y) / 2;
        topRight.transform.localPosition = new Vector2(sprite.size.x, sprite.size.y) / 2;
        bottomLeft.transform.localPosition = new Vector2(-sprite.size.x, -sprite.size.y) / 2;
        bottomRight.transform.localPosition = new Vector2(sprite.size.x, -sprite.size.y) / 2;
        topLeft.SetActive(false);
        topRight.SetActive(false);
        bottomLeft.SetActive(false);
        bottomRight.SetActive(false);
    }
}
