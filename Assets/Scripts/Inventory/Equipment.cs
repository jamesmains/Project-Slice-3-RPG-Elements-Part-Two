using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

public class Equipment : MonoBehaviour {
    [SerializeField, FoldoutGroup("Dependencies")]
    private List<EntityEquipmentModelSlot> ModelSlots = new();

    [SerializeField, FoldoutGroup("Dependencies")]
    private List<EntityEquipmentSkinMesh> SkinMeshes = new();

    [SerializeField, FoldoutGroup("Settings")]
    private ItemDetails DefaultWeapon;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private List<Stat> EquipmentStats = new List<Stat>();

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private List<Item> EquippedItems = new();

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    public WeaponBehavior CurrentWeaponBehavior;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    public Weapon CurrentWeapon;

    public Action<List<Stat>> OnEquipmentChanged;

    public void SetEquippedItems(List<Item> equippedItems) {
        EquippedItems = equippedItems;
        var i = EquippedItems.FirstOrDefault(o => o.Details.ItemBehavior is WeaponBehavior);
        if (i != null) {
            CurrentWeaponBehavior = i.Details.ItemBehavior as WeaponBehavior;
        }
        else if (DefaultWeapon != null) {
            CurrentWeaponBehavior = DefaultWeapon.ItemBehavior as WeaponBehavior;
            EquippedItems.Add(new Item(DefaultWeapon, 1));
        }
        else CurrentWeaponBehavior = null;

        OnEquipmentChanged.Invoke(GetEquipmentStats());
        UpdateModels();
        UpdateSkins();
    }

    private void UpdateModels() {
        foreach (var modelSlot in ModelSlots) {
            modelSlot.TrySet(EquippedItems.FirstOrDefault(o => o.Details.Type == modelSlot.ItemType));
        }

        var wep = ModelSlots.FirstOrDefault(o => o.ItemType == ItemType.MainHand);
        // Todo - Do ANY of the todos.. also fix this garbage
        if (wep.OccupyingSlotGameObject != null)
            CurrentWeapon = wep?.OccupyingSlotGameObject?.GetComponent<Weapon>();

        foreach (var skinMesh in SkinMeshes) {
            skinMesh.TrySet(EquippedItems.FirstOrDefault(o => o.Details.Type == skinMesh.ItemType));
        }
    }

    private void UpdateSkins() {
    }

    public Item GetEquippedItem(ItemType itemType) {
        return EquippedItems.FirstOrDefault(o => o.Details.Type == itemType);
    }

    public List<Item> GetEquippedItems() {
        return EquippedItems;
    }

    public List<Stat> GetEquipmentStats() {
        EquipmentStats = new List<Stat>();
        Dictionary<StatDetails, float> stats = new();
        foreach (var itemStat in EquippedItems.SelectMany(item => item.Details.GetAllStats())) {
            if (stats.ContainsKey(itemStat.Details)) {
                stats[itemStat.Details] += itemStat.Level;
            }
            else {
                stats.Add(itemStat.Details, itemStat.Level);
            }
        }

        foreach (var stat in stats) {
            var s = new Stat();
            s.Details = stat.Key;
            s.Level = (int)stat.Value;
            EquipmentStats.Add(s);
        }

        return EquipmentStats;
    }
}

[Serializable]
public class EntityEquipmentModelSlot {
    public ItemType ItemType;
    public Transform SlotTransform;
    [ReadOnly] public GameObject OccupyingSlotGameObject;

    public void Release() {
        if (OccupyingSlotGameObject != null) {
            Object.Destroy(OccupyingSlotGameObject);
        }
    }

    public void TrySet(Item item) {
        Release();
        if (item?.Details.ItemBehavior is not IViewableEquipment viewableEquipment) return;
        if (viewableEquipment.EquipModel != null)
            OccupyingSlotGameObject = Object.Instantiate(viewableEquipment.EquipModel, SlotTransform);
    }
}

[Serializable]
public class EntityEquipmentSkinMesh {
    public ItemType ItemType;
    public List<Mesh> Defaults;
    public List<SkinnedMeshRenderer> SkinnedMeshRenderer;
    private int indexer;

    public void Release() {
        if (indexer >= SkinnedMeshRenderer.Count) return;
        SkinnedMeshRenderer[indexer].sharedMesh = Defaults[indexer];
    }

    public void TrySet(Item item) {
        indexer = 0;
        var viewableEquipment = item?.Details.ItemBehavior as IViewableEquipment;
        foreach (var renderer in SkinnedMeshRenderer) {
            Release();
            if (viewableEquipment == null || indexer >= viewableEquipment.EquipMeshList.Count) {
                indexer++;
                continue;
            }

            SkinnedMeshRenderer[indexer].sharedMesh = viewableEquipment.EquipMeshList[indexer];
            indexer++;
        }
    }
}