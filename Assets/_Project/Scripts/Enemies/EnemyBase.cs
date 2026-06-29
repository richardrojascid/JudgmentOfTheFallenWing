using JudgmentOfTheFallenWing.Combat;
using UnityEngine;

namespace JudgmentOfTheFallenWing.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(EnemyHealthBar))]
    public class EnemyBase : MonoBehaviour, IDamageable
    {
        [SerializeField] protected float maxHealth = 30f;
        [SerializeField] protected float moveSpeed = 3f;
        [SerializeField] protected float detectionRange = 6f;
        [SerializeField] protected float attackRange = 1.5f;
        [SerializeField] protected Transform[] patrolPoints;
        [SerializeField] protected Hitbox attackHitbox;

        protected Rigidbody2D _rb;
        protected Transform Player;
        protected float _currentHealth;
        protected int PatrolIndex;
        protected bool IsAttacking;

        private EnemyHealthBar _healthBar;

        public bool IsAlive => _currentHealth > 0f;
        public float MaxHealth => maxHealth;
        public float CurrentHealth => _currentHealth;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _healthBar = GetComponent<EnemyHealthBar>();
            _currentHealth = maxHealth;
        }

        protected virtual void Start()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                Player = playerObj.transform;

            RefreshHealthBar();
        }

        protected virtual void Update()
        {
            if (!IsAlive || Player == null) return;

            var distance = Vector2.Distance(transform.position, Player.position);

            if (distance <= attackRange && !IsAttacking)
                Attack();
            else if (distance <= detectionRange)
                Chase();
            else
                Patrol();
        }

        protected virtual void Patrol()
        {
            if (patrolPoints == null || patrolPoints.Length == 0) return;

            var target = patrolPoints[PatrolIndex];
            MoveTowards(target.position);

            if (Vector2.Distance(transform.position, target.position) < 0.2f)
                PatrolIndex = (PatrolIndex + 1) % patrolPoints.Length;
        }

        protected virtual void Chase()
        {
            MoveTowards(Player.position);
        }

        protected virtual void Attack()
        {
            IsAttacking = true;
            if (attackHitbox != null)
                attackHitbox.Activate(gameObject);
            Invoke(nameof(ResetAttack), 0.5f);
        }

        protected void ResetAttack() => IsAttacking = false;

        protected void MoveTowards(Vector3 target)
        {
            var direction = (target - transform.position).normalized;
            _rb.velocity = new Vector2(direction.x * moveSpeed, _rb.velocity.y);

            var sprite = GetComponent<SpriteRenderer>();
            if (sprite != null && direction.x != 0)
                sprite.flipX = direction.x < 0;
        }

        public virtual void TakeDamage(DamageInfo damage)
        {
            if (!IsAlive) return;

            _currentHealth = Mathf.Max(0f, _currentHealth - damage.Amount);
            RefreshHealthBar();

            if (_currentHealth <= 0f)
                Die();
        }

        protected void RefreshHealthBar()
        {
            _healthBar?.UpdateBar(_currentHealth, maxHealth);
        }

        protected virtual void Die()
        {
            _healthBar?.Hide();
            Destroy(gameObject, 0.1f);
        }
    }
}
