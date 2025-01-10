using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public abstract class StorageView : MonoBehaviour {
    [SerializeField, FoldoutGroup("Dependencies")]
    public PlayerInput AssociatedPlayerInput;
    
    [SerializeField, FoldoutGroup("Dependencies")]
    public Player AssociatedPlayer;
    
    [SerializeField, FoldoutGroup("Dependencies")]
    protected RectTransform SlotContainer;

    [SerializeField, FoldoutGroup("Dependencies")]
    protected RectTransform Rect;

    [SerializeField, FoldoutGroup("Dependencies")]
    protected GameObject SlotPrefab;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    public InventoryModel Model;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    public Slot[] Slots;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    public bool isDragging;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private Slot originalSlot;
    
    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    public StorageView ClosestView;

    public static readonly List<StorageView> AllInventoryWindows = new();
    public event Action<Slot, Slot> OnDrop;

    private void OnEnable() {
        AllInventoryWindows.Add(this);
    }

    private void OnDisable() {
        AllInventoryWindows.Remove(this);
    }

    public abstract IEnumerator InitializeView(InventoryModel model, int size = 20);

    public abstract IEnumerator RegisterCallbacks();

    protected void OnStartDrag(Vector2 position, Slot slot) {
        isDragging = true;
        originalSlot = slot;

        SetGhostIconPosition(position);
        originalSlot.Hide();
        GhostIcon.Singleton.SetVisibility(true, originalSlot.BaseSprite);
    }

    private void SetGhostIconPosition(Vector2 position) {
        GhostIcon.Singleton.SetPosition(position);
    }

    private void GetClosestView() {
        ClosestView = AllInventoryWindows
            .Where(view => GhostIcon.Singleton.Rect.Overlaps(view.Rect))
            .OrderBy(view => Vector2.Distance(view.Rect.position, GhostIcon.Singleton.Rect.position))
            .FirstOrDefault();
    }

    protected void HandlePointerUp(InputAction.CallbackContext obj) {
        if (!isDragging) return;
        if (ClosestView != null) {
            Slot closestSlot = ClosestView.Slots
                .Where(slot => slot.Rect.Overlaps(GhostIcon.Singleton.Rect))
                .OrderBy(slot => Vector2.Distance(slot.Rect.position, GhostIcon.Singleton.Rect.position))
                .FirstOrDefault();
            if (closestSlot != null) {
                OnDrop?.Invoke(originalSlot, closestSlot);
            }
            else {
                originalSlot.Show();
            }
        }
        else {
            originalSlot.Show();
        }

        isDragging = false;
        originalSlot = null;
        GhostIcon.Singleton.SetVisibility(false);
    }

    protected void HandlePointerMove(InputAction.CallbackContext obj) {
        if (!isDragging) return;
        SetGhostIconPosition(obj.ReadValue<Vector2>());
        GetClosestView();
    }
}