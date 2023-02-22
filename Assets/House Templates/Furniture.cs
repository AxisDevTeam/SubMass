using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Furniture", menuName = "House Templates/Furniture")]

public class Furniture : ScriptableObject
{
    public GameObject furniture;
    public Vector2 size;
    public RoomPlacement roomPlacement;
}

public enum RoomPlacement
{
    corner,
    center,
    wall_random
}