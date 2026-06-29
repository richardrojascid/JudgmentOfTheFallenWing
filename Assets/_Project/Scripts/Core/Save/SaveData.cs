using System;
using System.Collections.Generic;

namespace JudgmentOfTheFallenWing.Core.Save
{
    [Serializable]
    public class SaveData
    {
        public string lastCheckpointId = "start";
        public string currentScene = "MainMenu";
        public float playerHealth = 100f;
        public float playerMaxHealth = 100f;
        public int currency;
        public List<string> unlockedAbilities = new();
        public List<string> defeatedBosses = new();
        public List<string> openedDoors = new();
        public List<string> collectedItems = new();
        public List<string> visitedRooms = new();

        public bool HasAbility(string abilityId) =>
            unlockedAbilities.Contains(abilityId);

        public void UnlockAbility(string abilityId)
        {
            if (!unlockedAbilities.Contains(abilityId))
                unlockedAbilities.Add(abilityId);
        }

        public bool HasVisitedRoom(string roomId) =>
            visitedRooms.Contains(roomId);

        public void MarkRoomVisited(string roomId)
        {
            if (!visitedRooms.Contains(roomId))
                visitedRooms.Add(roomId);
        }
    }
}
