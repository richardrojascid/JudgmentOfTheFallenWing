using System;
using JudgmentOfTheFallenWing.Combat;
using UnityEngine;

namespace JudgmentOfTheFallenWing.Enemies
{
    /// <summary>
    /// Vida del enemigo. Sincroniza la barra con Enemy Damage u otros scripts vía SetHealth().
    /// </summary>
    [RequireComponent(typeof(EnemyHealthBar))]
    [DisallowMultipleComponent]
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 30f;
        [SerializeField] private float currentHealth = 30f;

        private EnemyHealthBar _healthBar;

        public bool IsAlive => currentHealth > 0f;
        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;

        public event Action<float, float> OnHealthChanged;
        public event Action OnDied;

        private void Reset()
        {
            currentHealth = maxHealth;
        }

        private void Awake()
        {
            _healthBar = GetComponent<EnemyHealthBar>();
            if (_healthBar == null)
                _healthBar = gameObject.AddComponent<EnemyHealthBar>();

            if (currentHealth <= 0f || currentHealth > maxHealth)
                currentHealth = maxHealth;
        }

        private void Start()
        {
            RefreshBar();
        }

        private void OnValidate()
        {
            if (maxHealth < 1f) maxHealth = 1f;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        }

        public void TakeDamage(float amount)
        {
            TakeDamage(new DamageInfo(amount, Vector2.zero, null));
        }

        public void TakeDamage(DamageInfo damage)
        {
            if (!IsAlive) return;

            currentHealth = Mathf.Max(0f, currentHealth - damage.Amount);
            RefreshBar();

            Debug.Log($"{name} recibió {damage.Amount} de daño. Vida: {currentHealth}");
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (!IsAlive)
            {
                Debug.Log($"{name} fue derrotado.");
                _healthBar.Hide();
                OnDied?.Invoke();
            }
        }

        /// <summary>
        /// Llamar desde Enemy Damage u otro script al cambiar la vida.
        /// </summary>
        public void SetHealth(float current)
        {
            currentHealth = Mathf.Clamp(current, 0f, maxHealth);
            RefreshBar();
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (!IsAlive)
                _healthBar.Hide();
        }

        public void SetMaxHealth(float max, bool refill = true)
        {
            maxHealth = Mathf.Max(1f, max);
            if (refill)
                currentHealth = maxHealth;
            RefreshBar();
        }

        public void RefreshBar()
        {
            _healthBar?.UpdateBar(currentHealth, maxHealth);
        }
    }
}
