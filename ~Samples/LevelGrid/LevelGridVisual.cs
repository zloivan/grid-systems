using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Services.Grid.Base;
using _Game.Scripts.Units;
using _Game.Scripts.Units.Actions;
using _Game.Scripts.Utils;
using UnityEngine;

namespace _Game.Scripts.Services.Grid.LevelGrid
{
    public class LevelGridVisual : SingletonBehaviour<LevelGridVisual>
    {
        private enum GridVisualType
        {
            Red = 0,
            SoftRed = 5,
            White = 1,
            Yellow = 2,
            Green = 3,
            Blue = 4,
        }

        [Serializable]
        private struct GridVisualSetting
        {
            public GridVisualType GridVisualType;
            public Material Material;
        }

        [SerializeField] private LevelGridVisualSingle _visualPrefab;
        [SerializeField] private List<GridVisualSetting> _visualSettingsList = new();

        private LevelGridVisualSingle[,] _levelGridVisuals;

        private UnitActionSystem _unitActionSystem;
        private LevelGrid _levelGrid;
        private BaseUnitAction _selectedAction;
        private GridPosition _selectedGridPosition;

        private void Start()
        {
            _unitActionSystem = UnitActionSystem.Instance;
            _levelGrid = LevelGrid.Instance;

            _unitActionSystem.OnSelectedActionChanged += UnitActionSystem_HandleSelectedActionChanged;
            _unitActionSystem.OnSelectedUnitChanged += UnitActionSystem_HandleSelectedUnitChanged;
            _levelGrid.OnUnitMoved += LevelGrid_HandleUnitMoved;

            // Initialize with an invalid position to avoid null reference
            _selectedGridPosition = new GridPosition(-1, -1);

            InstantiateVisualSingles();
            UpdateVisuals();
        }

        private void Update()
        {
            var worldMousePosition = MouseWorld.GetMousePositionInWorld();
            var newSelected = _levelGrid.GetGridPosition(worldMousePosition);
            var isValid = _levelGrid.IsValidGridPosition(newSelected);
            if (newSelected == _selectedGridPosition || !isValid)
                return;

            if (_levelGrid.IsValidGridPosition(_selectedGridPosition))
            {
                _levelGridVisuals[_selectedGridPosition.X, _selectedGridPosition.Z].RemoveHighlight();
            }

            _levelGridVisuals[newSelected.X, newSelected.Z].Highlight();
            _selectedGridPosition = newSelected;
        }

        private void LevelGrid_HandleUnitMoved() =>
            UpdateVisuals();

        private void UnitActionSystem_HandleSelectedUnitChanged(object sender, EventArgs e) =>
            UpdateVisuals();

        private void UnitActionSystem_HandleSelectedActionChanged(object sender, EventArgs e) =>
            UpdateVisuals();

        private void InstantiateVisualSingles()
        {
            _levelGridVisuals = new LevelGridVisualSingle[_levelGrid.GetWidth(), _levelGrid.GetHeight()];

            for (var x = 0; x < _levelGrid.GetWidth(); x++)
            {
                for (var z = 0; z < _levelGrid.GetHeight(); z++)
                {
                    var gridVisual = Instantiate(_visualPrefab, transform);
                    var targetGridPos = new GridPosition(x, z);
                    gridVisual.transform.position = _levelGrid.GetWorldPosition(targetGridPos);

                    gridVisual.Hide();
                    _levelGridVisuals[x, z] = gridVisual;
                }
            }
        }

        private void UpdateVisuals()
        {
            HideAllVisuals();

            _selectedAction = _unitActionSystem.SelectedAction;
            var selectedUnit = _unitActionSystem.SelectedUnit;

            var validPositions = _selectedAction.GetValidActionGridPositionList();

            GridVisualType gridVisualType;
            switch (_selectedAction)
            {
                case SpinUnitAction:
                    gridVisualType = GridVisualType.Yellow;
                    break;
                case MoveUnitAction:
                    gridVisualType = GridVisualType.White;
                    break;
                case ShootAction shootAction:
                    gridVisualType = GridVisualType.Red;
                    ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetMaxShootDistance(),
                        GridVisualType.SoftRed);
                    break;
                case SwordAction swardAction:
                    gridVisualType = GridVisualType.Red;
                    ShowGridPositionRangeSquare(selectedUnit.GetGridPosition(), 1,
                        GridVisualType.SoftRed);
                    break;
                case GrenadeAction grenadeAction: //TODO: Add visual for grenade action with blow range
                    gridVisualType = GridVisualType.Yellow;
                   
                    break;
                default:
                    gridVisualType = GridVisualType.White;
                    break;
            }

            ShowGridVisualList(validPositions, gridVisualType);
        }

        private void ShowGridPositionRange(GridPosition gripPos, int range, GridVisualType visualType)
        {
            var rangeGridPositions = new List<GridPosition>();

            for (var x = -range; x <= range; x++)
            {
                for (var z = -range; z <= range; z++)
                {
                    var offsetGridPos = new GridPosition(x, z);
                    var validGridPos = offsetGridPos + gripPos;

                    if (validGridPos == gripPos)
                    {
                        // Skip the center position, as it is the current position of the unit
                        continue;
                    }
                    
                    if (!_levelGrid.IsValidGridPosition(validGridPos))
                    {
                        // Skip invalid grid positions (out of grid bounds)
                        continue;
                    }

                    var distance = Mathf.Abs(x) + Mathf.Abs(z);
                    if (distance > range)
                    {
                        // Skip positions that are outside the range (Manhattan distance)
                        continue;
                    }

                    rangeGridPositions.Add(validGridPos);
                }
            }

            ShowGridVisualList(rangeGridPositions, visualType);
        }
        
        private void ShowGridPositionRangeSquare(GridPosition gripPos, int range, GridVisualType visualType)
        {
            var rangeGridPositions = new List<GridPosition>();

            for (var x = -range; x <= range; x++)
            {
                for (var z = -range; z <= range; z++)
                {
                    var offsetGridPos = new GridPosition(x, z);
                    var validGridPos = offsetGridPos + gripPos;

                    if (!_levelGrid.IsValidGridPosition(validGridPos))
                        continue;

                    rangeGridPositions.Add(validGridPos);
                }
            }

            ShowGridVisualList(rangeGridPositions, visualType);
        }

        private void ShowGridVisualList(List<GridPosition> validPositions, GridVisualType gridVisualType)
        {
            foreach (var gridPosition in validPositions)
            {
                _levelGridVisuals[gridPosition.X, gridPosition.Z].Show(GetMaterialForGridVisual(gridVisualType));
            }
        }

        private void HideAllVisuals()
        {
            foreach (var child in _levelGridVisuals)
            {
                child.Hide();
            }
        }


        private Material GetMaterialForGridVisual(GridVisualType visualType)
        {
            var materialForGridPosition = _visualSettingsList
                .Where(visualSetting => visualSetting.GridVisualType == visualType)
                .Select(visualSetting => visualSetting.Material).FirstOrDefault();

            Debug.Assert(materialForGridPosition, $"Material for grid position type {visualType} not found!", this);
            return materialForGridPosition;
        }
    }
}