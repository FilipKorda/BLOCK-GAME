using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameButton : UIBehaviour, ISelectHandler, IPointerEnterHandler
{

    [SerializeField] protected UnityEvent onSelect;
    [SerializeField] protected UnityEvent onDeselect;
    [SerializeField] protected UnityEvent onDisable;

    public Action<GameButton> onSelectAction;

    protected Selectable selectable;


    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!selectable)
            selectable = GetComponent<Selectable>();

        if (selectable && selectable.interactable)
            selectable.Select();
    }

    public virtual void OnSelect(BaseEventData eventData)
    {
        onSelect?.Invoke();
        onSelectAction?.Invoke(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        onDisable?.Invoke();
    }
}