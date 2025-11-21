using System;
using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenu : MonoBehaviour
{
    public static event Action FinishedPauseAnimation;

    [SerializeField] float transitionTime;
    [SerializeField] AnimationCurve transitionCurve;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI levelNumbers;

    CanvasGroup cGroup;
    LevelVariables lVar;
    float startYPos;
    float endYPos;

    void OnEnable()
    {
        PlayerInputs.Paused += ActivatePauseMenu;
        PlayerInputs.Unpaused += DeactivatePauseMenu;
    }
    void OnDisable()
    {
        PlayerInputs.Paused -= ActivatePauseMenu;
        PlayerInputs.Unpaused -= DeactivatePauseMenu;
    }

    void Start()
    {
        cGroup = GetComponent<CanvasGroup>();
        lVar = FindAnyObjectByType<LevelVariables>();
        startYPos = transform.localPosition.y;
        endYPos = startYPos - 50;
        title.text = lVar.SlideName;
        levelNumbers.text = "L" + lVar.LevelNumber + " S" + lVar.SlideNumber;
    }

    private void ActivatePauseMenu()
    {
        StartCoroutine(ActivatePauseMenuCoroutine());
    }

    private IEnumerator ActivatePauseMenuCoroutine()
    {
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
        FinishedPauseAnimation?.Invoke();
    }

    private void DeactivatePauseMenu()
    {
        StartCoroutine(DeactivatePauseMenuCoroutine());
    }

    private IEnumerator DeactivatePauseMenuCoroutine()
    {
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
        FinishedPauseAnimation?.Invoke();
    }
}
