using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public abstract class ItemBehavior {
}

public class WeaponBehavior : ItemBehavior, IStat, IViewableEquipment {

    [field: SerializeField, FoldoutGroup("Settings")]
    public GameObject EquipModel { get; set; }

    [field: SerializeField, FoldoutGroup("Settings")]
    public List<Mesh> EquipMeshList { get; set; } = new();

    [field: SerializeField, FoldoutGroup("Settings"), ListDrawerSettings(ListElementLabelName = "Details")]
    public List<Stat> WeaponStats { get; set; } = new();

    public List<Stat> GetStats() {
        return WeaponStats;
    }
}

public class ArmorBehavior : ItemBehavior, IStat, IViewableEquipment {
    [field: SerializeField, FoldoutGroup("Settings"), ListDrawerSettings(ListElementLabelName = "Details")]
    public List<Stat> ArmorStats { get; set; } = new();
    
    [field: SerializeField, FoldoutGroup("Settings")]
    public GameObject EquipModel { get; set; }

    [field: SerializeField, FoldoutGroup("Settings")]
    public List<Mesh> EquipMeshList { get; set; } = new();

    public List<Stat> GetStats() {
        return ArmorStats;
    }

}

public class ConsumableBehavior : ItemBehavior, IStat {
    public List<Stat> GetStats() {
        throw new NotImplementedException();
    }
}