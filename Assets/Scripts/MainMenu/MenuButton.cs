using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class MenuButton : GameButton, IPointerDownHandler, IDeselectHandler
{
    [SerializeField] private Button basicButton;
    [SerializeField] public bool interactiveOnEnable = true;
    [SerializeField] public bool interactiveOnDisable = true;


    [SerializeField] private RectTransform selector;
    [SerializeField] private RectTransform arrow;

    [SerializeField] private TextMeshProUGUI textMesh;

    [Header("Selection animation parameters")]
    [SerializeField] private float startXPosition;

    [SerializeField] private float endXPosition;
    [SerializeField] private float slideDuration;

    [Header("Text parameters")]
    [SerializeField] protected FontWeight basicWeight;
    [SerializeField] protected FontWeight onSelectWeight;

    [SerializeField] protected int basicFontSize;
    [SerializeField] protected int onSelectFontSize;

    [Header("Material parameters")]
    [SerializeField] protected Material onSelectTextMaterial;
    [SerializeField] protected Material textBaseMaterial;

    [SerializeField] private UnityEvent onInteractable;
    private bool isSelected;

    [SerializeField] private bool useItalic = true;


    public bool Interactable
    {
        get => basicButton != null && basicButton.interactable;
        set
        {
            if (!basicButton) return;
            basicButton.interactable = value;
            if (value) onInteractable?.Invoke();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (basicButton) Interactable = interactiveOnEnable;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!selectable)
            selectable = GetComponent<Selectable>();

        if (selectable && selectable.interactable)
        {
            selectable.Select();
            SelectAnimation(true);
        }
    }

    private void MoveSelector(bool active)
        => selector.DOAnchorPosX(active ? endXPosition : startXPosition, slideDuration);

    private void EffectOnSelect(bool selected)
    {
        if (selector) selector.gameObject.SetActive(selected);
        if (selector) MoveSelector(selected);
        if (arrow) arrow.gameObject.SetActive(selected);
    }

    void FormatText(bool selected)
    {
        if (selected)
        {
            textMesh.fontWeight = onSelectWeight;
            textMesh.fontSizeMax = onSelectFontSize;

            if (onSelectTextMaterial) textMesh.fontSharedMaterial = onSelectTextMaterial;
        }
        else
        {
            textMesh.fontWeight = basicWeight;
            textMesh.fontSizeMax = basicFontSize;
            textMesh.fontStyle = FontStyles.UpperCase;
            if (textBaseMaterial) textMesh.fontSharedMaterial = textBaseMaterial;
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        SelectAnimation(true);
        base.OnSelect(eventData);
    }

    public void Select()
    {
        SelectNoEvent();
        //isSelected = true;
        onSelect?.Invoke();
        onSelectAction?.Invoke(this);
    }

    public void SelectNoEvent()
    {
        if (!TryGetComponent(out Selectable _)) return;
        EventSystem.current.SetSelectedGameObject(gameObject);
        SelectAnimation(true);
    }

    public void SelectAnimation(bool select)
    {
        EffectOnSelect(select);
        FormatText(select);

        isSelected = select;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SelectAnimation(false);
        onDeselect?.Invoke();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        SelectAnimation(false);
        if (basicButton) Interactable = interactiveOnDisable;
    }

    public void OnPointerDown(PointerEventData eventData)
    {


    }
}