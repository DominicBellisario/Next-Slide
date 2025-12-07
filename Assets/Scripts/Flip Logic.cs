using System.Collections;
using UnityEngine;

public class FlipLogic : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Vector2 sizeMultiplier;
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

    // Keeps the corners locked to scaled sprite size
    private void UpdateCorners()
    {
        Vector2 scaledSize = Vector2.Scale(sizeMultiplier, sprite.transform.localScale);

        topLeft.transform.localPosition     = new Vector2(-scaledSize.x,  scaledSize.y) * 0.5f;
        topRight.transform.localPosition    = new Vector2( scaledSize.x,  scaledSize.y) * 0.5f;
        bottomLeft.transform.localPosition  = new Vector2(-scaledSize.x, -scaledSize.y) * 0.5f;
        bottomRight.transform.localPosition = new Vector2( scaledSize.x, -scaledSize.y) * 0.5f;
    }

    private IEnumerator PlayFlipEffectsCoroutine(float flipTime, AnimationCurve flipCurve)
    {
        topLeft.SetActive(true);
        topRight.SetActive(true);
        bottomLeft.SetActive(true);
        bottomRight.SetActive(true);

        float t = 0f;

        // ---- Squish Phase ----
        while (t < 0.5f)
        {
            sprite.transform.localScale = new Vector3(
                Mathf.Lerp(1, 0, flipCurve.Evaluate(t)),
                1,
                1
            );

            UpdateCorners();

            t += Time.deltaTime / flipTime;
            yield return null;
        }

        sprite.flipX = !sprite.flipX;

        // ---- Expand Phase ----
        while (t < 1f)
        {
            sprite.transform.localScale = new Vector3(
                Mathf.Lerp(1, 0, flipCurve.Evaluate(t)),
                1,
                1
            );

            UpdateCorners();

            t += Time.deltaTime / flipTime;
            yield return null;
        }

        // ---- Cleanup ----
        sprite.transform.localScale = Vector3.one;

        UpdateCorners();

        topLeft.SetActive(false);
        topRight.SetActive(false);
        bottomLeft.SetActive(false);
        bottomRight.SetActive(false);
    }
}
