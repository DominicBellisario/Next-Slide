using UnityEditor.SearchService;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Dropdown : MonoBehaviour
{
    bool isActive;
    CanvasGroup cGroup;

    void OnEnable()
    {
        PauseMenu.Unpaused += DeactivateDropdown;
    }
    void OnDisable()
    {
        PauseMenu.Unpaused -= DeactivateDropdown;
    }

    void Start()
    {
        cGroup = GetComponent<CanvasGroup>();
    }

    public void ToggleDropdown()
    {
        if (!isActive) { ActivateDropdown(); }
        else { DeactivateDropdown(); }
    }

    private void ActivateDropdown()
    {
        cGroup.alpha = 1.0f;
        cGroup.interactable = true;
        cGroup.blocksRaycasts = true;
        isActive = true;
    }
    private void DeactivateDropdown()
    {
        cGroup.alpha = 0;
        cGroup.interactable = false;
        cGroup.blocksRaycasts = false;
        isActive = false;
    }
}
