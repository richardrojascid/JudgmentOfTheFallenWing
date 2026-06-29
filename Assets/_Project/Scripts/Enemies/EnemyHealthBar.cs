using UnityEngine;
using UnityEngine.UI;

namespace JudgmentOfTheFallenWing.Enemies
{
    /// <summary>
    /// Barra de vida en world-space sobre el enemigo. Se genera automáticamente si no hay referencias.
    /// </summary>
    public class EnemyHealthBar : MonoBehaviour
    {
        [Header("Posición")]
        [SerializeField] private Vector3 offset = new(0f, 1.1f, 0f);
        [SerializeField] private Vector2 barSize = new(0.8f, 0.08f);

        [Header("Comportamiento")]
        [SerializeField] private bool showOnlyWhenDamaged = true;

        [Header("Colores")]
        [SerializeField] private Color backgroundColor = new(0.1f, 0.1f, 0.1f, 0.85f);
        [SerializeField] private Color fillColor = new(0.85f, 0.15f, 0.15f, 1f);
        [SerializeField] private Color fillColorLow = new(1f, 0.45f, 0.1f, 1f);
        [SerializeField] private float lowHealthThreshold = 0.3f;

        [Header("Referencias (opcional)")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image fillImage;
        [SerializeField] private CanvasGroup canvasGroup;

        private bool _isBuilt;

        private void Awake()
        {
            BuildIfNeeded();
            SetVisible(!showOnlyWhenDamaged);
        }

        public void UpdateBar(float current, float max)
        {
            BuildIfNeeded();

            if (max <= 0f || fillImage == null) return;

            var normalized = Mathf.Clamp01(current / max);
            fillImage.fillAmount = normalized;
            fillImage.color = normalized <= lowHealthThreshold ? fillColorLow : fillColor;

            if (showOnlyWhenDamaged)
                SetVisible(normalized < 0.999f && current > 0f);
        }

        public void Hide() => SetVisible(false);

        private void SetVisible(bool visible)
        {
            if (canvasGroup != null)
                canvasGroup.alpha = visible ? 1f : 0f;
            else if (canvas != null)
                canvas.enabled = visible;
        }

        private void BuildIfNeeded()
        {
            if (_isBuilt) return;
            _isBuilt = true;

            if (canvas != null && fillImage != null)
            {
                _barRoot = canvas.transform;
                EnsureCanvasGroup();
                return;
            }

            CreateDefaultBar();
        }

        private void CreateDefaultBar()
        {
            var canvasGo = new GameObject("HealthBarCanvas");
            canvasGo.transform.SetParent(transform, false);
            canvasGo.transform.localPosition = offset;

            canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 100;

            canvasGo.AddComponent<CanvasScaler>();

            var canvasRect = canvasGo.GetComponent<RectTransform>();
            canvasRect.sizeDelta = barSize;
            canvasRect.localScale = Vector3.one;
            _barRoot = canvasGo.transform;

            canvasGroup = canvasGo.AddComponent<CanvasGroup>();

            var bgGo = new GameObject("Background");
            bgGo.transform.SetParent(canvasGo.transform, false);
            var bgImage = bgGo.AddComponent<Image>();
            ApplyDefaultSprite(bgImage);
            bgImage.color = backgroundColor;
            StretchRect(bgGo.GetComponent<RectTransform>());

            var fillGo = new GameObject("Fill");
            fillGo.transform.SetParent(bgGo.transform, false);
            fillImage = fillGo.AddComponent<Image>();
            ApplyDefaultSprite(fillImage);
            fillImage.color = fillColor;
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            fillImage.fillAmount = 1f;
            StretchRect(fillGo.GetComponent<RectTransform>());
        }

        private void EnsureCanvasGroup()
        {
            if (canvasGroup == null && canvas != null)
                canvasGroup = canvas.GetComponent<CanvasGroup>() ?? canvas.gameObject.AddComponent<CanvasGroup>();
        }

        private static void StretchRect(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void ApplyDefaultSprite(Image image)
        {
            image.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
        }
    }
}
