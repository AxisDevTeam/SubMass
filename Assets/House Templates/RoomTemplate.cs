using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Room", menuName = "House Templates/Room Template")]

public class RoomTemplate : ScriptableObject
{
    public Color wallColor = Color.white;
    public Color floorColor = Color.gray;

    public List<FurnitureInfo> furnitures = new List<FurnitureInfo>();
}

[System.Serializable]
public class FurnitureInfo{
    public Furniture furniture;
    public FurniturePlacementPriority priority;
    public FurniturePlacement placement;

    public FurnitureInfo(Furniture furniture, FurniturePlacement fp, FurniturePlacementPriority priority)
    {
        this.furniture = furniture;
        this.placement = fp;
        this.priority = priority;
    }
    
}

public enum FurniturePlacement
{
    wall,
    corner,
    floor
}

public enum FurniturePlacementPriority
{
    required,
    tryInclude,
    notNeeded
}