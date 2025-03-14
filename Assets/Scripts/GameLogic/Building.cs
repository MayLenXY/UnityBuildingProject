using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameLogic
{
    public class Building : MonoBehaviour
    {
        private Vector2Int gridPosistion;

        public void SetGridPosition(Vector2Int position, Vector3 worldPosition)
        {
            gridPosistion = position;
            transform.position = worldPosition;
        }

        public float GetHeight()
        {
            return GetComponent<Renderer>().bounds.size.y;
        }

        public int BuildingTypeIndex { get; set; } // ����� ���� ������

        public Vector2Int GetGridPosition()
        {
            return gridPosistion; // �������, ��� ���������� gridPosistion ���������� � ����������� ���������
        }

        public void Initialize(int typeIndex, Vector2Int position, Vector3 worldPosition)
        {
            BuildingTypeIndex = typeIndex;
            SetGridPosition(position, worldPosition);
        }

    }
}
