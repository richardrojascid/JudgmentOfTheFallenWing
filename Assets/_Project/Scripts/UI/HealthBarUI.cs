using JudgmentOfTheFallenWing.Core.Events;
using UnityEngine;
using UnityEngine.UI;

namespace JudgmentOfTheFallenWing.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;

        private void OnEnable() => GameEvents.OnPlayerHealthChanged += UpdateHealth;
        private void OnDisable() => GameEvents.OnPlayerHealthChanged -= UpdateHealth;

        private void UpdateHealth(int current, int max)
        {
            if (healthSlider == null) return;
            healthSlider.maxValue = max;
            healthSlider.value = current;
        }
    }
}
