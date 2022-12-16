using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseSetup : MonoBehaviour
{

    public float targetRoomCount = 4;

    public HouseTemplate testHouseTemplate;

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
        var house = GetComponent<HouseConstructor>().HouseGen(testHouseTemplate);

        while (house.floors[0].rooms.Count != testHouseTemplate.rooms.Count + 1)
        {
            house = GetComponent<HouseConstructor>().HouseGen(testHouseTemplate);
        }

        return;
    }
}
