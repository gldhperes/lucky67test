
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonScaleEffect : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("Referência")]
#if UNITY_EDITOR
    [ReadOnly]
#endif
    [SerializeField] private RectTransform target;

    [Header("Hover")]
    [SerializeField] private bool enableHover = true;
    [SerializeField, Min(1f)] private float hoverScale = 1.1f;
    [SerializeField, Min(0f)] private float hoverDuration = 0.1f;

    [Header("Click")]
    [SerializeField] private bool enableClick = true;
    [SerializeField, Range(0.1f, 1f)] private float clickScale = 0.9f;
    [SerializeField, Min(0f)] private float clickDuration = 0.1f;

    [Header("Animação")]
    [SerializeField] private AnimationCurve easing = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private bool useUnscaledTime = true;

    private Vector3 originalScale;
    private bool isHovering;
    private bool isPressed;

    private Coroutine scaleCoroutine;

    private void Awake()
    {
        if (target == null)
            target = GetComponent<RectTransform>();

        if (target == null)
        {
            Debug.LogError(
                "UIButtonScaleEffect precisa de um RectTransform.",
                this
            );

            enabled = false;
            return;
        }

        originalScale = target.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        UpdateScale();

        AudioManager.Instance?.PlaySFX(AudioManager.Instance.buttonHoverSfx);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        UpdateScale();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        UpdateScale();
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.buttonClickSfx);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        UpdateScale();
    }

    private void UpdateScale()
    {
        if (target == null)
            return;

        float scaleMultiplier = 1f;
        float duration = 0f;

        if (isPressed && enableClick)
        {
            scaleMultiplier = clickScale;
            duration = clickDuration;
        }
        else if (isHovering && enableHover)
        {
            scaleMultiplier = hoverScale;
            duration = hoverDuration;
        }

        Vector3 targetScale = originalScale * scaleMultiplier;

        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);

        scaleCoroutine = StartCoroutine(
            AnimateScale(targetScale, duration)
        );
    }

    private IEnumerator AnimateScale(
        Vector3 targetScale,
        float duration
    )
    {
        Vector3 startScale = target.localScale;

        if (duration <= 0f)
        {
            target.localScale = targetScale;
            scaleCoroutine = null;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += useUnscaledTime
                ? Time.unscaledDeltaTime
                : Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float curveValue = easing.Evaluate(t);

            target.localScale = Vector3.LerpUnclamped(
                startScale,
                targetScale,
                curveValue
            );

            yield return null;
        }

        target.localScale = targetScale;
        scaleCoroutine = null;
    }

    private void OnDisable()
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
            scaleCoroutine = null;
        }

        isHovering = false;
        isPressed = false;

        if (target != null)
            target.localScale = originalScale;
    }
}
