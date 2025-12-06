using System;
using System.Collections.Generic;
using IKhom.GridSystems.Runtime.components;
using IKhom.GridSystems.Runtime.core;
using UnityEngine;

namespace HexSort.Grid
{
    public class GridSystemHex<TGridObject>
    {
        const float HEX_SLOT_HEIGHT_OFFSET_MULTIPLIER = 0.75f;
        private readonly int _width;
        private readonly int _height;
        private readonly float _slotSize;
        private readonly TGridObject[,] _gridObjectArray;

        public GridSystemHex(int width, int height, float slotSize,
            Func<GridSystemHex<TGridObject>, GridPosition, TGridObject> gridObjectBuilder)
        {
            _gridObjectArray = new TGridObject[width, height];
            _slotSize = slotSize;
            _width = width;
            _height = height;


            for (var x = 0; x < _width; x++)
            {
                for (var z = 0; z < _height; z++)
                {
                    var gridPosition = new GridPosition(x, z);
                    _gridObjectArray[x, z] = gridObjectBuilder(this, gridPosition);
                }
            }
        }

        public int GetHeight() =>
            _height;

        public int GetWidth() =>
            _width;

        public GridPosition GetGridPosition(Vector3 worldPosition)
        {
            var approxGridPos = new GridPosition(Mathf.RoundToInt(worldPosition.x / _slotSize),
                Mathf.RoundToInt(worldPosition.z / _slotSize / HEX_SLOT_HEIGHT_OFFSET_MULTIPLIER));

            var neighbors = GetNeighbors(approxGridPos);

            var minDistance = float.MaxValue;
            var newGridPosition = approxGridPos;

            if (IsValidGridPosition(approxGridPos))
            {
                var approxWorldPos = GetWorldPosition(approxGridPos);
                minDistance = Vector3.Distance(worldPosition, approxWorldPos);
            }

            foreach (var neighborGridPosition in neighbors)
            {
                if (!IsValidGridPosition(neighborGridPosition))
                {
                    continue;
                }

                var neighborWorldPos = GetWorldPosition(neighborGridPosition);
                var distance = Vector3.Distance(worldPosition, neighborWorldPos);

                if (minDistance <= distance)
                    continue;

                newGridPosition = neighborGridPosition;
                minDistance = distance;
            }

            return newGridPosition;
        }

        public Vector3 GetWorldPosition(GridPosition gridPosition) =>
            new Vector3(gridPosition.X, 0, 0) * _slotSize +
            _slotSize * HEX_SLOT_HEIGHT_OFFSET_MULTIPLIER * new Vector3(0, 0, gridPosition.Z)
            + (gridPosition.Z % 2 == 1 ? _slotSize * 0.5f * new Vector3(1, 0, 0) : Vector3.zero);

        public TGridObject GetGridObject(GridPosition gridPos) =>
            _gridObjectArray[gridPos.X, gridPos.Z];

        public bool IsValidGridPosition(GridPosition gridPosition) =>
            gridPosition is { X: >= 0, Z: >= 0 } &&
            gridPosition.X < _width && gridPosition.Z < _height;

        public List<GridPosition> GetNeighbors(GridPosition pos)
        {
            var isAddRow = pos.Z % 2 == 1;

            return new List<GridPosition>
            {
                pos + new GridPosition(-1, 0), //left
                pos + new GridPosition(+1, 0), //right
                pos + new GridPosition(0, +1), //top-left
                pos + new GridPosition(0, -1), //bot-left
                pos + new GridPosition(isAddRow ? +1 : -1, +1), //top-right
                pos + new GridPosition(isAddRow ? +1 : -1, -1), //bot-right
            };
        }

        public void CreateDebugObjects(Transform prefab, Transform parent)
        {
            for (var x = 0; x < _width; x++)
            {
                for (var z = 0; z < _height; z++)
                {
                    var gridPosition = new GridPosition(x, z);

                    var debugObj = UnityEngine.Object.Instantiate(prefab, GetWorldPosition(gridPosition),
                        Quaternion.identity,
                        parent);

                    debugObj.GetComponent<GridDebugObject>().SetGridObject(GetGridObject(gridPosition));
                }
            }
        }
    }
}