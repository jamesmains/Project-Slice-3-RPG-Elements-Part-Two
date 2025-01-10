using System;
using System.Collections.Generic;
using UnityEngine;

namespace ParentHouse.Utils {
    [Serializable]

    public abstract class Condition {
        public abstract bool IsConditionMet();
        public abstract void Use();
    }

    public class AndCondition : Condition {
        [field: SerializeField] public List<Condition> Conditions { get; private set; } = new();

        public override bool IsConditionMet() {
            return Conditions.TrueForAll(c => c.IsConditionMet());
        }

        public override void Use() {
            Conditions.ForEach(c => c.Use());
        }
    }

    public class OrCondition : Condition {
        [field: SerializeField] public List<Condition> Conditions { get; private set; } = new();

        public override bool IsConditionMet() {
            return Conditions.Exists(c => c.IsConditionMet());
        }

        public override void Use() {
            Conditions.ForEach(c => c.Use());
        }
    }

    public class NotCondition : Condition {
        [field: SerializeField] public List<Condition> Conditions { get; private set; } = new();

        public override bool IsConditionMet() {
            return !Conditions.Exists(c => c.IsConditionMet());
        }

        public override void Use() {
            Conditions.ForEach(c => c.Use());
        }
    }
}




