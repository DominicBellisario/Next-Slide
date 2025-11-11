using UnityEngine;

public class CameraMove : MonoBehaviour
{
    protected Vector3 startPos;
    protected float startSize;
    protected Camera cam;

    [Header("General")]
    [SerializeField] protected float effectTime = 1;
    [SerializeField] protected AnimationCurve tChangeCurve;

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
        cam = GetComponent<Camera>();
        startPos = transform.position;
        startSize = cam.orthographicSize;
    }

    virtual protected void PlayCameraEffect(){}
}
