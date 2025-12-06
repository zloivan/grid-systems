using System;
using _Game.Scripts.Services.Grid.Base;
using _Game.Scripts.Services.Grid.PathFinding;
using _Game.Scripts.Units;
using _Game.Scripts.Utils;
using UnityEngine;

namespace _Game.Scripts.Services.Grid.LevelGrid
{
    public class LevelGrid : SingletonBehaviour<LevelGrid>
    {
        private const float CELL_SIZE = 2f;
        public event Action OnUnitMoved = delegate { };

        [SerializeField] private int _width = 10;
        [SerializeField] private int _height = 10;

        private GridSystem<GridObject> _gridSystem;

        protected override void Awake()
        {
            base.Awake();

            _gridSystem = new GridSystem<GridObject>(
                _width,
                _height,
                (grid, pos) => new GridObject(grid, pos),
                CELL_SIZE);
        }

        private void Start()
        {
            PathFindingManager.Instance.Initialize(_width, _height, CELL_SIZE);
        }

        public int GetHeight() =>
            _gridSystem.Height;

        public int GetWidth() =>
            _gridSystem.Width;

        public void AddUnitAtGridPosition(GridPosition gridPos, Unit unit) =>
            _gridSystem.GetGridObject(gridPos).AddUnit(unit);

        public void RemoveUnitFromGridPosition(GridPosition gridPos, Unit unit) =>
            _gridSystem.GetGridObject(gridPos).RemoveUnit(unit);

        public void UnitMovedToGridPosition(Unit unit, GridPosition oldGridPos, GridPosition newGridPos)
        {
            RemoveUnitFromGridPosition(oldGridPos, unit);
            AddUnitAtGridPosition(newGridPos, unit);

            OnUnitMoved?.Invoke();
        }

        public GridPosition GetGridPosition(Vector3 worldPosition) =>
            _gridSystem.GetGridPosition(worldPosition);

        public bool IsValidGridPosition(GridPosition gridPosition) =>
            _gridSystem.IsValidGridPosition(gridPosition);

        public bool HasAUnitAtGridPosition(GridPosition gridPosition) =>
            _gridSystem.GetGridObject(gridPosition).HasAnyUnit();

        public Vector3 GetWorldPosition(GridPosition gridPosition) =>
            _gridSystem.GetWorldPosition(gridPosition);

        public Unit GetUnitAtGridPosition(GridPosition gridPosition) =>
            _gridSystem.GetGridObject(gridPosition).GetUnit();
    }
}