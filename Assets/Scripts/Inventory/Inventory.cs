using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
    [SerializeField] StorageView InventoryView;
    [SerializeField] int Capacity = 20;
    [SerializeField] List<ItemDetails> StartingItems = new();
    [SerializeField] List<SlotOptions> SlotOptions = new();
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    private InventoryController Controller;
    public static Image GhostIcon;

    private void Start() {
        foreach (Transform child in this.transform) {
            Destroy(child.gameObject);
        }
        Controller = new InventoryController.Builder(InventoryView)
            .WithStartingItems(StartingItems)
            .WithCapacity(Capacity)
            .WithRestrictions(SlotOptions)
            .Build();
    }
}
