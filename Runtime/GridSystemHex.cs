using System;
using System.Collections.Generic;
using IKhom.GridSystems.Runtime.core;
using UnityEngine;

namespace IKhom.GridSystems.Runtime
{
    public class GridSystemHex<TGridObject> : GridSystemBase<TGridObject>
    {
        const float HEX_SLOT_HEIGHT_OFFSET_MULTIPLIER = 0.75f;

        public GridSystemHex(int width, int height,
            Func<GridSystemBase<TGridObject>, GridPosition, TGridObject> gridObjectBuilder, float slotSize = 1f)
            : base(width, height, slotSize)
        {
            for (var x = 0; x < GetWidth(); x++)
            {
                for (var z = 0; z < GetHeight(); z++)
                {
                    var gridPosition = new GridPosition(x, z);
                    _gridObjects[x, z] = gridObjectBuilder(this, gridPosition);
                }
            }
        }

        public override GridPosition GetGridPosition(Vector3 worldPosition)
        {
            var approxGridPos = new GridPosition(Mathf.RoundToInt(worldPosition.x / GetCellSize()),
                Mathf.RoundToInt(worldPosition.z / GetCellSize() / HEX_SLOT_HEIGHT_OFFSET_MULTIPLIER));

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

        public override Vector3 GetWorldPosition(GridPosition gridPosition) =>
            new Vector3(gridPosition.X, 0, 0) * GetCellSize() +
            GetCellSize() * HEX_SLOT_HEIGHT_OFFSET_MULTIPLIER * new Vector3(0, 0, gridPosition.Z)
            + (gridPosition.Z % 2 == 1 ? GetCellSize() * 0.5f * new Vector3(1, 0, 0) : Vector3.zero);

        public override bool IsValidGridPosition(GridPosition gridPosition) =>
            gridPosition is { X: >= 0, Z: >= 0 } &&
            gridPosition.X < GetWidth() && gridPosition.Z < GetHeight();

        public override List<GridPosition> GetNeighbors(GridPosition pos)
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
    }
}