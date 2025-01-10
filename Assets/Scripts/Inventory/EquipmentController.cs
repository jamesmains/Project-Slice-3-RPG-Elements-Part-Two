using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

// Bit sloppy to have this on the UI...
public class EquipmentController : MonoBehaviour {
    [SerializeField, FoldoutGroup("Dependencies")]
    private RectTransform StatBreakdownContainer;

    [SerializeField, FoldoutGroup("Dependencies")]
    private TextMeshProUGUI DebugStatBreakdownText;

    [SerializeField, FoldoutGroup("Dependencies")]
    private Player AssociatedPlayer;
    
    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private InventoryView InventoryView;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private Item[] CachedItems;

    private InventoryModel InventoryModel;

    private void Awake() {
        InventoryView = GetComponent<InventoryView>();
        InventoryView.OnSetInventoryModel += SetModel;
    }

    
    private void OnEnable() {
        if(AssociatedPlayer != null) {
            AssociatedPlayer.OnPlayerStatsChanged += SetBreakdownText;
            Debug.Log("Set player");
        }
    }

    private void OnDisable() {
        InventoryView.OnSetInventoryModel -= SetModel;
        if (AssociatedPlayer != null) {
            AssociatedPlayer.OnPlayerStatsChanged -= SetBreakdownText;
        }
    }

    private void SetModel(InventoryModel model) {
        InventoryModel = model;
        InventoryModel.OnModelChanged += SetPlayerEquipment;
    }

    private void SetPlayerEquipment(Item[] items) {
        Debug.Log("Setting equipment");
        List<Item> itemsToEquip = new();
        foreach (var item in InventoryModel.InventoryItems.Get()) {
            if (item != null && item.Details != null) {
                itemsToEquip.Add(item);
            }
        }

        UpdateBreakdown(itemsToEquip);
        AssociatedPlayer?.OnItemAdded.Invoke(itemsToEquip);
    }

    private void UpdateBreakdown(List<Item> items) {
        CachedItems = items.ToArray();
        SetBreakdownText();
    }

    private void SetBreakdownText() {
        Debug.Log(AssociatedPlayer.PlayerEntity);
        var text = string.Empty;
        var playerStats = AssociatedPlayer.PlayerEntity.GetBaseStats();
        var baseStats = new Dictionary<StatDetails, float>();
        var bonusStats = new Dictionary<StatDetails, float>();
        foreach (var playerStat in playerStats) {
            if (baseStats.ContainsKey(playerStat.Details))
                baseStats[playerStat.Details] += playerStat.Level;
            else baseStats.Add(playerStat.Details, playerStat.Level);
        }

        foreach (var item in CachedItems) {
            foreach (var itemStat in item.Details.GetAllStats()) {
                if (baseStats.ContainsKey(itemStat.Details))
                    baseStats[itemStat.Details] += itemStat.Level;
                if (bonusStats.ContainsKey(itemStat.Details))
                    bonusStats[itemStat.Details] += itemStat.Level;
                else bonusStats.Add(itemStat.Details, itemStat.Level);
            }
        }

        var extraStats = new Dictionary<StatDetails, float>();
        foreach (var baseStat in baseStats) {
            var baseText = $"<align=left>{baseStat.Key.SkillName}<line-height=0>\n";
            foreach (var bonusStat in bonusStats) {
                if (bonusStat.Key == baseStat.Key)
                    baseText += $" (+{bonusStat.Value}) ";
                else {
                    if (!extraStats.ContainsKey(bonusStat.Key) && !baseStats.ContainsKey(bonusStat.Key))
                        extraStats.Add(bonusStat.Key, bonusStat.Value);
                }
            }
            baseText += $"<align=right>{baseStat.Value}<line-height=1em>";
            text += $"{baseText}\n";
        }

        foreach (var extraStat in extraStats) {
            var baseText = $"<align=left>{extraStat.Key.SkillName}<line-height=0>\n<align=right>(+{extraStat.Value})<line-height=1em>";
            text += $"{baseText}\n";
        }

        DebugStatBreakdownText.text = text;
    }
}