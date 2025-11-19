using System.Collections;
using UnityEngine;

public class ObjectImpact : MonoBehaviour
{
    Vector3 startPos;
    [SerializeField] float shakeDuration;
    [SerializeField] float shakeMagnitude;

    void Start()
    {
        startPos = transform.position;
    }

    public void HitObject()
    {
        StartCoroutine(Shake());
    }

    public void DestroyObject()
    {
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
