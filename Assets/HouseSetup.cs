using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseSetup : MonoBehaviour
{

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
        int count = 0;

        while (house.floors[0].rooms.Count-1 != testHouseTemplate.rooms.Count)
        {
            print(house.floors[0].rooms.Count - 1 + " : " + testHouseTemplate.rooms.Count);

            house = GetComponent<HouseConstructor>().HouseGen(testHouseTemplate);

            if (count == 100)
            {
                break;
            }
            count++;
        }

        return;
    }
}
