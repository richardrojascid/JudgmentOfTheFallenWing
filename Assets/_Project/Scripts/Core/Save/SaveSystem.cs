using System.IO;
using UnityEngine;

namespace JudgmentOfTheFallenWing.Core.Save
{
    public static class SaveSystem
    {
        private const string SaveFileName = "alia_save.json";

        private static string SavePath =>
            Path.Combine(Application.persistentDataPath, SaveFileName);

        public static void Save(SaveData data)
        {
            var json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"[SaveSystem] Partida guardada en {SavePath}");
        }

        public static SaveData Load()
        {
            if (!File.Exists(SavePath))
            {
                Debug.Log("[SaveSystem] No hay partida guardada. Creando nueva.");
                return new SaveData();
            }

            var json = File.ReadAllText(SavePath);
            var data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("[SaveSystem] Partida cargada.");
            return data ?? new SaveData();
        }

        public static bool HasSave() => File.Exists(SavePath);

        public static void DeleteSave()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log("[SaveSystem] Partida eliminada.");
            }
        }
    }
}
