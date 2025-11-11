using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
public enum MoveType
{
    Slide,
    ZoomIn
}
public class CameraMove : MonoBehaviour
{
    Vector3 startPos;
    [SerializeField] MoveType moveType;

    [Header("General")]
    [SerializeField] float effectTime;
    [SerializeField] Vector3 slideDistance = new(20, 0, 0);
    [SerializeField] AnimationCurve tChangeCurve;


    void OnEnable()
    {
        PlayerEffects.FinishedTargetEffect += PlayCameraEffect;
    }
    void OnDisable()
    {
        PlayerEffects.FinishedTargetEffect -= PlayCameraEffect;
    }

    void Start()
    {
        startPos = transform.position;
    }

    private void PlayCameraEffect()
    {
        switch (moveType)
        {
            case MoveType.Slide:
                StartCoroutine(Slide());
                break;
            case MoveType.ZoomIn:
                StartCoroutine(ZoomIn());
                break;
        }
    }
    
    private IEnumerator Slide()
    {
        float t = 0f;
        Vector3 finalPos = startPos + slideDistance;
        while (t < 1)
        {
            transform.position = Vector3.Lerp(startPos, finalPos, tChangeCurve.Evaluate(t));
            t += Time.deltaTime / effectTime;
            yield return null;
        }
        transform.position = finalPos;
        Helper.LoadNextSlide();
    }

    private IEnumerator ZoomIn()
    {
        float t = 0f;
        Vector3 finalPos = startPos + slideDistance;
        while (t < 1)
        {
            transform.position = Vector3.Lerp(startPos, finalPos, tChangeCurve.Evaluate(t));
            t += Time.deltaTime / effectTime;
            yield return null;
        }
        transform.position = finalPos;
        Helper.LoadNextSlide();
    }
}
