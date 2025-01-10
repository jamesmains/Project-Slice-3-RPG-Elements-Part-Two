using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class Stat {
    /// <summary>
    /// Currently, stats are very simplistic, using Lvl as the out value when modifying
    /// </summary>
    /// 
    [SerializeField, BoxGroup("Settings")]
    public StatDetails Details;
    
    [SerializeField, BoxGroup("Settings")]
    public int Level = 1;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    public float CurrentExp;
    
    public float ExpToNextLevel() => Level * 100;
    public const float TrainValueMultiplier = 2f;
    
    // Todo: Find usage for bool out
    // Bool out for detecting if should show event on level up
    public bool TrainStat(float amount) {
        CurrentExp += amount;
        if (!(CurrentExp >= ExpToNextLevel())) return false;

        CurrentExp = Mathf.Abs(ExpToNextLevel() - CurrentExp);
        Level++;
        return true;
    }

    public string GetStatName() {
        var text = string.Empty;
        text += $"<color=yellow>Lvl {Level}</color> {Details.SkillName}";
        return text;
    }

    public string GetStatComparison(Stat matchingStat = null) {
        var text = string.Empty;
        var valueToCompare = matchingStat?.Level ?? Level * 2;
        string statComparison = Level > valueToCompare
            ? $"<color=green>(+{Level - valueToCompare})</color>"
            : Level < valueToCompare
                ? $"<color=red>(-{valueToCompare - Level})</color>"
                : $"";
        text += $"{statComparison}";
        return text;
    }

    public string GetStatFooter() {
        var text = string.Empty;
        text += "\n";
        text += $"<size=85%>Exp {CurrentExp} | {ExpToNextLevel()}\n</size>";
        return text;
    }
    
    public static List<Stat> CombineStats(List<Stat> list1, List<Stat> list2)
    {
        var result = list1.ToDictionary(stat => stat.Details, stat => new {stat.Level});
        
        foreach (var stat in list2)
        {
            if (result.ContainsKey(stat.Details))
            {
                var current = result[stat.Details];
                result[stat.Details] = new
                {
                    Level = stat.Level + current.Level 
                };
            }
        }

        return result.Select(kvp => new Stat
        {
            Details = kvp.Key,
            Level = kvp.Value.Level
        }).ToList();
    }
}