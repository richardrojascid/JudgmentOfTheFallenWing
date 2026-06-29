using JudgmentOfTheFallenWing.Combat;
using UnityEngine;

namespace JudgmentOfTheFallenWing.Combat
{
    [RequireComponent(typeof(Collider2D))]
    public class Hitbox : MonoBehaviour
    {
        [SerializeField] private float damage = 10f;
        [SerializeField] private Vector2 knockback = new(3f, 2f);
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private float activeDuration = 0.15f;

        private bool _isActive;
        private GameObject _owner;

        public void Activate(GameObject owner)
        {
            _owner = owner;
            _isActive = true;
            CancelInvoke(nameof(Deactivate));
            Invoke(nameof(Deactivate), activeDuration);
        }

        public void Deactivate() => _isActive = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isActive) return;
            if (((1 << other.gameObject.layer) & targetLayers) == 0) return;
            if (other.gameObject == _owner) return;

            if (other.TryGetComponent<IDamageable>(out var damageable) && damageable.IsAlive)
            {
                var direction = (other.transform.position - transform.position).normalized;
                var knockbackVector = new Vector2(direction.x * knockback.x, knockback.y);
                damageable.TakeDamage(new DamageInfo(damage, knockbackVector, _owner));
            }
        }
    }
}
