using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ParentHouse.UI {
    public class JuicyCore : MonoBehaviour {
        
    }
    
    [Serializable]
    public abstract class Effect {
        
        [SerializeField] [FoldoutGroup("Settings")]
        public float EffectTime = 1;
        
        public virtual void Play() {
        }
        public virtual void PlayInstant() {
            Play();
        }
    }

    public class SpriteSwap : Effect {
        [SerializeField] private Image Img;
        [SerializeField] private Sprite Sprite;

        public override void Play() {
            Img.sprite = Sprite;
        }
        
        public override void PlayInstant() {
            Play();
        }
    }
    
    public class SetText : Effect {
        [SerializeField] private string Message;
        [SerializeField] private TextMeshProUGUI Text;

        public override void Play() {
            Text.text = Message;
        }
        
        public override void PlayInstant() {
            Play();
        }
    }

    public class EventTrigger : Effect {
        [SerializeField] private UnityEvent Event = new();
        public override void Play() {
            Event.Invoke();
        }
    }

    public class SetObjectActive : Effect {
        [SerializeField] private bool State;
        [SerializeField] [BoxGroup("Dependencies")]
        private GameObject Obj;
        public override void Play() {
            Obj.SetActive(State);
        }
    }

    public class FadeIn : Effect {
        [SerializeField] [FoldoutGroup("Settings")] float TargetValue;
        [SerializeField] [FoldoutGroup("Settings")] float Duration;
        [SerializeField] [FoldoutGroup("Dependencies")] CanvasGroup Group;
        
        public override void Play() {
            Group.DOFade(TargetValue, Duration);
        }

        public override void PlayInstant() {
            Group.alpha = TargetValue;
        }
    }
    
    public abstract class TweenEffect : Effect{
        [SerializeField] [FoldoutGroup("Settings")]
        protected Ease EaseType = Ease.InOutSine;

        [SerializeField] [FoldoutGroup("Settings")]
        protected float Overshoot;

        [SerializeField] [BoxGroup("Dependencies")]
        protected RectTransform Rect;
    }

    public class Scale : TweenEffect {
        [SerializeField] [FoldoutGroup("Settings")]
        private Vector3 TargetSize;

        public override void Play() {
            Rect.DOScale(TargetSize, EffectTime)
                .SetEase(EaseType, Overshoot);
        }
        public override void PlayInstant() {
            Rect.localScale = TargetSize;
        }
    }

    public class Rotate : TweenEffect {
        [SerializeField] [FoldoutGroup("Settings")]
        private RotateMode RotateMode;

        [SerializeField] [FoldoutGroup("Settings")]
        private Vector3 TargetAngle;

        public override void Play() {
            Rect.DORotate(TargetAngle, EffectTime, RotateMode)
                .SetEase(EaseType, Overshoot);
        }
        
        public override void PlayInstant() {
            Rect.rotation = Quaternion.Euler(TargetAngle);
        }
    }

    public class Move : TweenEffect {
        [SerializeField] [FoldoutGroup("Settings")]
        private Vector3 TargetPosition;

        public override void Play() {
            Rect.DOAnchorPos3D(TargetPosition, EffectTime)
                .SetEase(EaseType, Overshoot);
        }
        
        public override void PlayInstant() {
            Rect.anchoredPosition = TargetPosition;
        }
        
#if UNITY_EDITOR
        [Button]
        private void SetTargetPosition() {
            TargetPosition = Rect.anchoredPosition;
        }
#endif
    }
}
