using System.Collections.Generic;
using JudgmentOfTheFallenWing.Core.Events;
using JudgmentOfTheFallenWing.Core.Managers;
using JudgmentOfTheFallenWing.Core.Save;
using UnityEngine;

namespace JudgmentOfTheFallenWing.Abilities
{
    public class AriaAbilityController : MonoBehaviour, ISaveable
    {
        private readonly HashSet<AbilityType> _unlocked = new();

        public bool HasAbility(AbilityType ability) => _unlocked.Contains(ability);

        private void Start()
        {
            if (GameManager.Instance != null)
                RestoreState(GameManager.Instance.CurrentSave);
        }

        public void UnlockAbility(AbilityType ability)
        {
            if (_unlocked.Add(ability))
            {
                GameEvents.RaiseAbilityUnlocked(ability.ToString());
                Debug.Log($"[ARIA] Habilidad desbloqueada: {ability}");
            }
        }

        public void CaptureState(SaveData data)
        {
            data.unlockedAbilities.Clear();
            foreach (var ability in _unlocked)
                data.unlockedAbilities.Add(ability.ToString());
        }

        public void RestoreState(SaveData data)
        {
            _unlocked.Clear();
            foreach (var abilityName in data.unlockedAbilities)
            {
                if (System.Enum.TryParse(abilityName, out AbilityType ability))
                    _unlocked.Add(ability);
            }
        }
    }
}
