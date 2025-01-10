using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class DEBUGTargetFinder : MonoBehaviour {
    public Transform Origin;
    public Transform CurrentTarget;
    public List<Transform> Targets = new();

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            FindLinearNextTarget(-1);
        }

        if (Input.GetMouseButtonDown(1)) {
            FindLinearNextTarget(1);
        }
    }

    [Button]
    public void ClearTarget() {
        CurrentTarget = null;
    }

    [Button]
    public void FindNearest() {
        Targets = Targets.OrderBy(o => Vector3.Distance(o.position, Origin.position)).ToList();
        CurrentTarget = Targets[0];
    }

    [Button]
    public void FindLinearNextTarget(int direction) {
        if (CurrentTarget == null) FindNearest();

        var sortedTargets = Targets.OrderBy(t => t.position.x).ToArray();

        int currentIndex = Array.IndexOf((Array)sortedTargets, CurrentTarget);

        // int newIndex = (currentIndex + direction + sortedTargets.Length) % sortedTargets.Length;
        int newIndex = (currentIndex + direction) >= Targets.Count || (currentIndex + direction) < 0
            ? currentIndex
            : currentIndex + direction;

        CurrentTarget.localScale = new Vector3(1, 1, 1);
        CurrentTarget = sortedTargets[newIndex];
        CurrentTarget.localScale = new Vector3(2, 2, 2);
    }
}