using UnityEngine;

namespace JudgmentOfTheFallenWing.Enemies
{
    /// <summary>
    /// Barra de vida sobre el enemigo usando SpriteRenderers (fiable en 2D).
    /// </summary>
    public class EnemyHealthBar : MonoBehaviour
    {
        [Header("Posición")]
        [SerializeField] private Vector3 offset = new(0f, 1.1f, 0f);
        [SerializeField] private Vector2 barSize = new(0.8f, 0.08f);

        [Header("Comportamiento")]
        [SerializeField] private bool showOnlyWhenDamaged;

        [Header("Colores")]
        [SerializeField] private Color backgroundColor = new(0.1f, 0.1f, 0.1f, 0.9f);
        [SerializeField] private Color fillColor = new(0.85f, 0.15f, 0.15f, 1f);
        [SerializeField] private Color fillColorLow = new(1f, 0.45f, 0.1f, 1f);
        [SerializeField] private float lowHealthThreshold = 0.3f;

        [Header("Referencias (opcional)")]
        [SerializeField] private Transform barRoot;
        [SerializeField] private Transform fillTransform;
        [SerializeField] private SpriteRenderer fillRenderer;

        private SpriteRenderer _backgroundRenderer;
        private static Sprite _whiteSprite;
        private bool _isBuilt;

        private void Awake()
        {
            BuildIfNeeded();
            SetVisible(!showOnlyWhenDamaged);
        }

        public void UpdateBar(float current, float max)
        {
            BuildIfNeeded();

            if (max <= 0f || fillTransform == null) return;

            var normalized = Mathf.Clamp01(current / max);
            fillTransform.localScale = new Vector3(barSize.x * normalized, barSize.y, 1f);
            fillTransform.localPosition = new Vector3(-barSize.x * 0.5f + (barSize.x * normalized * 0.5f), 0f, 0f);

            if (fillRenderer != null)
                fillRenderer.color = normalized <= lowHealthThreshold ? fillColorLow : fillColor;

            if (showOnlyWhenDamaged)
                SetVisible(normalized < 0.999f && current > 0f);
            else
                SetVisible(current > 0f);
        }

        public void Hide() => SetVisible(false);

        private void SetVisible(bool visible)
        {
            if (barRoot != null)
                barRoot.gameObject.SetActive(visible);
        }

        private void BuildIfNeeded()
        {
            if (_isBuilt) return;
            _isBuilt = true;

            if (barRoot != null && fillTransform != null && fillRenderer != null)
                return;

            CreateDefaultBar();
        }

        private void CreateDefaultBar()
        {
            var rootGo = new GameObject("HealthBar");
            rootGo.transform.SetParent(transform, false);
            rootGo.transform.localPosition = offset;
            barRoot = rootGo.transform;

            var bgGo = new GameObject("Background");
            bgGo.transform.SetParent(barRoot, false);
            _backgroundRenderer = bgGo.AddComponent<SpriteRenderer>();
            _backgroundRenderer.sprite = GetWhiteSprite();
            _backgroundRenderer.color = backgroundColor;
            bgGo.transform.localScale = new Vector3(barSize.x, barSize.y, 1f);

            var fillGo = new GameObject("Fill");
            fillGo.transform.SetParent(barRoot, false);
            fillRenderer = fillGo.AddComponent<SpriteRenderer>();
            fillRenderer.sprite = GetWhiteSprite();
            fillRenderer.color = fillColor;
            fillGo.transform.localScale = new Vector3(barSize.x, barSize.y, 1f);
            fillGo.transform.localPosition = Vector3.zero;
            fillTransform = fillGo.transform;

            ApplySorting();
        }

        private void ApplySorting()
        {
            var reference = GetComponentInParent<SpriteRenderer>();
            if (reference == null) return;

            _backgroundRenderer.sortingLayerID = reference.sortingLayerID;
            fillRenderer.sortingLayerID = reference.sortingLayerID;
            _backgroundRenderer.sortingOrder = reference.sortingOrder + 10;
            fillRenderer.sortingOrder = reference.sortingOrder + 11;
        }

        private static Sprite GetWhiteSprite()
        {
            if (_whiteSprite != null) return _whiteSprite;

            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();

            _whiteSprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, 1f, 1f),
                new Vector2(0.5f, 0.5f),
                1f);

            return _whiteSprite;
        }
    }
}
