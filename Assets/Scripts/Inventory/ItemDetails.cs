using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public enum ItemType {
    None,
    Head,
    Neck,
    Body,
    Hand,
    Waist,
    Legs,
    Feet,
    MainHand,
    OffHand,
    Accessory,
    Ring,
    General,
}

[CreateAssetMenu(fileName = "Item", menuName = "Item")]
public class ItemDetails : SerializedScriptableObject {
    public ItemDetails() {
        AssignNewGuid();
    }

    [SerializeField, BoxGroup("Settings")] 
    public string Name;

    [SerializeField, BoxGroup("Settings")] 
    public ItemType Type = ItemType.General;

    [SerializeField, BoxGroup("Settings")] 
    public int MaxStack = 1;

    [SerializeField, BoxGroup("Settings"),PreviewField]
    public Sprite Icon;

    [SerializeField, BoxGroup("Settings")] 
    public ItemBehavior ItemBehavior;
    
    public SerializableGuid Id = SerializableGuid.NewGuid();

    [Button]
    private void AssignNewGuid() {
        Id = SerializableGuid.NewGuid();
    }

    public Item Create(int quantity) {
        return new Item(this, quantity);
    }

    public string GetItemStatDetails(List<Item> equipment) {
        var text = string.Empty;
        var theseStats = GetAllStats();
        int debugInteractionIndex = 0;
        foreach (var stat in theseStats) {
            text += stat.GetStatName();
            foreach (var e in equipment) {
                var matchingStat = e.Details.GetAllStats()
                    .FirstOrDefault(o => o.Details == stat.Details);
                matchingStat = stat.Details == matchingStat?.Details
                    ? matchingStat
                    : null;
                if (matchingStat != null) {
                    text += stat.GetStatComparison(matchingStat);
                }
            }

            debugInteractionIndex++;
            text += stat.GetStatFooter();
        }

        return text;
    }

    // public List<Skill> GetAllSkills() {
    //     var skills = new List<Skill>();
    //     if (ItemBehavior is IStat ItemWithStats)
    //         foreach (var stat in ItemWithStats.GetStats()) {
    //             skills.Add(stat.Details.AssociatedSkill);
    //         }
    //
    //     return skills;
    // }

    public List<Stat> GetAllStats() {
        var stats = new List<Stat>();
        if (ItemBehavior is IStat ItemWithStats)
            stats.AddRange(ItemWithStats.GetStats());
        return stats;
    }
    //
    // public Stat GetMatchingStat(Stat comparisonStat) {
    //     return GetAllStats()
    //         .FirstOrDefault(stat => stat.Details.AssociatedSkill == comparisonStat.Details.AssociatedSkill);
    // }
}