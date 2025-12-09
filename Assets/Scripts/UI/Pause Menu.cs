using System;
using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenu : MonoBehaviour
{
    public static event Action FinishedPauseAnimation;
    public static event Action Unpaused;

    [SerializeField] float transitionTime;
    [SerializeField] AnimationCurve transitionCurve;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI levelNumbers;

    CanvasGroup cGroup;
    LVar lVar;
    GVar gVar;

    float startYPos;
    float endYPos;
    bool isPaused;
    bool canPause = true;

    void OnEnable()
    {
        PlayerInputs.PauseButtonPressed += RunPauseLogic;
    }
    void OnDisable()
    {
        PlayerInputs.PauseButtonPressed -= RunPauseLogic;
    }

    void Start()
    {
        cGroup = GetComponent<CanvasGroup>();
        lVar = FindAnyObjectByType<LVar>();
        gVar = GVar.Instance;
        startYPos = transform.localPosition.y;
        endYPos = startYPos - 50;
        title.text = lVar.SlideName;
        levelNumbers.text = "L" + lVar.LevelNumber + " S" + lVar.SlideNumber + "/" + gVar.NumOfSlidesInLevel;
    }

    private void RunPauseLogic()
    {
        if (!canPause) return;
        if (!isPaused) { ActivatePauseMenu(); }
        else { DeactivatePauseMenu(); }
    }
    public void ActivatePauseMenu()
    {
        StartCoroutine(ActivatePauseMenuCoroutine());
    }

    private IEnumerator ActivatePauseMenuCoroutine()
    {
        canPause = false;
        float t = 0;
        while (t < 1)
        {
            float yPos = Mathf.Lerp(startYPos, endYPos, transitionCurve.Evaluate(t));
            transform.localPosition = new Vector3(0, yPos, 0);

            t += Time.deltaTime / transitionTime;
            yield return null;
        }
        cGroup.interactable = true;
        cGroup.blocksRaycasts = true;

        isPaused = true;
        canPause = true;

        FinishedPauseAnimation?.Invoke();
    }

    public void DeactivatePauseMenu()
    {
        StartCoroutine(DeactivatePauseMenuCoroutine());
    }

    private IEnumerator DeactivatePauseMenuCoroutine()
    {
        Unpaused?.Invoke();
        canPause = false;
        cGroup.interactable = false;
        cGroup.blocksRaycasts = false;
        float t = 0;
        while (t < 1)
        {
            float yPos = Mathf.Lerp(endYPos, startYPos, transitionCurve.Evaluate(t));
            transform.localPosition = new Vector3(0, yPos, 0);

            t += Time.deltaTime / transitionTime;
            yield return null;
        }
        isPaused = false;
        canPause = true;
        FinishedPauseAnimation?.Invoke();
    }
}
