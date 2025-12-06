using System;
using IKhom.grid_systems.Runtime.components;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IKhom.grid_systems.Runtime.core
{
    public class GridSystem<T>
    {
        private readonly float _cellSize;
        private readonly T[,] _gridObjects;

        public int Height { get; }
        public int Width { get; }

        public GridSystem(int width, int height, Func<GridSystem<T>, GridPosition, T> factory, float cellSize = 1f)
        {
            Width = width;
            Height = height;
            _cellSize = cellSize;

            _gridObjects = new T[Width, Height];

            for (var x = 0; x < Width; x++)
            {
                for (var z = 0; z < Height; z++)
                {
                    var gridPosition = new GridPosition(x, z);
                    _gridObjects[x, z] = factory(this, gridPosition);
                }
            }
        }

        public Vector3 GetWorldPosition(GridPosition gridPos) =>
            new Vector3(gridPos.X, 0, gridPos.Z) * _cellSize;

        public GridPosition GetGridPosition(Vector3 worldPosition) =>
            new(
                Mathf.RoundToInt(worldPosition.x / _cellSize),
                Mathf.RoundToInt(worldPosition.z / _cellSize)
            );

        public void CreateDebugObjects(Transform prefab, Transform parent)
        {
            for (var x = 0; x < Width; x++)
            {
                for (var z = 0; z < Height; z++)
                {
                    var gridPosition = new GridPosition(x, z);

                    var debugObj = Object.Instantiate(prefab, GetWorldPosition(gridPosition), Quaternion.identity,
                        parent);

                    debugObj.GetComponent<GridDebugObject>().SetGridObject(GetGridObject(gridPosition));
                }
            }
        }

        public T GetGridObject(GridPosition gridPos) =>
            _gridObjects[gridPos.X, gridPos.Z];

        public bool IsValidGridPosition(GridPosition gridPos) =>
            gridPos.X >= 0
            && gridPos.X < Width
            && gridPos.Z >= 0
            && gridPos.Z < Height;
    }
}