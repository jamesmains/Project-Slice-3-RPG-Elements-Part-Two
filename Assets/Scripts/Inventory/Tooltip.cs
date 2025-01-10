using System;
using System.Collections;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {
    [SerializeField, FoldoutGroup("Settings")]
    private float Delay = .5f;

    [SerializeField, FoldoutGroup("Dependencies")]
    private PlayerInput AssociatedPlayerInput;
    [SerializeField, FoldoutGroup("Dependencies")]
    private TextMeshProUGUI TooltipText;

    [SerializeField, FoldoutGroup("Dependencies")]
    private CanvasGroup TooltipCanvasGroup;

    [SerializeField, FoldoutGroup("Dependencies")]
    private LayoutGroup LayoutGroup;

    [SerializeField, FoldoutGroup("Dependencies")]
    private RectTransform TooltipRect;
    
    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private Vector2 CachedTooltipPosition;

    public static Action<string> OnShowTooltip;
    public static Action OnHideTooltip;
    
    private Coroutine ShowTooltipCoroutine;

    private void OnEnable() {
        OnShowTooltip += Show;
        OnHideTooltip += Hide;
        Hide();
    }

    private void OnDisable() {
        OnShowTooltip -= Show;
        OnHideTooltip -= Hide;
    }

    public void Show(string text) {
        TooltipText.text = text;
        if (ShowTooltipCoroutine == null) {
            TooltipCanvasGroup.alpha = 0f;
            ShowTooltipCoroutine = StartCoroutine(Appear());
        }
        else SetPosition();

        IEnumerator Appear() {
            yield return new WaitForSeconds(Delay);
            CachePosition();
            SetPosition();
            TooltipCanvasGroup.alpha = 1f;
        }
    }

    private void CachePosition() {
        var pos = AssociatedPlayerInput.InputSystem.UI.Point.ReadValue<Vector2>();
        pos.x += TooltipRect.rect.width;
        pos.y += -TooltipRect.rect.height;
        TooltipRect.position = pos;
        CachedTooltipPosition = TooltipRect.position;
    }

    private void SetPosition() {
        var pos = AssociatedPlayerInput.InputSystem.UI.Point.ReadValue<Vector2>();
        pos.x += CachedTooltipPosition.x + TooltipRect.rect.width > Screen.width ? -TooltipRect.rect.width : TooltipRect.rect.width;
        pos.y += CachedTooltipPosition.y - TooltipRect.rect.height < 0 ? TooltipRect.rect.height : -TooltipRect.rect.height;
        TooltipRect.position = pos;
    }

    private void Hide() {
        if (ShowTooltipCoroutine != null) {
            StopCoroutine(ShowTooltipCoroutine);
            ShowTooltipCoroutine = null;
        }
        TooltipText.text = "";
        TooltipCanvasGroup.alpha = 0f;
    }
}