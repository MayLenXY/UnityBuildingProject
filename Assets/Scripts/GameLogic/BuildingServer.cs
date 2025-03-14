using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.GameLogic
{
    [System.Serializable]
    public class BuildingData
    {
        public int buildingType;
        public Vector2Int gridPosition;
    }

    [System.Serializable]
    public class SaveData
    {
        public List<BuildingData> buildings = new List<BuildingData>();
    }

    public class BuildingServer : MonoBehaviour
    {
        private static string savePath => Path.Combine(Application.persistentDataPath, "buildings.json");

        public static void SaveBuildings(List<BuildingData> buildings)
        {
            SaveData data = new SaveData { buildings = buildings };
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);
            Debug.Log($"Buildings saved to {savePath}");
        }

        public static List<BuildingData> LoadBuildings()
        {
            if (!File.Exists(savePath))
            {
                Debug.Log("No save file found.");
                return new List<BuildingData>();
            }

            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log($"Loaded {data.buildings.Count} buildings.");
            return data.buildings;
        }
    }
}
