using JudgmentOfTheFallenWing.Combat;
using UnityEngine;

namespace JudgmentOfTheFallenWing.Player
{
    public class AliaCombat : MonoBehaviour
    {
        [SerializeField] private Hitbox attackHitbox;
        [SerializeField] private Transform attackPoint;
        [SerializeField] private float attackCooldown = 0.4f;
        [SerializeField] private Animator animator;
        [SerializeField] private string attackTriggerName = "Attack";

        private float _cooldownTimer;
        private int _facingDirection = 1;

        public bool CanAttack => _cooldownTimer <= 0f;

        public void SetFacingDirection(int direction)
        {
            _facingDirection = direction >= 0 ? 1 : -1;
        }

        private void Update()
        {
            if (_cooldownTimer > 0f)
                _cooldownTimer -= Time.deltaTime;
        }

        public void TryAttack()
        {
            if (!CanAttack || attackHitbox == null) return;

            _cooldownTimer = attackCooldown;

            if (attackPoint != null)
            {
                var localPos = attackPoint.localPosition;
                localPos.x = Mathf.Abs(localPos.x) * _facingDirection;
                attackPoint.localPosition = localPos;
            }

            attackHitbox.Activate(gameObject);

            if (animator != null && !string.IsNullOrEmpty(attackTriggerName))
                animator.SetTrigger(attackTriggerName);
        }
    }
}
