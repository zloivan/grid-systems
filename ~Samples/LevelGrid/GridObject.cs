using System.Collections.Generic;
using System.Linq;
using IKhom.GridSystems.Runtime;
using IKhom.GridSystems.Runtime.core;
using UnityEngine.UI;

namespace _Game.Scripts.Services.Grid.LevelGrid
{
    public class GridObject
    {
        private readonly GridPosition _gridPosition;
        private readonly GridSystem<GridObject> _gridSystem;
        private readonly List<CanvasScaler.Unit> _unitList;

        public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
        {
            _gridPosition = gridPosition;
            _gridSystem = gridSystem;
            _unitList = new List<CanvasScaler.Unit>();
        }

        public override string ToString() => 
            $"{_gridPosition}\n" + string.Join("\n", _unitList.Select(e => e.name));

        public void AddUnit(CanvasScaler.Unit unit) => _unitList.Add(unit);

        public List<CanvasScaler.Unit> GetUnitList() => _unitList;

        public void RemoveUnit(CanvasScaler.Unit unit)
        {
            if (!_unitList.Contains(unit)) 
                return;
            
            _unitList.Remove(unit);
        }
        
        public void ClearUnits() => _unitList.Clear();
        public bool HasAnyUnit() => _unitList.Count > 0;

        public CanvasScaler.Unit GetUnit() => HasAnyUnit() ? _unitList[0] : null;
    }
}