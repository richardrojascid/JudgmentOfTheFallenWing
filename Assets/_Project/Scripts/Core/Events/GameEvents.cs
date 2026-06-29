using System;
using UnityEngine;

namespace JudgmentOfTheFallenWing.Core.Events
{
    /// <summary>
    /// Eventos globales del juego. Usar EventBus para suscribirse/publicar.
    /// </summary>
    public static class GameEvents
    {
        public static event Action<int, int> OnPlayerHealthChanged;
        public static event Action OnPlayerDied;
        public static event Action<string> OnAbilityUnlocked;
        public static event Action<string> OnCheckpointActivated;
        public static event Action<string> OnRoomEntered;
        public static event Action<int> OnCurrencyChanged;
        public static event Action OnGamePaused;
        public static event Action OnGameResumed;
        public static event Action OnGameSaved;

        public static void RaisePlayerHealthChanged(int current, int max) =>
            OnPlayerHealthChanged?.Invoke(current, max);

        public static void RaisePlayerDied() => OnPlayerDied?.Invoke();

        public static void RaiseAbilityUnlocked(string abilityId) =>
            OnAbilityUnlocked?.Invoke(abilityId);

        public static void RaiseCheckpointActivated(string checkpointId) =>
            OnCheckpointActivated?.Invoke(checkpointId);

        public static void RaiseRoomEntered(string roomId) =>
            OnRoomEntered?.Invoke(roomId);

        public static void RaiseCurrencyChanged(int amount) =>
            OnCurrencyChanged?.Invoke(amount);

        public static void RaiseGamePaused() => OnGamePaused?.Invoke();
        public static void RaiseGameResumed() => OnGameResumed?.Invoke();
        public static void RaiseGameSaved() => OnGameSaved?.Invoke();
    }
}
