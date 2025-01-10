using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace ParentHouse.Utils {
    public class ConditionalCheck : SerializedMonoBehaviour {
        [SerializeField] [BoxGroup("Settings")]
        private bool ContinuousCheck;

        [SerializeField] [BoxGroup("Settings")]
        private float CheckRate = 0.25f;
        
        [SerializeField] [BoxGroup("Settings")]
        private List<Condition> Conditions = new();

        private float timer;

        [FoldoutGroup("Events")] [SerializeField]
        private UnityEvent OnMeetsRequirements;

        [FoldoutGroup("Events")] [SerializeField]
        private UnityEvent OnFailsRequirements;
        
        private bool cachedState;

        private void Awake() {
            CheckStatus(true);
        }

        private void Update() {
            if (!ContinuousCheck) return;
            if (timer <= 0) {
                CheckStatus();
                timer = CheckRate;
            }
            else {
                timer -= Time.deltaTime;
            }
        }

        private void CheckStatus(bool resetCache = false) {
            bool canDo = IsConditionMet();
            if (resetCache) {
                cachedState = !canDo;
            }

            if (canDo && (!cachedState)) {
                OnMeetsRequirements.Invoke();
            }
            else if (!canDo && (cachedState)) {
                OnFailsRequirements.Invoke();
            }

            cachedState = canDo;
        }

        private bool IsConditionMet() {
            return Conditions.TrueForAll(c => c.IsConditionMet());
        }
    }
}