using System;
using ParentHouse.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerInput : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Dependencies")]
    private Player CurrentPlayer;

    [SerializeField] [FoldoutGroup("Dependencies")]
    private Entity PlayerEntity;

    [SerializeField, FoldoutGroup("Settings")]
    private float GamepadLookSpeedModifier = 100f;

    [SerializeField, FoldoutGroup("Settings")]
    public bool invertHorizontalInput = false;

    [SerializeField, FoldoutGroup("Settings")]
    public bool invertVerticalInput = false;

    [SerializeField, FoldoutGroup("Settings")]
    public float mouseInputMultiplier = 0.01f;

    [HideInInspector] public InputSystem_Actions InputSystem;

    private string CurrentInput;

    private void OnEnable() {
        InputSystem ??= new InputSystem_Actions();
        InputSystem.Enable();
        InputSystem.Player.Attack.performed += Attack;
        InputSystem.Player.LockOn.performed += CurrentPlayer.TryLockOnTarget;
        InputSystem.Player.ChangeLockOnTarget.performed += CurrentPlayer.TryChangeLockOnTarget;
        UnityEngine.InputSystem.InputSystem.onEvent += SetControlType;
    }

    private void OnDisable() {
        InputSystem.Player.Attack.performed -= Attack;
        InputSystem.Player.LockOn.performed -= CurrentPlayer.TryLockOnTarget;
        InputSystem.Player.ChangeLockOnTarget.performed -= CurrentPlayer.TryChangeLockOnTarget;
        UnityEngine.InputSystem.InputSystem.onEvent -= SetControlType;
        InputSystem?.Disable();
    }

    private void SetControlType(InputEventPtr arg1, InputDevice arg2) {
        CurrentInput = arg2.device.name.ToLower();
    }

    private void Attack(InputAction.CallbackContext obj) {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        PlayerEntity.Attack();
    }

    public Vector2 GetPlayerMoveInput() {
        return InputSystem.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetPlayerLookInput() {
        if((CurrentInput == "mouse" || CurrentInput == "keyboard") && !Input.GetMouseButton(0))
            return Vector2.zero;
        if((CurrentInput == "mouse" || CurrentInput == "keyboard") && EventSystem.current.IsPointerOverGameObject())
            return Vector2.zero;
        var lookInputVector = InputSystem.Player.Look.ReadValue<Vector2>();

        if (Time.timeScale > 0f && Time.deltaTime > 0f) {
            lookInputVector /= Time.deltaTime;
            lookInputVector *= Time.timeScale;
        }
        else
            lookInputVector = Vector2.zero;

        lookInputVector *= mouseInputMultiplier;

        if (invertHorizontalInput)
            lookInputVector.x *= -1f;

        if (invertVerticalInput)
            lookInputVector.y *= -1f;

        if (CurrentInput == "gamepad")
            lookInputVector *= GamepadLookSpeedModifier;

        return lookInputVector;
    }

    public bool IsJumpKeyPressed() {
        return InputSystem.Player.Jump.ReadValue<float>() > 0;
    }
}