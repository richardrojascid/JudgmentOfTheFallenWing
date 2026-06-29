using JudgmentOfTheFallenWing.Combat;
using JudgmentOfTheFallenWing.Core.Events;
using JudgmentOfTheFallenWing.Core.Managers;
using JudgmentOfTheFallenWing.Core.Save;
using UnityEngine;

namespace JudgmentOfTheFallenWing.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class AliaHealth : MonoBehaviour, IDamageable, ISaveable
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float invulnerabilityDuration = 1f;
        [SerializeField] private float knockbackForce = 5f;

        private float _currentHealth;
        private float _invulnerabilityTimer;
        private Rigidbody2D _rb;

        public bool IsAlive => _currentHealth > 0f;
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => maxHealth;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _currentHealth = maxHealth;
        }

        private void Start()
        {
            if (GameManager.Instance != null)
                RestoreState(GameManager.Instance.CurrentSave);

            GameEvents.RaisePlayerHealthChanged(Mathf.RoundToInt(_currentHealth), Mathf.RoundToInt(maxHealth));
        }

        private void Update()
        {
            if (_invulnerabilityTimer > 0f)
                _invulnerabilityTimer -= Time.deltaTime;
        }

        public void TakeDamage(DamageInfo damage)
        {
            if (!IsAlive || _invulnerabilityTimer > 0f) return;

            _currentHealth = Mathf.Max(0f, _currentHealth - damage.Amount);
            _invulnerabilityTimer = invulnerabilityDuration;

            if (damage.Knockback != Vector2.zero)
                _rb.velocity = damage.Knockback.normalized * knockbackForce;

            GameEvents.RaisePlayerHealthChanged(Mathf.RoundToInt(_currentHealth), Mathf.RoundToInt(maxHealth));

            if (!IsAlive)
            {
                GameEvents.RaisePlayerDied();
                Debug.Log("[ALIA] Ha caído...");
            }
        }

        public void Heal(float amount)
        {
            if (!IsAlive) return;
            _currentHealth = Mathf.Min(maxHealth, _currentHealth + amount);
            GameEvents.RaisePlayerHealthChanged(Mathf.RoundToInt(_currentHealth), Mathf.RoundToInt(maxHealth));
        }

        public void CaptureState(SaveData data)
        {
            data.playerHealth = _currentHealth;
            data.playerMaxHealth = maxHealth;
        }

        public void RestoreState(SaveData data)
        {
            maxHealth = data.playerMaxHealth > 0 ? data.playerMaxHealth : maxHealth;
            _currentHealth = data.playerHealth > 0 ? data.playerHealth : maxHealth;
        }
    }
}
