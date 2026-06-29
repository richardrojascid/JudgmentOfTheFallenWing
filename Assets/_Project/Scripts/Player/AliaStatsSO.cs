using UnityEngine;

namespace JudgmentOfTheFallenWing.Player
{
    [CreateAssetMenu(fileName = "AliaStats", menuName = "JudgmentOfTheFallenWing/Alia Stats")]
    public class AliaStatsSO : ScriptableObject
    {
        [Header("Movimiento")]
        public float moveSpeed = 8f;
        public float jumpForce = 14f;
        public float dashSpeed = 20f;

        [Header("Combate")]
        public float maxHealth = 100f;
        public float attackDamage = 15f;
        public float attackCooldown = 0.4f;

        [Header("Narrativa")]
        [TextArea] public string characterBio =
            "ALIA, portadora del Ala Caída, debe atravesar un reino fragmentado " +
            "para restaurar el equilibrio entre cielo y tierra.";
    }
}
