using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New House", menuName = "House Templates/House Template")]

public class HouseTemplate : ScriptableObject
{
    public List<RoomTemplate> rooms = new List<RoomTemplate>();
}
