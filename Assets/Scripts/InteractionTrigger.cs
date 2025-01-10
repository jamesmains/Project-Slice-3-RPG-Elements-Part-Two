using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public enum ActivationType {
    Once,
    Infinite,
    Toggle
}

public class InteractionTrigger : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Settings")]
    private ActivationType ActivationSetting = ActivationType.Once;

    [SerializeField] [FoldoutGroup("Settings")]
    private ActivationType DeactivationSetting = ActivationType.Once;

    [SerializeField] [FoldoutGroup("Settings")]
    public bool RequireKeyToActivate;

    [SerializeField] [FoldoutGroup("Settings")]
    public bool RequireKeyToDeactivate;

    [SerializeField] [FoldoutGroup("Settings")] [ShowIf("RequireKeyToActivate")]
    private bool ShowInteractPrompt;

    [SerializeField] [FoldoutGroup("Settings")]
    private bool ActiveOnEnable = false;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private bool Activated;

    [SerializeField] [FoldoutGroup("Events")]
    private UnityEvent OnActivate;

    [SerializeField] [FoldoutGroup("Events")]
    private UnityEvent OnDeactivate;

    private void OnEnable() {
        if (ActiveOnEnable)
            Activate();
        else Deactivate();
    }

    public void Notify() {
    }

    public void Activate() {
        if (ActivationSetting == ActivationType.Once && Activated) return;
        if (ActivationSetting == ActivationType.Toggle && Activated) Deactivate();
        else {
            OnActivate.Invoke();
            Activated = true;
        }
    }

    public void Deactivate() {
        if (DeactivationSetting == ActivationType.Once && !Activated) return;
        if (DeactivationSetting == ActivationType.Toggle && !Activated) Activate();
        else {
            OnDeactivate.Invoke();
            Activated = false;
        }
    }
}