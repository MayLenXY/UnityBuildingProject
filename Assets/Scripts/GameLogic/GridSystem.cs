using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameLogic
{
    public class GridSystem : MonoBehaviour
    {
        private Dictionary<Vector2Int, Building> placedBuildings = new Dictionary<Vector2Int, Building>();

        public bool IsCellOccupied(Vector2Int gridPos)
        {
            return placedBuildings.ContainsKey(gridPos);
        }

        public void OccupyCell(Vector2Int gridPos, Building building)
        {
            if (!placedBuildings.ContainsKey(gridPos))
            {
                placedBuildings[gridPos] = building;
            }
        }

        public Building GetBuildingAt(Vector2Int gridPos)
        {
            placedBuildings.TryGetValue(gridPos, out Building building);
            return building;
        }

        public void FreeCell(Vector2Int gridPos)
        {
            if (placedBuildings.ContainsKey(gridPos))
            {
                placedBuildings.Remove(gridPos);
            }
        }

        [SerializeField] private int width = 10;
        [SerializeField] private int height = 10;
        [SerializeField] private float cellSize = 1f;

        public Vector3 GetCellCenter(int x, int y)
        {
            return new Vector3(x * cellSize + cellSize / 2, 0, y * cellSize + cellSize / 2);
        }

        public Vector2Int GetGridPosition(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x / cellSize);
            int y = Mathf.FloorToInt(worldPosition.z / cellSize);
            return new Vector2Int(x, y);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 center = GetCellCenter(x, y);
                    Gizmos.DrawWireCube(center, new Vector3(cellSize, 0.1f, cellSize));
                }
            }
        }

        public List<Building> GetAllBuildings()
        {
            List<Building> buildings = new List<Building>();

            foreach (var cell in placedBuildings.Values)
            {
                buildings.Add(cell);
            }

            return buildings;
        }

    }
}
