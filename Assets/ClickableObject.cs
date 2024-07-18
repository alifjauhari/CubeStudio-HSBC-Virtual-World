using UnityEngine;
using UnityEngine.Events;

public class ClickableObject : MonoBehaviour
{
    public UnityEvent OnClick;
    public UnityEvent OnMouseHoverEnter;
    public UnityEvent OnMouseHoverExit;

    void OnMouseDown()
    {
        OnClick?.Invoke();
    }

    void OnMouseEnter()
    {
        OnMouseHoverEnter?.Invoke();
    }

    void OnMouseExit()
    {
        OnMouseHoverExit?.Invoke();
    }
}
