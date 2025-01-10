using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class SlotOptions {
    public Sprite FrameBackground;
    public Sprite RestrictionIcon;
    public ItemType ItemTypeRestriction;
    public const float RestrictionIconAlpha = 0.15f;
}

public class Slot : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler, IPointerExitHandler {
    [SerializeField, FoldoutGroup("Dependencies")]
    public RectTransform Rect;

    [SerializeField, FoldoutGroup("Dependencies")]
    private CanvasGroup CanvasGroup;

    [SerializeField, FoldoutGroup("Dependencies")]
    private Image Background;
    
    [SerializeField, FoldoutGroup("Dependencies")]
    private Image RestrictionIcon;

    [SerializeField, FoldoutGroup("Dependencies")]
    private Image StoredItemIcon;

    [SerializeField, FoldoutGroup("Dependencies")]
    private TextMeshProUGUI StackLabel;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    public ItemType ItemTypeRestriction = ItemType.None;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    public Sprite BaseSprite;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    public StorageView ParentView;

    public int Index => transform.GetSiblingIndex();
    public SerializableGuid ItemId { get; private set; } = SerializableGuid.Empty;

    public event Action<Vector2, Slot> OnStartDrag = delegate { };

    public void SetSlotOptions(SlotOptions options) {
        if (options == null) return;
        Background.sprite = options.FrameBackground ? options.FrameBackground : null;
        RestrictionIcon.sprite = options.RestrictionIcon ? options.RestrictionIcon : null;
        RestrictionIcon.enabled = RestrictionIcon.sprite != null;
        RestrictionIcon.color = new Color(1,1,1,SlotOptions.RestrictionIconAlpha);
        ItemTypeRestriction = options.ItemTypeRestriction;
    }

    public void Set(SerializableGuid id, Sprite icon, StorageView parentWindow, int quantity = 0) {
        ItemId = id;
        BaseSprite = icon;
        ParentView = parentWindow;
        if (BaseSprite == null)
            Hide();
        else Show();
        StoredItemIcon.sprite = BaseSprite != null ? icon : null;
        StackLabel.gameObject.SetActive(quantity > 1);
        StackLabel.text = quantity > 1 ? quantity.ToString() : string.Empty;
    }

    public void Hide() {
        if (CanvasGroup != null)
            CanvasGroup.alpha = 0;
        RestrictionIcon.gameObject.SetActive(true);
    }

    public void Show() {
        if (CanvasGroup != null)
            CanvasGroup.alpha = 1;
        RestrictionIcon.gameObject.SetActive(false);
    }

    public void Remove() {
        ItemId = SerializableGuid.Empty;
        StoredItemIcon.sprite = null;
        Hide();
    }

    public void OnPointerDown(PointerEventData evt) {
        if (evt.button == PointerEventData.InputButton.Right && !ItemId.Equals(SerializableGuid.Empty))
            if (ParentView.Model.InventoryItems[Index].Details.ItemBehavior is ItemBehavior) {
                
            }
        if (evt.button != 0 || ItemId.Equals(SerializableGuid.Empty)) return;
        OnStartDrag.Invoke(evt.position, this);
    }

    public void OnPointerMove(PointerEventData eventData) {
        if (ItemId == SerializableGuid.Empty || ParentView.isDragging) {
            Tooltip.OnHideTooltip.Invoke();
            return;
        }
        Tooltip.OnShowTooltip.Invoke(ParentView.Model.InventoryItems[Index].GetTooltipText(ParentView.AssociatedPlayer.PlayerEntity.EntityEquipment.GetEquippedItems()));
    }

    public void OnPointerExit(PointerEventData eventData) {
        Tooltip.OnHideTooltip.Invoke();
    }
}