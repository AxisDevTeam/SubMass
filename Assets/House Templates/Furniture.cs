using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Furniture", menuName = "House Templates/Furniture")]

public class Furniture : ScriptableObject
{
    public GameObject furniture;
    public int length;
    public int width;

    public List<DynamicFurnitureColorInfo> DynamicColorInfo = new List<DynamicFurnitureColorInfo>();
}

[System.Serializable]
public class DynamicFurnitureColorInfo{
    public int materialSlot;
    public string attributeName;
    public DynamicFurnitureColorSource colorSource;

    public DynamicFurnitureColorInfo()
    {
        this.materialSlot = 0;
        this.attributeName = "_baseColor";
        this.colorSource = DynamicFurnitureColorSource.random;
    }
    public DynamicFurnitureColorInfo(int materialSlot, DynamicFurnitureColorSource colorSource, string attributeName)
    {
        this.materialSlot = materialSlot;
        this.attributeName = attributeName;
        this.colorSource = colorSource;
    }
}

public enum DynamicFurnitureColorSource{
    wall_color,
    floor_color,
    random
}

