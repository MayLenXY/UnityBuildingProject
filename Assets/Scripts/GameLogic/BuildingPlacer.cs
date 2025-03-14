using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.GameLogic
{
    public class BuildingPlacer : MonoBehaviour
    {
        [SerializeField] private GridSystem gridSystem;
        [SerializeField] private List<Building> buildingPrefab;
        private Building currentBuilding;
        private bool isDeleteMode = false; // Флаг удаления
        private int buildingsIndex = 0;
        private Color originalColor; // Переменная для хранения цвета

        private PlayerInput playerInput;
        private InputAction moveAction;
        private InputAction placeBuildingAction;
        private InputAction toggleDeleteModeAction;

        private void Update()
        {
            if (currentBuilding != null)
            {
                Vector3 mouseWorldPos = GetMouseWorldPosition();
                Vector2Int gridPos = gridSystem.GetGridPosition(mouseWorldPos);
                Vector3 worldPos = gridSystem.GetCellCenter(gridPos.x, gridPos.y);

                float buildingHeight = currentBuilding.GetHeight();
                worldPos.y += buildingHeight / 2f;
                currentBuilding.transform.position = worldPos;

                // Меняем цвет в зависимости от того, можно ли построить
                bool canBuild = !gridSystem.IsCellOccupied(gridPos);
                SetBuildingColor(canBuild ? Color.green : Color.red);
            }
        }



        private void SetBuildingColor(Color color)
        {
            Renderer[] renderers = currentBuilding.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                renderer.material.color = color;
            }
        }



        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();

            moveAction = playerInput.actions["Move"];
            placeBuildingAction = playerInput.actions["PlaceBuilding"];
            toggleDeleteModeAction = playerInput.actions["ToggleDeleteMode"];

            placeBuildingAction.performed += ctx => TryPlaceBuilding();
            toggleDeleteModeAction.performed += ctx => ToggleDeleteMode();

            LoadBuildings();
        }

        private void OnApplicationQuit()
        {
            SaveBuildings();
        }

        private void TryPlaceBuilding()
        {
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            Vector2Int gridPos = gridSystem.GetGridPosition(mouseWorldPos);
            Vector3 worldPos = gridSystem.GetCellCenter(gridPos.x, gridPos.y);

            if (isDeleteMode) // Если режим удаления включен
            {
                TryDeleteBuilding(gridPos);
                return;
            }

            if (currentBuilding != null)
            {
                PlaceBuilding(gridPos, worldPos);
            }
        }

        public void StartPlacingBuilding()
        {
            if (currentBuilding != null)
                Destroy(currentBuilding.gameObject);

            currentBuilding = Instantiate(buildingPrefab[buildingsIndex]);
            currentBuilding.BuildingTypeIndex = buildingsIndex; // Устанавливаем индекс типа

            Renderer renderer = currentBuilding.GetComponentInChildren<Renderer>();
            if (renderer != null)
                originalColor = renderer.material.color;

            Vector3 mouseWorldPos = GetMouseWorldPosition();
            Vector2Int gridPos = gridSystem.GetGridPosition(mouseWorldPos);
            Vector3 worldPos = gridSystem.GetCellCenter(gridPos.x, gridPos.y);

            float buildingHeight = currentBuilding.GetHeight();
            worldPos.y += buildingHeight / 2f;
            currentBuilding.SetGridPosition(gridPos, worldPos);

            isDeleteMode = false;
        }

        private void PlaceBuilding(Vector2Int gridPos, Vector3 worldPos)
        {
            if (gridSystem.IsCellOccupied(gridPos)) return;

            worldPos.y = buildingPrefab[buildingsIndex].GetHeight() / 2f;

            currentBuilding.SetGridPosition(gridPos, worldPos);
            gridSystem.OccupyCell(gridPos, currentBuilding);

            SetBuildingColor(originalColor);

            currentBuilding = null;
        }

        private void TryDeleteBuilding(Vector2Int gridPos)
        {
            Building building = gridSystem.GetBuildingAt(gridPos);
            if (building == null) return;

            Destroy(building.gameObject);
            gridSystem.FreeCell(gridPos);
        }

        public void ToggleDeleteMode()
        {
            isDeleteMode = !isDeleteMode; // Переключаем режим
            if (isDeleteMode && currentBuilding != null)
            {
                Destroy(currentBuilding.gameObject); // Если был выбран объект — убираем
                currentBuilding = null;
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            if (groundPlane.Raycast(ray, out float enter))
            {
                return ray.GetPoint(enter);
            }
            return Vector3.zero;
        }

        public void SelectBuilding(int index)
        {
            if (index < 0 || index >= buildingPrefab.Count) return; // Проверка границ списка
            buildingsIndex = index; // Устанавливаем выбранное здание
        }

        private void SaveBuildings()
        {
            List<BuildingData> buildings = new List<BuildingData>();

            foreach (var building in gridSystem.GetAllBuildings())
            {
                Debug.Log($"Сохраняем здание: Тип {building.BuildingTypeIndex}, Позиция {building.GetGridPosition()}");

                buildings.Add(new BuildingData
                {
                    buildingType = building.BuildingTypeIndex,
                    gridPosition = building.GetGridPosition()
                });
            }

            BuildingServer.SaveBuildings(buildings);
        }

        private void LoadBuildings()
        {
            List<BuildingData> buildings = BuildingServer.LoadBuildings();

            foreach (var data in buildings)
            {
                if (data.buildingType < 0 || data.buildingType >= buildingPrefab.Count)
                {
                    Debug.LogWarning($"Неверный индекс здания: {data.buildingType}");
                    continue;
                }

                // Создаём здание по его сохранённому индексу
                Building newBuilding = Instantiate(buildingPrefab[data.buildingType]);
                newBuilding.BuildingTypeIndex = data.buildingType; // Восстанавливаем тип

                Vector3 worldPos = gridSystem.GetCellCenter(data.gridPosition.x, data.gridPosition.y);
                worldPos.y += newBuilding.GetHeight() / 2f;
                newBuilding.SetGridPosition(data.gridPosition, worldPos);

                gridSystem.OccupyCell(data.gridPosition, newBuilding);

                Debug.Log($"Загружено здание: Тип {newBuilding.BuildingTypeIndex}, Позиция {data.gridPosition}");
            }
        }
    }
}
