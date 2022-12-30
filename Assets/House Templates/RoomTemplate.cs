using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Room", menuName = "House Templates/Room Template")]

public class RoomTemplate : ScriptableObject
{
    public Color wallColor = Color.white;
    public Color floorColor = Color.gray;
}
