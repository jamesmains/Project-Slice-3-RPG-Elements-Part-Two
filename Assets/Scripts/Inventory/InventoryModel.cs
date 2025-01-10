using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryModel
{
    public ObservableArray<Item> InventoryItems { get; set; }
    public List<SlotOptions> SlotRestrictions = new();
    
    public event Action<Item[]> OnModelChanged {
        add => InventoryItems.AnyValueChanged += value;
        remove => InventoryItems.AnyValueChanged -= value;
    }

    public InventoryModel(IEnumerable<ItemDetails> itemDetails, int capacity, List<SlotOptions> typeRestrictions = null) {
        InventoryItems = new ObservableArray<Item>(capacity);
        SlotRestrictions = typeRestrictions ?? new List<SlotOptions>();
        
        // Todo: Logic error -- While this works for collapsing stacks, it in turn prevents detecting over stacked items
        Dictionary<ItemDetails, int> AllItems = new();
        foreach (var itemDetail in itemDetails) {
            if (!AllItems.TryAdd(itemDetail, 1)) {
                AllItems[itemDetail]++;
            }
        }

        foreach (var item in AllItems) {
            InventoryItems.TryAdd(item.Key.Create(item.Value));
        }
    }
    
    public Item Get(int index) => InventoryItems[index];
    public void Clear() => InventoryItems.Clear();
    public bool Add(Item item) => InventoryItems.TryAdd(item);
    public Item AddTo(Item item, int index) => InventoryItems.TryAdd(item, index);
    public Item Remove(Item item) => InventoryItems.TryRemove(item);
    
    public void Swap(int source, int target) => InventoryItems.Swap(source, target);

    public void Swap(int source, int target, InventoryModel targetModel) {
        var targetItem = targetModel.InventoryItems[target];
        var sourceItem = InventoryItems[source];
        targetModel.AddTo(sourceItem, target);
        AddTo(targetItem, source);
    }

    // Todo: Fix this so that if it exceeds max quantity, it spits back out the remainder
    public int Combine(int source, int target) {
        var total = InventoryItems[source].Quantity + InventoryItems[target].Quantity;
        InventoryItems[target].Quantity = total;
        Remove(InventoryItems[source]);
        return total;
    }
    
    public int Combine(int source, int target, InventoryModel targetModel) {
        var total = InventoryItems[source].Quantity + targetModel.InventoryItems[target].Quantity;
        targetModel.InventoryItems[target].Quantity = total;
        Remove(InventoryItems[source]);
        return total;
    }
}
