using JudgmentOfTheFallenWing.Core.Events;
using UnityEngine;

namespace JudgmentOfTheFallenWing.Player
{
    /// <summary>
    /// Gestiona la muerte y respawn de ALIA en el último checkpoint.
    /// </summary>
    public class AliaRespawn : MonoBehaviour
    {
        [SerializeField] private float respawnDelay = 1.5f;

        private AliaHealth _health;
        private Vector3 _spawnPosition;
        private bool _isDead;

        private void Awake()
        {
            _health = GetComponent<AliaHealth>();
            _spawnPosition = transform.position;
        }

        private void OnEnable() => GameEvents.OnPlayerDied += HandleDeath;
        private void OnDisable() => GameEvents.OnPlayerDied -= HandleDeath;

        public void SetSpawnPoint(Vector3 position) => _spawnPosition = position;

        private void HandleDeath()
        {
            if (_isDead) return;
            _isDead = true;
            Invoke(nameof(Respawn), respawnDelay);
        }

        private void Respawn()
        {
            _isDead = false;
            transform.position = _spawnPosition;
            _health.Heal(_health.MaxHealth);
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}
