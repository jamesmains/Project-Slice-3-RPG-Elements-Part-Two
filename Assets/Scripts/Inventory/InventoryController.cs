using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController {
    private readonly StorageView InventoryView;
    public readonly InventoryModel Model;
    private readonly int InventoryCapacity;
    private static Action ForceRefresh;

    private InventoryController(StorageView inventoryView, InventoryModel model, int inventoryCapacity) {
        Debug.Assert(inventoryView != null, "Inventory View is null!");
        Debug.Assert(model != null, "Model is null!");
        Debug.Assert(inventoryCapacity > 0, "Capacity is less than 1!");
        InventoryView = inventoryView;
        Model = model;
        InventoryCapacity = inventoryCapacity;
        InventoryView.StartCoroutine(Initialize());
    }

    IEnumerator Initialize() {
        yield return InventoryView.InitializeView(Model,InventoryCapacity);
        yield return InventoryView.RegisterCallbacks();
        InventoryView.OnDrop += HandleDrop;
        Model.OnModelChanged += HandleModelChanged;
        ForceRefresh += RefreshView;
        Model.InventoryItems.Invoke();
        RefreshView();
    }

    private void HandleDrop(Slot originalSlot, Slot closestSlot) {
        // If item is same as target
        // -> check max stack
        // -> put in as much as you can
        // -> and return any remaining
        // If item is different, swap
        // If item is empty, swap
        Debug.Log(closestSlot);
        var sourceView = originalSlot.ParentView;
        var targetView = closestSlot.ParentView;
        
        var sourceItemId = originalSlot.ItemId.Equals(SerializableGuid.Empty) ? SerializableGuid.Empty : sourceView.Model.InventoryItems[originalSlot.Index].Details.Id;
        var targetItemId = closestSlot.ItemId.Equals(SerializableGuid.Empty) ? SerializableGuid.Empty : targetView.Model.InventoryItems[closestSlot.Index].Details.Id;
        
        // check for restrictions on slot
        if (closestSlot.ItemTypeRestriction != ItemType.None) {
            if (sourceView.Model.InventoryItems[originalSlot.Index].Details.Type != closestSlot.ItemTypeRestriction) {
                RefreshView();
                return;
            }
        }
        
        if(sourceView == targetView) {
            // self or target is empty
            if (originalSlot.Index == closestSlot.Index || targetItemId.Equals(SerializableGuid.Empty)) {
                Model.Swap(originalSlot.Index, closestSlot.Index);
                return;
            }
            // target is same item, add to stack
            if (sourceItemId.Equals(targetItemId) && Model.InventoryItems[closestSlot.Index].Details.MaxStack > 1) {
                Model.Combine(originalSlot.Index, closestSlot.Index);
            }
            else {
                Model.Swap(originalSlot.Index, closestSlot.Index);
            }
        }
        else {
            // target is empty
            if (targetItemId.Equals(SerializableGuid.Empty)) {
                Model.Swap(originalSlot.Index, closestSlot.Index, targetView.Model);
                // RefreshView();
                return;
            }
            // target is same item, add to stack
            if (sourceItemId.Equals(targetItemId) && Model.InventoryItems[closestSlot.Index].Details.MaxStack > 1) {
                Model.Combine(originalSlot.Index, closestSlot.Index, targetView.Model);
                ForceRefresh.Invoke();
            }
            else {
                Model.Swap(originalSlot.Index, closestSlot.Index, targetView.Model);
            }
        }
    }

    private void HandleModelChanged(IList<Item> items) => RefreshView();

    private void RefreshView() {
        for (int i = 0; i < InventoryCapacity; i++) {
            var item = Model.Get(i);
            if (item == null) {
                InventoryView.Slots[i].Set(SerializableGuid.Empty, null, InventoryView);
            }
            else {
                InventoryView.Slots[i].Set(item.Id, item.Details.Icon, InventoryView, item.Quantity);
            }
        }
    }

    public class Builder {
        private StorageView InventoryView;
        private IEnumerable<ItemDetails> InventoryItemDetails;
        private List<SlotOptions> SlotItemTypeRestrictions = new();
        private int Capacity;

        public Builder(StorageView inventoryView) {
            InventoryView = inventoryView;
        }

        public Builder WithStartingItems(IEnumerable<ItemDetails> itemDetails) {
            InventoryItemDetails = itemDetails;
            return this;
        }

        public Builder WithRestrictions(List<SlotOptions> itemTypeRestrictions) {
            SlotItemTypeRestrictions = itemTypeRestrictions;
            return this;
        }

        public Builder WithCapacity(int inventory) {
            Capacity = inventory;
            return this;
        }

        // Todo: Fix this. This ternary expression sucks.
        public InventoryController Build() {
            var InventoryDetails = InventoryItemDetails ?? Array.Empty<ItemDetails>();
            InventoryModel model = new InventoryModel(InventoryDetails, Capacity, SlotItemTypeRestrictions);
            return new InventoryController(InventoryView, model, Capacity);
        }
    }
}