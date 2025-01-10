using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionActivator : MonoBehaviour
{
    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private readonly Dictionary<Collider2D,InteractionTrigger> NearbyTriggers = new ();

    private void Start() {
        // PlayerInput.Singleton.InputSystem.Player.Interact.performed += TryInteract;
    }

    private void TryInteract(InputAction.CallbackContext obj) {
        print("Trying to interact");
        foreach (var trigger in NearbyTriggers) {
            trigger.Value.Activate();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent(out InteractionTrigger pTrigger)) {
            {
                if(pTrigger.RequireKeyToActivate)
                    pTrigger.Notify();
                else pTrigger.Activate();
                NearbyTriggers.TryAdd(other, pTrigger);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (NearbyTriggers.ContainsKey(other)) {
            if(!NearbyTriggers[other].RequireKeyToDeactivate)
                NearbyTriggers[other].Deactivate();
            NearbyTriggers.Remove(other);
        }
    }
}
