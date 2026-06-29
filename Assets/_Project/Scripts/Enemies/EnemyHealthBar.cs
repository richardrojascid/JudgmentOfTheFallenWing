using UnityEngine;

namespace JudgmentOfTheFallenWing.Enemies
{
    /// <summary>
    /// Barra de vida sobre el enemigo usando SpriteRenderers (compatible URP 2D).
    /// </summary>
    [DisallowMultipleComponent]
    public class EnemyHealthBar : MonoBehaviour
    {
        [Header("Posición")]
        [SerializeField] private bool autoOffsetFromSprite = true;
        [SerializeField] private float offsetPadding = 0.15f;
        [SerializeField] private Vector3 offset = new(0f, 1.1f, 0f);
        [SerializeField] private Vector2 barSize = new(1f, 0.12f);

        [Header("Comportamiento")]
        [SerializeField] private bool showOnlyWhenDamaged;

        [Header("Colores")]
        [SerializeField] private Color backgroundColor = new(0.1f, 0.1f, 0.1f, 0.95f);
        [SerializeField] private Color fillColor = new(0.9f, 0.15f, 0.15f, 1f);
        [SerializeField] private Color fillColorLow = new(1f, 0.45f, 0.1f, 1f);
        [SerializeField] private float lowHealthThreshold = 0.3f;

        private Transform _barRoot;
        private Transform _fillTransform;
        private SpriteRenderer _backgroundRenderer;
        private SpriteRenderer _fillRenderer;
        private SpriteRenderer _ownerSprite;
        private static Sprite _whiteSprite;
        private static Material _fallbackMaterial;
        private bool _isBuilt;

        private void Awake()
        {
            ResolveOffset();
            BuildIfNeeded();
        }

        private void Start()
        {
            SyncFromHealthComponent();
        }

        private void LateUpdate()
        {
            SyncFlipFromOwner();
        }

        public void UpdateBar(float current, float max)
        {
            BuildIfNeeded();

            if (max <= 0f || _fillTransform == null) return;

            var normalized = Mathf.Clamp01(current / max);
            _fillTransform.localScale = new Vector3(barSize.x * normalized, barSize.y, 1f);
            _fillTransform.localPosition = new Vector3(
                -barSize.x * 0.5f + (barSize.x * normalized * 0.5f),
                0f,
                -0.01f);

            if (_fillRenderer != null)
                _fillRenderer.color = normalized <= lowHealthThreshold ? fillColorLow : fillColor;

            var visible = showOnlyWhenDamaged
                ? normalized < 0.999f && current > 0f
                : current > 0f;

            SetVisible(visible);
        }

        public void Hide() => SetVisible(false);

        public void ForceShow()
        {
            BuildIfNeeded();
            UpdateBar(1f, 1f);
            SetVisible(true);
        }

        private void SyncFromHealthComponent()
        {
            var health = GetComponent<EnemyHealth>();
            if (health != null)
            {
                UpdateBar(health.CurrentHealth, health.MaxHealth);
                return;
            }

            // Fallback: mostrar barra llena para confirmar que renderiza
            ForceShow();
        }

        private void SyncFlipFromOwner()
        {
            if (_ownerSprite == null || _backgroundRenderer == null || _fillRenderer == null) return;

            _backgroundRenderer.flipX = _ownerSprite.flipX;
            _fillRenderer.flipX = _ownerSprite.flipX;
        }

        private void ResolveOffset()
        {
            _ownerSprite = GetComponent<SpriteRenderer>();
            if (!autoOffsetFromSprite || _ownerSprite == null || _ownerSprite.sprite == null) return;

            offset.y = _ownerSprite.sprite.bounds.extents.y + offsetPadding;
        }

        private void SetVisible(bool visible)
        {
            if (_barRoot != null)
                _barRoot.gameObject.SetActive(visible);
        }

        private void BuildIfNeeded()
        {
            if (_isBuilt && _barRoot != null) return;

            DestroyExistingBar();
            CreateDefaultBar();
            _isBuilt = true;
        }

        private void DestroyExistingBar()
        {
            var existing = transform.Find("HealthBar");
            if (existing != null)
                Destroy(existing.gameObject);
        }

        private void CreateDefaultBar()
        {
            _ownerSprite ??= GetComponent<SpriteRenderer>();

            var rootGo = new GameObject("HealthBar");
            rootGo.transform.SetParent(transform, false);
            rootGo.transform.localPosition = new Vector3(offset.x, offset.y, -0.01f);
            _barRoot = rootGo.transform;

            _backgroundRenderer = CreateBarPart("Background", backgroundColor, out var bgTransform);
            bgTransform.localScale = new Vector3(barSize.x, barSize.y, 1f);

            _fillRenderer = CreateBarPart("Fill", fillColor, out _fillTransform);
            _fillTransform.localScale = new Vector3(barSize.x, barSize.y, 1f);
            _fillTransform.localPosition = new Vector3(0f, 0f, -0.001f);

            ApplyRendering(_backgroundRenderer, 10);
            ApplyRendering(_fillRenderer, 11);
        }

        private SpriteRenderer CreateBarPart(string partName, Color color, out Transform partTransform)
        {
            var go = new GameObject(partName);
            go.transform.SetParent(_barRoot, false);

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = GetWhiteSprite();
            renderer.color = color;
            partTransform = go.transform;
            return renderer;
        }

        private void ApplyRendering(SpriteRenderer target, int orderOffset)
        {
            var reference = _ownerSprite ?? GetComponent<SpriteRenderer>();

            if (reference != null)
            {
                target.sortingLayerID = reference.sortingLayerID;
                target.sortingOrder = reference.sortingOrder + orderOffset;

                if (reference.sharedMaterial != null)
                {
                    target.sharedMaterial = reference.sharedMaterial;
                    return;
                }
            }

            target.sharedMaterial = GetFallbackMaterial();
        }

        private static Material GetFallbackMaterial()
        {
            if (_fallbackMaterial != null) return _fallbackMaterial;

            var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default")
                         ?? Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default")
                         ?? Shader.Find("Sprites/Default");

            if (shader != null)
                _fallbackMaterial = new Material(shader);

            return _fallbackMaterial;
        }

        private static Sprite GetWhiteSprite()
        {
            if (_whiteSprite != null) return _whiteSprite;

            var texture = new Texture2D(4, 4, TextureFormat.RGBA32, false);
            var pixels = new Color[16];
            for (var i = 0; i < pixels.Length; i++)
                pixels[i] = Color.white;
            texture.SetPixels(pixels);
            texture.Apply();
            texture.filterMode = FilterMode.Bilinear;

            _whiteSprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, 4f, 4f),
                new Vector2(0.5f, 0.5f),
                4f);

            return _whiteSprite;
        }
    }
}
