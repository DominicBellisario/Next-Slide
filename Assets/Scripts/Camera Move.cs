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

    [Header("Slide")]
    [SerializeField] float slideTime;
    void OnEnable()
    {
        PlayerMovement.HitTarget += PlayEffect;
    }
    void OnDisable()
    {
        PlayerMovement.HitTarget -= PlayEffect;
    }

    void Start()
    {
        startPos = transform.position;
    }

    private void PlayEffect()
    {
        switch (moveType)
        {
            case MoveType.Slide:
                StartCoroutine(Slide());
                break;
            case MoveType.ZoomIn:
                break;
        }
    }
    
    private IEnumerator Slide()
    {
        float t = 0f;
        while (t < 1)
        {
            transform.position = Vector3.Lerp(startPos, startPos + new Vector3(20f, 0, 0), t);
            t += Time.deltaTime / slideTime;
            yield return null;
        }
        Debug.Log("finished slide");
        Helper.LoadNextSlide();
        
    }
}
