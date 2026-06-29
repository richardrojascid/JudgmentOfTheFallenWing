using JudgmentOfTheFallenWing.Core.Events;
using JudgmentOfTheFallenWing.Core.Managers;
using UnityEngine;

namespace JudgmentOfTheFallenWing.World
{
    public class RoomTrigger : MonoBehaviour
    {
        [SerializeField] private string roomId;
        [SerializeField] private BoxCollider2D cameraBounds;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            GameEvents.RaiseRoomEntered(roomId);

            if (GameManager.Instance != null)
                GameManager.Instance.CurrentSave.MarkRoomVisited(roomId);

            if (cameraBounds != null)
            {
                var cameraFollow = Camera.main?.GetComponent<JudgmentOfTheFallenWing.Camera.CameraFollow2D>();
                cameraFollow?.SetBounds(cameraBounds);
            }
        }
    }
}
