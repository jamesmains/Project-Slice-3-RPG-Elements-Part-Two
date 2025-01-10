using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class GhostIcon : MonoBehaviour {
    [SerializeField, BoxGroup("Dependencies")]
    public RectTransform Rect;

    [SerializeField, BoxGroup("Dependencies")]
    public Image Icon;
    
    [SerializeField, BoxGroup("Dependencies")]
    private CanvasGroup CanvasGroup;
    public static GhostIcon Singleton;

    private void Awake() {
        if (Singleton == null) {
            Singleton = this;
            SetVisibility(false);
        }
        else Destroy(gameObject);
    }

    public void SetVisibility(bool isVisible, Sprite icon = null) {
        Icon.sprite = icon;
        CanvasGroup.alpha = isVisible ? 1 : 0;
    }

    public void SetPosition(Vector2 position) {
        // position.x -= Screen.width * 0.5f;
        // position.y -= Screen.height * 0.5f;
        Rect.position = Input.mousePosition;
    }
}
