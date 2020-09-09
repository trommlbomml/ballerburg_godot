using Godot;
using System.Linq;
using System.Collections.Generic;
using Ballerburg.castle.buildings;
using System;

public class Castle : Spatial
{
    [Export] public PackedScene FarmerHouseScene { get; set; }

    private Spatial _buildingsParent; 
    private Spatial _weaponsParent;
    
    private CameraController _cameraController;
    private Action _onPlacementFinished;
    private IBuilding _currentlyPlacingBuilding;
    private Vector3 _rayIntersectionCoords;
    private bool[] _cells = new bool[CellsX * CellsZ];

    public const int CellsX = 34;
    public const int CellsZ = 24;

    public List<IWeapon> Weapons { get; private set; }
    public List<IBuilding> Buildings { get; private set; }
    
    public override void _Ready()
    {
        _buildingsParent = GetNode<Spatial>("buildings");
        _weaponsParent = GetNode<Spatial>("weapons");

        Weapons = _weaponsParent.GetChildren().OfType<IWeapon>().ToList();
        
        Buildings = new List<IBuilding>();
        foreach(var building in _buildingsParent.GetChildren().OfType<IBuilding>())
        {
            building.Place();
            AddBuilding(building);
        }
    }

    public override void _Process(float delta)
    {
        if (_currentlyPlacingBuilding == null) return;

        if (Input.IsActionJustPressed("cancel_build"))
        {
            _buildingsParent.RemoveChild((Node)_currentlyPlacingBuilding);
            _onPlacementFinished?.Invoke();   
        }

        if (Input.IsActionJustPressed("accept_build"))
        {
            AddBuilding(_currentlyPlacingBuilding);
            _currentlyPlacingBuilding = null;
            _onPlacementFinished?.Invoke();
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_currentlyPlacingBuilding == null) return;
        var spaceState = GetWorld().DirectSpaceState;
        var intersection = spaceState.IntersectRay(
            _cameraController.GetPickingRayPosition(), 
            _cameraController.GetPickingRayEnd(50), 
            null, 
            1 << 4);

        if (intersection.Count > 0)
        {
            var localPosition = (Vector3)intersection["position"] - GlobalTransform.origin;

            _currentlyPlacingBuilding.MoveTo(new Vector3(Mathf.Floor(localPosition.x), 0.0f, Mathf.Floor(localPosition.z)));
        }
    }

    public Vector3 GetCenter()
    {
        return GlobalTransform.origin + new Vector3(CellsX * 0.5f, 0.0f, CellsZ * 0.5f);
    }

    public void PlaceNewBuilding(BuildingType buildingType, CameraController controller, Action finished)
    {
        _cameraController = controller;
        _onPlacementFinished = finished;
        _currentlyPlacingBuilding = (IBuilding)FarmerHouseScene.Instance();
        _buildingsParent.AddChild((Node)_currentlyPlacingBuilding);
    }

    private void AddBuilding(IBuilding building)
    {
        Buildings.Add(building);

        for(var x = 0; x < building.Width; x++)
        {
            for(var z = 0; z < building.Height; z++)
            {
                _cells[(building.CastleZ + z)*CellsX + x + building.CastleX] = true;
            }
        }
    }

    private bool CanPlace(IBuilding building)
    {
        for(var x = 0; x < building.Width; x++)
        {
            for(var z = 0; z < building.Height; z++)
            {
                if (_cells[(building.CastleZ + z)*CellsX + x + building.CastleX]) return false;
            }
        }

        return true;
    }
}
