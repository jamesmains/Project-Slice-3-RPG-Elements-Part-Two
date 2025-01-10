using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class TargetLockVolume : MonoBehaviour {
    [SerializeField, FoldoutGroup("Dependencies")]
    public Transform PlayerTransform;
    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private List<Transform> Targets = new();
    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    public Transform CurrentTarget;
    
    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            FindNextTarget(-1);
        }

        if (Input.GetMouseButtonDown(1)) {
            FindNextTarget(1);
        }
    }

    [Button]
    public void ClearTarget() {
        CurrentTarget = null;
    }

    [Button]
    public Transform FindNearest() {
        if (Targets.Count == 0) return null;
        Debug.Log("Trying to find nearest");
        Targets = Targets.OrderBy(o => Vector3.Distance(o.position, PlayerTransform.position)).ToList();
        CurrentTarget = Targets[0];
        return CurrentTarget;
    }

    [Button]
    public Transform FindNextTarget(int direction) {
        
        if (Targets.Count == 0 || CurrentTarget == null) return null;

        Targets = Targets.OrderBy(o => Vector3.Distance(o.position, PlayerTransform.position)).ToList();

        int currentIndex = Targets.IndexOf(CurrentTarget);

        int newIndex = (currentIndex + direction + Targets.Count) % Targets.Count;

        CurrentTarget = Targets[newIndex];
        return CurrentTarget;
    }
    
    
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Entity") && !Targets.Contains(other.transform)) {
            Targets.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Entity") && Targets.Contains(other.transform)) {
            Targets.Remove(other.transform);
            if (CurrentTarget == other.transform) {
                ClearTarget();
            }
        }
    }
}
