using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMove : MonoBehaviour
{
    protected Vector3 startPos;
    protected float startSize;
    protected Camera cam;

    [Header("General")]
    [SerializeField] protected GameObject borderPrefab;
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

    private void Start()
    {
        cam = GetComponent<Camera>();
        startPos = transform.position;
        startSize = cam.orthographicSize;
    }

    virtual protected void PlayCameraEffect(){}
}
