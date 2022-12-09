using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseSetup : MonoBehaviour
{

    public float targetRoomCount = 4;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            CreateHouseWithRooms();
        }
    }

    void CreateHouseWithRooms()
    {
        var house = GetComponent<HouseConstructor>().HouseGen();

        while (house.floors[0].rooms.Count != targetRoomCount+1)
        {
            house = GetComponent<HouseConstructor>().HouseGen();
        }

        return;
    }
}
