using UnityEngine;

namespace JudgmentOfTheFallenWing.Enemies
{
    /// <summary>
    /// Conecta Enemy Damage (u otro script) con la barra de vida.
    /// Añadir si el daño lo maneja un script separado que no llama a EnemyHealth.
    /// </summary>
    [RequireComponent(typeof(EnemyHealthBar))]
    public class EnemyHealthBarSync : MonoBehaviour
    {
        [SerializeField] private EnemyHealth enemyHealth;
        [SerializeField] private MonoBehaviour damageScript;

        private EnemyHealthBar _healthBar;

        private void Awake()
        {
            _healthBar = GetComponent<EnemyHealthBar>();
            enemyHealth ??= GetComponent<EnemyHealth>();
        }

        private void Start()
        {
            SyncBar();
        }

        /// <summary>
        /// Llamar desde Enemy Damage después de aplicar daño.
        /// </summary>
        public void SyncBar()
        {
            if (enemyHealth != null)
                _healthBar.UpdateBar(enemyHealth.CurrentHealth, enemyHealth.MaxHealth);
        }

        public void ReportDamage(float currentHealth, float maxHealth)
        {
            _healthBar.UpdateBar(currentHealth, maxHealth);
        }
    }
}
