using System.Collections;
using System.Collections.Generic;
using ParentHouse.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace ParentHouse.UI {
    public enum MenuState {
        Closed,
        Open
    }

    [RequireComponent(typeof(CanvasGroup))]
    public class Menu : SerializedMonoBehaviour {

        [SerializeField] [FoldoutGroup("Settings")]
        private MenuState InitialState = MenuState.Open;

        [SerializeField] [FoldoutGroup("Settings")]
        private bool InstantOnEnable = true;

        [SerializeField] [FoldoutGroup("Settings")]
        private List<Effect> OpenEffects = new();

        [SerializeField] [FoldoutGroup("Settings")]
        private List<Effect> CloseEffects = new();

        [SerializeField] [FoldoutGroup("Events")]
        private UnityEvent OnFinishOpen = new();

        [SerializeField] [FoldoutGroup("Events")]
        private UnityEvent OnFinishClose = new();

        [SerializeField] [FoldoutGroup("Dependencies")] [ReadOnly]
        private CanvasGroup CanvasGroup;

        [SerializeField] [FoldoutGroup("Dependencies")] [ReadOnly]
        private RectTransform Rect;

        [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
        public MenuState State;

        [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
        private float OpenTime;

        [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
        private float CloseTime;

        [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
        private bool Initialized;

#if UNITY_EDITOR
        private void OnValidate() {
            if (TryGetComponent(out CanvasGroup cg)) {
                CanvasGroup = cg;
            }

            if (TryGetComponent(out RectTransform r)) {
                Rect = r;
            }
        }
#endif
        private void OnEnable() {
            Initialized = false;
            if (CanvasGroup == null) CanvasGroup = GetComponent<CanvasGroup>();
            if (Rect == null) Rect = GetComponent<RectTransform>();

            SetTimes();
            if (InitialState == MenuState.Closed) {
                Open(true);
                Initialized = true;
                Close(InstantOnEnable);
            }
            else {
                Close(true);
                Initialized = true;
                Open(InstantOnEnable);
            }

            StartCoroutine(Init());

            IEnumerator Init() {
                yield return new WaitForEndOfFrame();
            }
        }

        private void SetTimes() {
            foreach (var e in OpenEffects) {
                OpenTime = e.EffectTime > OpenTime ? e.EffectTime : OpenTime;
            }

            foreach (var e in CloseEffects) {
                CloseTime = e.EffectTime > CloseTime ? e.EffectTime : CloseTime;
            }
        }

        [Button]
        public void Toggle() {
            if (State == MenuState.Open) {
                Close();
            }
            else Open();
        }

        [Button]
        public void Open(bool instant = false) {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                foreach (var e in OpenEffects) {
                    e.PlayInstant();
                }
                return;
            }
#endif
            foreach (var e in OpenEffects) {
                if (instant)
                    e.PlayInstant();
                else e.Play();
            }

            Activate();
        }

        [Button]
        public void Close(bool instant = false) {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                foreach (var e in CloseEffects) {
                    e.PlayInstant();
                }

                return;
            }
#endif
            foreach (var e in CloseEffects) {
                if (instant)
                    e.PlayInstant();
                else e.Play();
            }

            Deactivate();
        }

        private void Activate() {
            CanvasGroup.blocksRaycasts = true;
            CanvasGroup.interactable = true;
            State = MenuState.Open;

            if (!Initialized) return;
            StartCoroutine(WaitToOpen());

            IEnumerator WaitToOpen() {
                yield return new WaitForSeconds(OpenTime);
                OnFinishOpen.Invoke();
            }
        }

        private void Deactivate() {
            CanvasGroup.blocksRaycasts = false;
            CanvasGroup.interactable = false;
            State = MenuState.Closed;

            if (!Initialized) return;
            StartCoroutine(WaitToClose());

            IEnumerator WaitToClose() {
                yield return new WaitForSeconds(CloseTime);
                OnFinishClose.Invoke();
            }
        }
    }
}