using System;
using JudgmentOfTheFallenWing.Combat;
using UnityEngine;

namespace JudgmentOfTheFallenWing.Enemies
{
    /// <summary>
    /// Vida genérica para enemigos. Funciona solo o junto a tu script de IA existente.
    /// Añadir a Poseido_Condenado_01, Zombi_Penitente_01, etc.
    /// </summary>
    [RequireComponent(typeof(EnemyHealthBar))]
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 30f;

        private float _currentHealth;
        private EnemyHealthBar _healthBar;

        public bool IsAlive => _currentHealth > 0f;
        public float MaxHealth => maxHealth;
        public float CurrentHealth => _currentHealth;

        public event Action<float, float> OnHealthChanged;
        public event Action OnDied;

        private void Awake()
        {
            _healthBar = GetComponent<EnemyHealthBar>();
            if (_healthBar == null)
                _healthBar = gameObject.AddComponent<EnemyHealthBar>();

            _currentHealth = maxHealth;
        }

        private void Start()
        {
            RefreshBar();
        }

        public void TakeDamage(float amount)
        {
            TakeDamage(new DamageInfo(amount, Vector2.zero, null));
        }

        public void TakeDamage(DamageInfo damage)
        {
            if (!IsAlive) return;

            _currentHealth = Mathf.Max(0f, _currentHealth - damage.Amount);
            RefreshBar();

            Debug.Log($"{name} recibió {damage.Amount} de daño. Vida: {_currentHealth}");

            OnHealthChanged?.Invoke(_currentHealth, maxHealth);

            if (!IsAlive)
            {
                Debug.Log($"{name} fue derrotado.");
                _healthBar.Hide();
                OnDied?.Invoke();
            }
        }

        /// <summary>
        /// Usar desde tu script de enemigo si ya gestionas la vida manualmente.
        /// </summary>
        public void SetHealth(float current)
        {
            _currentHealth = Mathf.Clamp(current, 0f, maxHealth);
            RefreshBar();
            OnHealthChanged?.Invoke(_currentHealth, maxHealth);
        }

        public void RefreshBar()
        {
            _healthBar?.UpdateBar(_currentHealth, maxHealth);
        }
    }
}
