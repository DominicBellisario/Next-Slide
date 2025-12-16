using UnityEngine;
using UnityEngine.EventSystems;

public class HoverAnimator : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] Animator animator;

    public void OnPointerEnter(PointerEventData eventData) { animator.SetBool("isHovering", true); }

    public void OnPointerExit(PointerEventData eventData) { animator.SetBool("isHovering", false); }
}
