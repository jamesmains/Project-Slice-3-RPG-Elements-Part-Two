using System;
using System.Collections.Generic;
using CMF;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[Flags]
public enum PlayerState {
    Paused = 0,
    Menu = 1 << 0,
    MoveLocked = 1 << 1, // Can't move
    LookLocked = 1 << 2, // Can't free cam
}
public class Player : MonoBehaviour {
    [SerializeField, FoldoutGroup("Dependencies")]
    private TargetLockVolume LockTarget;
    
    [SerializeField, FoldoutGroup("Dependencies")]
    private TurnTowardControllerVelocity TurnByVelocity;
    
    [SerializeField, FoldoutGroup("Dependencies")]
    private TurnTowardCameraDirection TurnByCam;
    
    [SerializeField, FoldoutGroup("Dependencies")]
    private CameraController CameraController;
    
    [SerializeField,FoldoutGroup("Dependencies")]
    public Entity PlayerEntity;
    
    [SerializeField,FoldoutGroup("Status")]
    public PlayerState CurrentState = PlayerState.Paused;
    
    [SerializeField,FoldoutGroup("Status")]
    public PlayerState CachedState = PlayerState.Paused;
    
    public Action<List<Item>> OnItemAdded;
    public Action OnPlayerStatsChanged;
    
    public static Action<Player> OnPlayerCreated;
    
    private void Awake() {
        OnPlayerCreated?.Invoke(this);
    }

    private void OnEnable() {
        OnItemAdded += PlayerEntity.EntityEquipment.SetEquippedItems;
        PlayerEntity.OnStatLeveled += UpdatePlayerStatBreakdown;
    }

    private void OnDisable() {
        OnItemAdded -= PlayerEntity.EntityEquipment.SetEquippedItems;
        PlayerEntity.OnStatLeveled -= UpdatePlayerStatBreakdown;
    }
    
    private void UpdatePlayerStatBreakdown(Stat leveledStat)
    {
        Debug.Log($"{leveledStat.Details.SkillName} Leveled Up! {leveledStat.Level}");
        OnPlayerStatsChanged?.Invoke();
    }

    public void TryLockOnTarget(InputAction.CallbackContext obj) {
        var lockedOnTarget = LockTarget.FindNearest();
        UpdateTarget(lockedOnTarget);
    }
    
    public void TryChangeLockOnTarget(InputAction.CallbackContext obj) {
        int dir = Math.Clamp((int)obj.ReadValue<Vector2>().x,-1,1);
        var lockedOnTarget = LockTarget.FindNextTarget(dir);
        UpdateTarget(lockedOnTarget);
    }

    private void UpdateTarget(Transform target) {
        CameraController.DEBUG_Target = target;
        TurnByCam.enabled = target != null;
        TurnByVelocity.enabled = target == null;
    }
}
