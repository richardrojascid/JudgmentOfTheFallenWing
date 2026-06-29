using JudgmentOfTheFallenWing.Abilities;
using JudgmentOfTheFallenWing.Core.Managers;
using UnityEngine;

namespace JudgmentOfTheFallenWing.World
{
    public class AbilityPickup : MonoBehaviour
    {
        [SerializeField] private AbilityType abilityToGrant;
        [SerializeField] private string pickupId;
        [SerializeField] private GameObject collectEffect;

        private void Start()
        {
            if (GameManager.Instance != null &&
                GameManager.Instance.CurrentSave.collectedItems.Contains(pickupId))
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            var abilities = other.GetComponent<AriaAbilityController>();
            if (abilities == null) return;

            abilities.UnlockAbility(abilityToGrant);

            if (GameManager.Instance != null &&
                !GameManager.Instance.CurrentSave.collectedItems.Contains(pickupId))
            {
                GameManager.Instance.CurrentSave.collectedItems.Add(pickupId);
                GameManager.Instance.SaveGame();
            }

            if (collectEffect != null)
                Instantiate(collectEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
