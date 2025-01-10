using System;
using Sirenix.OdinInspector;
using UnityEngine;

public enum ActionAnimationType {
    Unarmed_Melee_Attack_Punch_A,
    One_Hand_Melee_Attack_Chop,
    One_Hand_Melee_Attack_Stab,
    Two_Hand_Melee_Attack_Chop,
    Two_Hand_Melee_Attack_Stab,
}

public class Weapon : MonoBehaviour {
    [SerializeField, FoldoutGroup("Settings")]
    private ActionAnimationType PrimaryAttackTrigger;

    public string PrimaryAttack() {
        return PrimaryAttackTrigger.ToString();
    }
}

[Serializable]
public abstract class WeaponAction {
}