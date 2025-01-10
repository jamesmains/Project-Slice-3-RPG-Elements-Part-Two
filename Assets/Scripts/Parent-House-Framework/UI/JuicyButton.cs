using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ParentHouse.UI {
    public class JuicyButton : SerializedMonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler,
        IPointerExitHandler, ISubmitHandler, ICancelHandler {
        [SerializeField] private string ButtonName;
        
        [SerializeField] [FoldoutGroup("Settings")]
        private bool RequireHoldTime;

        [SerializeField] [FoldoutGroup("Settings")] [ShowIf("RequireHoldTime")]
        private float HoldTime;

        [SerializeField] [FoldoutGroup("Settings")] [ShowIf("RequireHoldTime")]
        private bool ForceReleaseWhenHeldTimeIsMet;
        
        [SerializeField] [FoldoutGroup("Settings")] [ReadOnly] [ShowIf("RequireHoldTime")]
        private float StartHoldTime;

        [SerializeField] [FoldoutGroup("Settings")] [ReadOnly] [ShowIf("RequireHoldTime")]
        private bool IsHolding;

        [SerializeField] [FoldoutGroup("Settings")]
        private List<Effect> PointerEnterEffect = new();
        
        [SerializeField] [FoldoutGroup("Settings")]
        private List<Effect> PointerExitEffects = new();
        
        [SerializeField] [FoldoutGroup("Settings")]
        private List<Effect> PointerUpEffects = new();

        [SerializeField] [FoldoutGroup("Settings")]
        private List<Effect> PointerDownEffects = new();

        [SerializeField] [FoldoutGroup("Settings")]
        private List<Effect> ClickEffects = new();

        [SerializeField] [FoldoutGroup("Events")]
        private UnityEvent OnButtonUp;

        [SerializeField] [FoldoutGroup("Events")]
        private UnityEvent OnButtonDown;

        [SerializeField] [FoldoutGroup("Events")]
        private UnityEvent OnClick;

        [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
        private bool IsMouseOver;
        
        #if UNITY_EDITOR
        private void OnValidate() {
            this.gameObject.name = $"Button_{ButtonName}";
        }
        #endif

        private void OnEnable() {
            if (!RequireHoldTime) {
                ForceReleaseWhenHeldTimeIsMet = false;
            }
        }

        private Coroutine ForceReleaseCoroutine;

        private void Release() {
            IsHolding = false;
            ButtonUp();
            if (!IsMouseOver) return;
            if (RequireHoldTime && Time.time < StartHoldTime + HoldTime) return;
            Click();
        }

        private void ButtonUp() {
            if (ForceReleaseCoroutine != null) {
                StopCoroutine(ForceReleaseCoroutine);
                ForceReleaseCoroutine = null;
            }
            foreach (var upEffect in PointerUpEffects) {
                upEffect.Play();
            }
            OnButtonUp.Invoke();
        }
        
        private void Click() {
            OnClick.Invoke();
            print("Click");
            foreach (var clickEffect in ClickEffects) {
                clickEffect.Play();
            }
        }

        public void OnPointerEnter(PointerEventData eventData) {
            print("Enter");
            IsMouseOver = true;
            foreach (var enterEffect in PointerEnterEffect) {
                enterEffect.Play();
            }
        }

        public void OnPointerDown(PointerEventData eventData) {
            OnButtonDown.Invoke();
            IsHolding = true;
            if (RequireHoldTime) {
                StartHoldTime = Time.time;
            }

            if (ForceReleaseWhenHeldTimeIsMet) {
                ForceReleaseCoroutine = StartCoroutine(ForceReleaseDelay());

                IEnumerator ForceReleaseDelay() {
                    yield return new WaitForSeconds(HoldTime);
                    Release();
                }
            }

            foreach (var downEffect in PointerDownEffects) {
                downEffect.Play();
            }
        }

        public void OnPointerUp(PointerEventData eventData) {
            if (IsHolding)
                Release();
        }

        public void OnPointerExit(PointerEventData eventData) {
            IsMouseOver = false;
            foreach (var exitEffects in PointerExitEffects) {
                exitEffects.Play();
            }
        }

        public void OnSubmit(BaseEventData eventData) {
            print("Submit");
        }

        public void OnCancel(BaseEventData eventData) {
            throw new NotImplementedException();
        }
    }
}