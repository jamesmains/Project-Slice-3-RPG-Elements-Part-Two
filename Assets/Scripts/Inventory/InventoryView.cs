using System;
using System.Collections;
using ParentHouse.Utils;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryView : StorageView {
    [SerializeField] private string PanelName = "Inventory";
    public Action<InventoryModel> OnSetInventoryModel;
    public override IEnumerator InitializeView(InventoryModel model, int size = 20) {
        Slots = new Slot[size];
        Model = model;
        OnSetInventoryModel?.Invoke(Model);
        for (int i = 0; i < size; i++) {
            var slot = Pooler.Spawn(SlotPrefab,SlotContainer).GetComponent<Slot>();
            Slots[i] = slot;
        }
        yield return null;
    }

    public override IEnumerator RegisterCallbacks() {
        for (var i = 0; i < Slots.Length; i++) {
            var slot = Slots[i];
            slot.OnStartDrag += OnStartDrag;
            if(i < Model.SlotRestrictions.Count)
                slot.SetSlotOptions(Model.SlotRestrictions[i]);
        }
        
        AssociatedPlayerInput.InputSystem.UI.Point.performed += HandlePointerMove;
        AssociatedPlayerInput.InputSystem.UI.Click.performed += HandlePointerUp;
        yield return null;
    }
}