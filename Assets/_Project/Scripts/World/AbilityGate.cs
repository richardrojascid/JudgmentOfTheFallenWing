using JudgmentOfTheFallenWing.Abilities;
using JudgmentOfTheFallenWing.Core.Managers;
using UnityEngine;

namespace JudgmentOfTheFallenWing.World
{
    /// <summary>
    /// Puerta o pasaje que requiere una habilidad específica para abrirse.
    /// </summary>
    public class AbilityGate : MonoBehaviour
    {
        [SerializeField] private AbilityType requiredAbility;
        [SerializeField] private string gateId;
        [SerializeField] private GameObject blockingCollider;
        [SerializeField] private GameObject openVisual;
        [SerializeField] private GameObject closedVisual;

        private bool _isOpen;

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                var save = GameManager.Instance.CurrentSave;
                if (save.openedDoors.Contains(gateId))
                    OpenGate(silent: true);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isOpen) return;
            if (!other.CompareTag("Player")) return;

            var abilities = other.GetComponent<AriaAbilityController>();
            if (abilities == null || !abilities.HasAbility(requiredAbility)) return;

            OpenGate();
        }

        private void OpenGate(bool silent = false)
        {
            _isOpen = true;

            if (blockingCollider != null)
                blockingCollider.SetActive(false);

            if (openVisual != null) openVisual.SetActive(true);
            if (closedVisual != null) closedVisual.SetActive(false);

            if (!silent && GameManager.Instance != null)
            {
                if (!GameManager.Instance.CurrentSave.openedDoors.Contains(gateId))
                    GameManager.Instance.CurrentSave.openedDoors.Add(gateId);
            }

            if (!silent)
                Debug.Log($"[AbilityGate] Puerta '{gateId}' abierta con {requiredAbility}");
        }
    }
}
