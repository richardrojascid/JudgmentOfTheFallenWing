using JudgmentOfTheFallenWing.Core.Events;
using JudgmentOfTheFallenWing.Core.Managers;
using UnityEngine;

namespace JudgmentOfTheFallenWing.World
{
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] private string checkpointId = "checkpoint_01";
        [SerializeField] private Transform respawnPoint;
        [SerializeField] private GameObject inactiveVisual;
        [SerializeField] private GameObject activeVisual;

        private bool _isActive;

        private void Start()
        {
            if (GameManager.Instance != null &&
                GameManager.Instance.CurrentSave.lastCheckpointId == checkpointId)
            {
                Activate(silent: true);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isActive) return;
            if (!other.CompareTag("Player")) return;
            Activate();
        }

        private void Activate(bool silent = false)
        {
            _isActive = true;

            if (inactiveVisual != null) inactiveVisual.SetActive(false);
            if (activeVisual != null) activeVisual.SetActive(true);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.CurrentSave.lastCheckpointId = checkpointId;
                GameManager.Instance.SaveGame();
            }

            if (!silent)
                GameEvents.RaiseCheckpointActivated(checkpointId);
        }

        public Vector3 GetRespawnPosition()
        {
            return respawnPoint != null ? respawnPoint.position : transform.position;
        }
    }
}
