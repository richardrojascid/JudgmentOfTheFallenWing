using UnityEngine;

namespace JudgmentOfTheFallenWing.Camera
{
    public class CameraFollow2D : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float smoothSpeed = 8f;
        [SerializeField] private Vector3 offset = new(0f, 1f, -10f);
        [SerializeField] private bool useBounds;
        [SerializeField] private float minX, maxX, minY, maxY;

        private BoxCollider2D _currentBounds;

        public void SetTarget(Transform newTarget) => target = newTarget;

        public void SetBounds(BoxCollider2D bounds)
        {
            _currentBounds = bounds;
            useBounds = bounds != null;

            if (bounds != null)
            {
                var center = bounds.bounds.center;
                var size = bounds.bounds.size;
                minX = center.x - size.x * 0.5f;
                maxX = center.x + size.x * 0.5f;
                minY = center.y - size.y * 0.5f;
                maxY = center.y + size.y * 0.5f;
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            var desiredPosition = target.position + offset;

            if (useBounds)
            {
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
                desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
            }

            var smoothed = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothed;
        }
    }
}
