using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HouseConstructor : MonoBehaviour
{
    public Floor f;
    public House h;
    public int _currentSplitRoom = 0;
    public int minRoomDimension = 3;

    public int roomSplitRepetitions = 25;

    public int variation;

    public GameObject WallBlock;
    public GameObject CornerBlock;
    public GameObject FloorBlock;
    public GameObject DoorBlock;

    //public GameObject[] BLOCK_TYPES;

    public Dictionary<string,GameObject> BLOCK_TYPES;

    public float scale = 1f;




    // Start is called before the first frame update
    void Awake()
    {
        BLOCK_TYPES = new Dictionary<string, GameObject>();

        BLOCK_TYPES.Add("wall", WallBlock);
        BLOCK_TYPES.Add("corner", CornerBlock);
        BLOCK_TYPES.Add("floor", FloorBlock);
        BLOCK_TYPES.Add("door", DoorBlock);
    }

    public void HouseFurnish(House h, HouseTemplate template)
    {
        foreach(var templateRoom in template.rooms)
        {
            foreach (var f in templateRoom.furniture)
            {
                switch (f.priority)
                {
                    case FurniturePlacementPriority.required:

                        if(f.placement == FurniturePlacement.corner)
                        {

                            // find the room
                            Room room = null;
                            int index = 0;
                            foreach (var r in h.floors[0].rooms)
                            {
                                index++;
                                if (r.template == templateRoom)
                                {
                                    room = r;
                                }
                            }
                            if(room == null)
                            {
                                Debug.LogError("something went VERY wrong cause i cant find the room...");
                            }

                            // get a randomized list of blocks in the room
                            var b = room.blocks.OrderBy(x => Random.value).ToList();

                            for (int i = 0; i < b.Count; i++)
                            {
                                // go through each block in list, place furniture if corner works
                                var block = b[i];

                                if (block.direction.Length == 2)
                                {
                                    // get the location of the block and initiate place if that fBlock is empty
                                    var loc = block.GetBlockLocation(h.floors[0]);
                                    if(h.floors[0].fBlocks[loc[0],loc[1]].isEmpty == true)
                                    {
                                        h.floors[0].PlaceFurniture(f.furniture, room,  loc);
                                        break;
                                    }
                                }
                            }


                        }

                        break;
                }
            }
        }
    }

    //match house with template attributes like color
    public House HouseConfigure(House h, HouseTemplate template)
    {
        h.floors[0].rooms = h.floors[0].rooms.OrderBy(a => a.blocks.Count).ToList();

        for(int i = 0; i < h.floors[0].rooms.Count; i++)
        {
            //var r = h.floors[0].rooms[i];
            //print(i + " : " + h.floors[0].rooms[i].blocks.Count + " : " + template.rooms[i]);
            h.floors[0].rooms[i].template = template.rooms[i];
            h.floors[0].rooms[i].wallColor = template.rooms[i].wallColor;
            h.floors[0].rooms[i].floorColor = template.rooms[i].floorColor;
        }

        return h;
    }

    public House HouseGen(HouseTemplate template)
    {
        var l = template.length;
        var w = template.width;

        if (template.randomSize)
        {
            l = Random.Range(template.lengthRange.x, template.lengthRange.y + 1);
            w = Random.Range(template.widthRange.x, template.widthRange.y + 1);
        }

        h = new House(l, w, BLOCK_TYPES, scale);
        int roomCount = template.rooms.Count;

        void repeatRoomGen()
        {
            var success = false;

            List<int> roomids = new List<int>();
            for (int i = 0; i < h.floors[0].rooms.Count; i++)
            {
                roomids.Add(i);
            }

            //roomids = roomids.OrderBy(a => Random.value).ToList();

            foreach (var r in roomids)
            {
                var id = h.floors[0].GetRoomID(r);


                var choose = h.floors[0].SplitHorizontal(id, minRoomDimension, variation);
                if(choose == false)
                {
                    choose = h.floors[0].SplitVertical(id, minRoomDimension, variation);
                }

                //var choose = (Random.Range(0, 2) == 1) ? h.floors[0].SplitHorizontal(id, minRoomDimension, variation) : 

                if (choose)
                {
                    success = true;
                    break;
                }
            }

        }

        var iterations = 0;
        for (int i = 0; i < roomCount - 1; i++)
        {
            var room = Random.Range(0, h.floors[0].rooms.Count);
            var id = h.floors[0].GetRoomID(room);
            var choose = (Random.Range(0, 2) == 1) ? h.floors[0].SplitHorizontal(id, minRoomDimension, variation) : h.floors[0].SplitVertical(id, minRoomDimension, variation);

            if (choose == false)
            {
                //print("cant find suitable room for split??");
                repeatRoomGen();
            }

            if (iterations >= 100)
            {
                break;
            }
        }

        //take random layout and apply template attributes to it
        h = HouseConfigure(h, template);

        h.BuildHouse();
        //print("Room Count" + h.floors[0].rooms.Count);

        //HouseFurnish(h, template);

        if (h.floors[0].rooms.Count != template.rooms.Count)
        {
            h = HouseGen(template);
            return h;
        }
        else
        {
            HouseFurnish(h, template);
            return h;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

public class House
{
    public List<Floor> floors;
    Dictionary<string, GameObject> BLOCK_TYPES;
    float scale;
    public House(int length, int width, Dictionary<string,GameObject> BLOCK_TYPES, float scale)
    {
        floors = new List<Floor>();
        var f1 = new Floor(length,width);
        floors.Add(f1);
        this.BLOCK_TYPES = BLOCK_TYPES;
        this.scale = scale;
    }

    public void BuildHouse()
    {
        DestroyHouse();
        DestroyFurniture();
        for (int i = 0; i<floors.Count; i++)
        {
            floors[0].Final();
        }

        for (int f = 0; f < floors.Count; f++)
        {
            for (int l = 0; l < floors[f].blocks.GetLength(0); l++)
            {
                for (int w = 0; w < floors[f].blocks.GetLength(1); w++)
                {
                    if (!floors[f].blocks[l, w].type.Equals("null"))
                    {
                        if (!floors[f].blocks[l, w].type.Equals("empty")) {
                            var go = floors[f].blocks[l, w].ConvertBlock(BLOCK_TYPES);


                            var rot = 0;
                            if(floors[f].blocks[l, w].direction == "S" || floors[f].blocks[l, w].direction == "SW")
                            {
                                rot = 180;
                            }
                            else if (floors[f].blocks[l, w].direction == "E" || floors[f].blocks[l, w].direction == "SE")
                            {
                                rot = 90;
                            }
                            else if (floors[f].blocks[l, w].direction == "W" || floors[f].blocks[l, w].direction == "NW")
                            {
                                rot = 270;
                            }

                            var obj = GameObject.Instantiate(go);
                            obj.transform.position = new Vector3(l* scale, f* scale, w* scale);
                            obj.transform.rotation = Quaternion.Euler(new Vector3(-90, 90, 0) + new Vector3(0, rot, 0));
                            obj.transform.localScale = new Vector3(scale, scale, scale);
                            obj.tag = "house";

                            if (floors[f].blocks[l, w].type.Equals("door"))
                            {
                                obj.GetComponent<MeshRenderer>().materials[0].color = Color.white;
                            }
                            else if (floors[f].blocks[l, w].type.Equals("wall"))
                            {
                                obj.GetComponent<MeshRenderer>().materials[0].color = floors[f].GetRoom(floors[f].blocks[l, w].GetRoomID()).wallColor;
                            }
                            else if (floors[f].blocks[l, w].type.Equals("window"))
                            {
                                obj.GetComponent<MeshRenderer>().materials[0].color = new Color(Color.blue.r, Color.blue.g, Color.blue.b, 0.5f);
                            }
                        }

                        var floor = GameObject.Instantiate(BLOCK_TYPES["floor"]);
                        floor.transform.position = new Vector3(l *scale , f * scale, w * scale);
                        floor.transform.rotation = Quaternion.Euler(new Vector3(-90, 90, 0));
                        floor.transform.localScale = new Vector3(scale, scale, scale);
                        floor.tag = "house";


                        floor.GetComponent<MeshRenderer>().materials[0].color = floors[f].GetRoom(floors[f].blocks[l, w].GetRoomID()).floorColor;
                        
                    }
                }
            }
        }
    }

    public void DestroyHouse()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("house");

        for (var i = 0; i < gameObjects.Length; i++)
            GameObject.Destroy(gameObjects[i]);
    }

    public void DestroyFurniture()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("furniture");

        for (var i = 0; i < gameObjects.Length; i++)
            GameObject.Destroy(gameObjects[i]);
    }
}

public class Floor 
{
    public Block[,] blocks;
    public FurnitureBlock[,] fBlocks;
    public List<Room> rooms;
    

    public Floor(int length, int width)
    {
        List<Block> b = new List<Block>();
        // populate blocks list with new blocks of roomID 0
        blocks = new Block[length, width];
        fBlocks = new FurnitureBlock[length, width];

        for (int l = 0; l < fBlocks.GetLength(0); l++)
        {
            for (int w = 0; w < fBlocks.GetLength(1); w++)
            {
                fBlocks[l, w] = new FurnitureBlock();
            }
        }

        rooms = new List<Room>();
        for (int l = 0; l < blocks.GetLength(0); l++)
        {
            for (int w = 0; w < blocks.GetLength(1); w++)
            {
                blocks[l, w] = new Block();
                b.Add(blocks[l, w]);
            }
        }

        // add room 0
        var r = new Room(this);
        r.SetBlocks(b,blocks[0,0],blocks[length-1,width-1]);
        rooms.Add(r);
    }

    public bool PlaceFurniture(Furniture furniture, Room room, int[] location)
    {
        var rot = 0;
        var dir = blocks[location[0], location[1]].direction;
        var scale = 3;
        for(int l = 0; l < furniture.length; l++)
        {
            for (int w = 0; w < furniture.width; w++)
            {
                switch (dir)
                {
                    case ("NE"):
                        if (isValid(location[0] + l, location[1] + w) == false || fBlocks[location[0] + l, location[1] + w].isEmpty == false)
                        {
                            Debug.Log("furniture place failed");
                            return false;
                        }
                        fBlocks[location[0] + l, location[1] + w].SetFurniture(furniture);
                        fBlocks[location[0] + l, location[1] + w].isEmpty = false;
                        break;
                    case ("SE"):
                        if (isValid(location[0] - w, location[1] - l) == false || fBlocks[location[0] - w, location[1] - l].isEmpty == false)
                        {
                            Debug.Log("furniture place failed");
                            return false;
                        }
                        fBlocks[location[0] - w, location[1] - l].SetFurniture(furniture);
                        fBlocks[location[0] - w, location[1] - l].isEmpty = false;

                        rot = 90;
                        break;
                    case ("NW"):
                        if (isValid(location[0] + l, location[1] + w) == false || fBlocks[location[0] + l, location[1] + w].isEmpty == false)
                        {
                            Debug.Log("furniture place failed");
                            return false;
                        }
                        fBlocks[location[0] + l, location[1] + w].SetFurniture(furniture);
                        fBlocks[location[0] + l, location[1] + w].isEmpty = false;
                        rot = 0;
                        break;
                    case ("SW"):
                        if (isValid(location[0] + -l, location[1] + -w) == false || fBlocks[location[0] + -l, location[1] + -w].isEmpty == false)
                        {
                            Debug.Log("furniture place failed");
                            return false;
                        }
                        fBlocks[location[0] + -l, location[1] + -w].SetFurniture(furniture);
                        fBlocks[location[0] + -l, location[1] + -w].isEmpty = false;
                        rot = 180;
                        break;
                }
            }
        }

        var obj = GameObject.Instantiate(furniture.furniture);
        obj.transform.position = new Vector3(location[0], 0, location[1]) * scale;
        obj.transform.rotation = Quaternion.Euler(new Vector3(-90, 90, 0) + new Vector3(0, rot, 0)) ;
        obj.transform.localScale = new Vector3(scale, scale, scale);
        obj.transform.position += (obj.transform.up * 0.4f * scale) + (obj.transform.right * 0.4f * scale);
        obj.tag = "furniture";

        

        ColorFurniture(obj, room, furniture);

        DebugDrawFurnitureBlocks();

        return true;

        
    }

    public void ColorFurniture(GameObject obj, Room room, Furniture fnt)
    {
        foreach(var ci in fnt.DynamicColorInfo)
        {
            int matID = ci.materialSlot;
            string attName = ci.attributeName;

            switch(ci.colorSource)
            {
                case (DynamicFurnitureColorSource.wall_color):
                    obj.GetComponent<MeshRenderer>().materials[matID].SetColor(attName,room.wallColor);
                    break;
                case (DynamicFurnitureColorSource.floor_color):
                    obj.GetComponent<MeshRenderer>().materials[matID].SetColor(attName, room.floorColor);
                    break;
                case (DynamicFurnitureColorSource.random):
                    var col = Random.ColorHSV();
                    obj.GetComponent<MeshRenderer>().materials[matID].SetColor(attName, col);
                    break;

            }
            
        }
        
    }

    public void DebugDrawFurnitureBlocks()
    {
        var scale = 3f;
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("displayTest");
        foreach (var g in gameObjects)
        {
            GameObject.Destroy(g);
        }

        for (int l = 0; l < fBlocks.GetLength(0); l++)
        {
            for (int w = 0; w < fBlocks.GetLength(1); w++)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Cube);
                var gizmo = GameObject.Instantiate(sphere);
                gizmo.transform.position = new Vector3(l * scale, 0.5f, w * scale);
                gizmo.transform.rotation = Quaternion.Euler(new Vector3(-90, 90, 0));
                gizmo.transform.localScale = new Vector3(0.5f, 0.5f, 2f);
                gizmo.tag = "displayTest";
                if (fBlocks[l, w].isEmpty == false)
                {
                    gizmo.GetComponent<MeshRenderer>().material.color = Color.cyan;
                }
                else
                {
                    gizmo.GetComponent<MeshRenderer>().material.color = Color.red;
                }

            }
        }
    }

    public bool isValid(int l, int w)
    {
        if (l < 0 || w < 0 || l > blocks.GetLength(0) - 1 || w > blocks.GetLength(1) - 1)
        {
            return false;
        }
        return true;
    }

    public void GenerateBlockTypes()
    {
        

        for (int l = 0; l < blocks.GetLength(0); l++)
        {
            for (int w = 0; w < blocks.GetLength(1); w++)
            {
                var s = "";
                

                if (isValid(l - 1, w) == false || blocks[l - 1, w].GetRoomID() != blocks[l, w].GetRoomID())
                {
                    s += "N";
                }
                if (isValid(l + 1,w) == false || blocks[l + 1, w].GetRoomID() != blocks[l, w].GetRoomID())
                {
                    s += "S";
                }
                if (isValid(l, w + 1) == false || blocks[l, w + 1].GetRoomID() != blocks[l, w].GetRoomID())
                {
                    s += "E";
                }
                if (isValid(l, w - 1) == false || blocks[l, w - 1].GetRoomID() != blocks[l, w].GetRoomID())
                {
                    s += "W";
                }

                if (s.Equals(""))
                {
                    blocks[l, w].type = "empty";
                    blocks[l, w].direction = "X";
                }
                else
                {
                    blocks[l, w].type = "wall";
                    blocks[l, w].direction = s;
                }

                if (l == 0 || w == 0 || l==blocks.GetLength(0)-1|| w==blocks.GetLength(0)-1)
                {
                    blocks[l, w].isEdge = true;
                }
            }
        }
    }

    public bool SplitVertical(int roomNum, int minLength, int maxVar)
    {
        Room splitRoom = GetRoom(roomNum);
        //var splitRoom = room;
        var sLength = splitRoom.GetLength();
        var sWidth = splitRoom.GetWidth();

        Room r1 = new Room(this);
        Room r2 = new Room(this);

        bool success = false;
        int trials = 0;

        List<int> randIds = new List<int>();
        for (int i = 0; i < Mathf.Min(maxVar, sWidth / 3); i++)
        //for (int i = 0; i < 1; i++)
        {
            randIds.Add(i);
        }

        randIds = randIds.OrderBy(a => Random.value).ToList();
        //Debug.Log(string.Join(", ", randIds));

        foreach (var vari in randIds)
        {
            var variation = vari;
            int[] startPos = splitRoom.topLeftCorner.GetBlockLocation(this);

            List<Block> blocks1 = new List<Block>();
            List<Block> blocks2 = new List<Block>();

            Block[] corners1 = new Block[2];
            Block[] corners2 = new Block[2];

            int add = -variation;
            if (sWidth % 2 == 0)
            {
                for (int l = startPos[0]; l < startPos[0] + sLength; l++)
                {
                    for (int w = startPos[1]; w <= startPos[1] - variation + (sWidth / 2); w++)
                    {
                        blocks1.Add(blocks[l, w]);
                    }
                }

                add += (startPos[1] + (sWidth / 2)) - 1;
                corners1[0] = blocks[startPos[0], startPos[1]];
                corners1[1] = blocks[(startPos[0] + sLength - 1), add];
            }
            else
            {
                for (int l = startPos[0]; l < startPos[0] + sLength; l++)
                {
                    for (int w = startPos[1]; w <= startPos[1] - variation + (sWidth / 2); w++)
                    {
                        blocks1.Add(blocks[l, w]);
                    }
                }

                add += (startPos[1] + (sWidth / 2));
                corners1[0] = blocks[startPos[0], startPos[1]];
                corners1[1] = blocks[(startPos[0] + sLength - 1), add];
            }

            for (int l = startPos[0]; l < startPos[0] + sLength; l++)
            {
                for (int w = startPos[1] - variation + Mathf.CeilToInt(sWidth / 2f); w < startPos[1] + sWidth; w++)
                {
                    blocks2.Add(blocks[l, w]);
                }
            }

            corners2[0] = blocks[startPos[0], add];
            corners2[1] = blocks[(startPos[0] + sLength - 1), (startPos[1] + sWidth - 1)];

            r1 = new Room(this);
            r2 = new Room(this);

            r1.SetBlocks(blocks1, corners1[0], corners1[1]);
            r2.SetBlocks(blocks2, corners2[0], corners2[1]);

            trials++;

            if (r1.GetLength() < minLength || r1.GetWidth() < minLength)
            {
                //Debug.Log("Room is too small! Cancelling split.");
                continue;
            }

            if (r2.GetLength() < minLength || r2.GetWidth() < minLength)
            {
                //Debug.Log("Room is too small! Cancelling split.");
                continue;
            }

            success = true;
            break;
        }

        if(success == false)
        {
            //Debug.Log("split failed (vertical)");
            return false;
        }

        rooms.Remove(splitRoom);
        rooms.Add(r1);
        rooms.Add(r2);

        UpdateRoomData();
        return true;
    }

    public bool SplitHorizontal(int roomNum, int minLength, int maxVar)
    {
        Room splitRoom = GetRoom(roomNum);
        //var splitRoom = room;
        var sLength = splitRoom.GetLength();
        var sWidth = splitRoom.GetWidth();

        Room r1 = new Room(this);
        Room r2 = new Room(this);

        bool success = false;
        int trials = 0;

        List<int> randIds = new List<int>();
        for (int i = 0; i < Mathf.Min(maxVar, sLength / 3); i++)
        //for (int i = 0; i < 1; i++)
        {
            randIds.Add(i);
        }

        randIds = randIds.OrderBy(a => Random.value).ToList();
        //Debug.Log(string.Join(", ", randIds));

        foreach (var vari in randIds)
        {
            //int variation = Random.Range(0, Mathf.Min(maxVar, sLength / 2));
            //variation = 0;

            var variation = vari;

            int[] startPos = splitRoom.topLeftCorner.GetBlockLocation(this);

            List<Block> blocks1 = new List<Block>();
            List<Block> blocks2 = new List<Block>();

            Block[] corners1 = new Block[2];
            Block[] corners2 = new Block[2];

            int add = -variation;
            if (sLength % 2 == 0)
            {
                for (int l = startPos[0]; l <= startPos[0] - variation + (sLength / 2); l++)
                {
                    for (int w = startPos[1]; w < startPos[1] + sWidth; w++)
                    {
                        blocks1.Add(blocks[l, w]);
                    }
                }
                add += (startPos[0] + (sLength / 2)) - 1;
                corners1[0] = blocks[startPos[0], startPos[1]];
                corners1[1] = blocks[add, (startPos[1] + sWidth - 1)];
            }
            else
            {
                for (int l = startPos[0]; l <= startPos[0] - variation + (sLength / 2); l++)
                {
                    for (int w = startPos[1]; w < startPos[1] + sWidth; w++)
                    {
                        blocks1.Add(blocks[l, w]);
                    }
                }

                add += (startPos[0] + (sLength / 2));
                corners1[0] = blocks[startPos[0], startPos[1]];
                corners1[1] = blocks[add, (startPos[1] + sWidth - 1)];
            }

            for (int l = startPos[0] - variation + Mathf.CeilToInt(sLength / 2f); l < startPos[0] + sLength; l++)
            {
                for (int w = startPos[1]; w < startPos[1] + sWidth; w++)
                {

                    blocks2.Add(blocks[l, w]);
                }
            }

            corners2[0] = blocks[add, startPos[1]];
            corners2[1] = blocks[(startPos[0] + sLength - 1), (startPos[1] + sWidth - 1)];


            r1 = new Room(this);
            r2 = new Room(this);

            r1.SetBlocks(blocks1, corners1[0], corners1[1]);
            r2.SetBlocks(blocks2, corners2[0], corners2[1]);

            trials++;

            if (r1.GetLength() < minLength || r1.GetWidth() < minLength)
            {
                //Debug.Log("Room is too small! Cancelling split.");
                continue;
            }

            if (r2.GetLength() < minLength || r2.GetWidth() < minLength)
            {
                //Debug.Log("Room is too small! Cancelling split.");
                continue;
            }

            success = true;
            break;
        }

        if(success == false)
        {
            //Debug.Log("split failed (horizontal)");
            return false;
        }

        rooms.Remove(splitRoom);
        rooms.Add(r1);
        rooms.Add(r2);

        UpdateRoomData();
        return true;
    }

    public Room GetRoom(int idNum)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if(rooms[i].id == idNum)
            {
                return rooms[i];
            }
        }
        //Debug.LogError("GetRoom() couldn't find given room! Defaulting to first available room.");
        return rooms[0];
    }

    public int GetRoomID(int idNum)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].id == idNum)
            {
                return rooms[i].id;
            }
        }
        return -1;
    }

    public void UpdateRoomData()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            UpdateRoomCorners_TLC(i);
            UpdateRoomCorners_BRC(i);
            rooms[i].CalcDimensions();
            UpdateRoomBlocks(i);
            GenerateBlockTypes();
        }
    }

    public void UpdateRoomCorners_TLC(int id)
    {
        for (int l = 0; l < blocks.GetLength(0); l++)
        {
            for (int w = 0; w < blocks.GetLength(1); w++)
            {
                if (blocks[l, w].GetRoomID() == rooms[id].id)
                {
                    rooms[id].topLeftCorner = blocks[l, w];
                    return;
                }

            }
        }
    }
    public void UpdateRoomCorners_BRC(int id)
    {
        for (int l = blocks.GetLength(0)-1; l >= 0; l--)
        {
            for (int w = blocks.GetLength(1) - 1; w >= 0; w--)
            {
                if (blocks[l, w].GetRoomID() == rooms[id].id)
                {
                    rooms[id].bottomRightCorner = blocks[l, w];
                    return;
                }

            }
        }
    }
    public void UpdateRoomBlocks(int id)
    {
        var room = rooms[id];

        var tLC = room.topLeftCorner;
        var bRC = room.bottomRightCorner;

        int[] tLC_Coords = tLC.GetBlockLocation(this);
        int[] bRC_Coords = bRC.GetBlockLocation(this);

        List<Block> b = new List<Block>();

        for (int l = tLC_Coords[0]; l <= bRC_Coords[0]; l++)
        {
            for (int w = tLC_Coords[1]; w <= bRC_Coords[1]; w++)
            {
                b.Add(blocks[l, w]);

            }
        }

        room.blocks = b;

        room.DimCheck("UpdateRoomBlocks()");
    }

    public void PrintRoom(int currentNum = -1)
    {
        if (currentNum == -1 || GetRoomID(currentNum) == -1)
        {
            currentNum = rooms[0].id;
        }
        string s = "";
        for (int l = 0; l < blocks.GetLength(0); l++)
        {
            for (int w = 0; w < blocks.GetLength(1); w++)
            {
                if (GetRoom(currentNum).topLeftCorner == blocks[l, w] || GetRoom(currentNum).bottomRightCorner == blocks[l, w])
                {
                    s += "x";
                }
                else
                {
                    s += blocks[l, w].GetRoomID();
                }
            }
            s += "\n";
        }
        Debug.Log(s);
    }

    public void PrintRoomDir()
    {
        string s = "";
        for (int l = 0; l < blocks.GetLength(0); l++)
        {
            for (int w = 0; w < blocks.GetLength(1); w++)
            {
                if(blocks[l, w].direction.Length == 1)
                {
                    s += blocks[l, w].direction + "  ";
                }
                else
                {
                    s += blocks[l, w].direction + " ";
                }
                
            }
            s += "\n";
        }
        Debug.Log(s);
    }

    public void Final()
    {
        //temporarily commenting this out while i figure out furniture placement logic :)

        //AddDoors();
        //AddFrontDoor();

        //AddWindows();
    }

    public void AddFrontDoor()
    {
        var shuffledRooms = rooms.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < shuffledRooms.Count; i++)
        {
            var room = shuffledRooms[i]; 

            for (int b = 0; b < room.blocks.Count; b++)
            {
                var block = room.blocks[b];

                if(block.type == "wall" && block.direction.Length == 1 && block.isEdge)
                {
                    block.type = "door";
                    return;
                }
            }
        }
    }

    public void AddWindows()
    {
        var shuffledRooms = rooms.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < shuffledRooms.Count; i++)
        {
            var room = shuffledRooms[i];

            for (int b = 0; b < room.blocks.Count; b++)
            {
                var block = room.blocks[b];

                if (block.type == "wall" && block.direction.Length == 1 && block.isEdge)
                {
                    int chance = (int)((Random.value * 10) + 0.5f);

                    if (chance > 5)
                    {
                        block.type = "window";
                    }
                }
            }
        }
    }

    public void AddDoors1()
    {
        for(int i = 0; i < rooms.Count; i++)
        {
            var room = rooms[i];
            for(int b = 0; b < room.blocks.Count; b++)
            {
                if (room.blocks[b].type == "wall" && room.blocks[b].isEdge == false && room.blocks[b].direction.Length == 1)
                {
                    var block = room.blocks[b];
                    int[] location = block.GetBlockLocation(this);
                    

                    if (block.direction == "N")
                    {
                        int[] newLoc = new int[2];
                        newLoc[0] = location[0] - 1;
                        newLoc[1] = location[1];
                        if (isValid(newLoc[0],newLoc[1]))
                        {
                            if(blocks[newLoc[0],newLoc[1]].direction.Length == 1 && !blocks[newLoc[0], newLoc[1]].direction.Equals("X"))
                            {
                                block.type = "door";
                                blocks[newLoc[0], newLoc[1]].type = "door";
                                break;
                            }
                        }
                    }
                    else if (block.direction == "E")
                    {
                        int[] newLoc = new int[2];
                        newLoc[0] = location[0];
                        newLoc[1] = location[1] + 1;
                        if (isValid(newLoc[0], newLoc[1]))
                        {
                            if (blocks[newLoc[0], newLoc[1]].direction.Length == 1 && !blocks[newLoc[0], newLoc[1]].direction.Equals("X"))
                            {
                                block.type = "door";
                                blocks[newLoc[0], newLoc[1]].type = "door";
                                break;
                            }
                        }
                    }
                    else if (block.direction == "S")
                    {
                        int[] newLoc = new int[2];
                        newLoc[0] = location[0] + 1;
                        newLoc[1] = location[1];
                        if (isValid(newLoc[0], newLoc[1]))
                        {
                            if (blocks[newLoc[0], newLoc[1]].direction.Length == 1 && !blocks[newLoc[0], newLoc[1]].direction.Equals("X"))
                            {
                                block.type = "door";
                                blocks[newLoc[0], newLoc[1]].type = "door";
                                break;
                            }
                        }
                    }
                    else if (block.direction == "W")
                    {
                        int[] newLoc = new int[2];
                        newLoc[0] = location[0];
                        newLoc[1] = location[1] - 1;
                        if (isValid(newLoc[0], newLoc[1]))
                        {
                            if (blocks[newLoc[0], newLoc[1]].direction.Length == 1 && !blocks[newLoc[0], newLoc[1]].direction.Equals("X"))
                            {
                                block.type = "door";
                                blocks[newLoc[0], newLoc[1]].type = "door";
                                break;
                            }
                        }
                    }

                }
            }
        }
    }

    public void AddDoors()
    {
        for (int l = 0; l < blocks.GetLength(0); l++)
        {
            for (int w = 0; w < blocks.GetLength(1); w++)
            {
                var block = blocks[l, w];
                if (block.type == "wall" && block.direction.Length == 1 && block.isEdge == false)
                {
                    if (block.direction == "N")
                    {
                        var dirBlock = blocks[l - 1, w];
                        if (dirBlock.direction == "X")
                        {
                            continue;
                        }

                        if (dirBlock.direction.Length != 1)
                        {
                            continue;
                        }
                    }
                    else if (block.direction == "S")
                    {
                        var dirBlock = blocks[l + 1, w];
                        if (dirBlock.direction == "X")
                        {
                            continue;
                        }

                        if (dirBlock.direction.Length != 1)
                        {
                            continue;
                        }
                    }
                    else if (block.direction == "E")
                    {
                        var dirBlock = blocks[l, w + 1];
                        if (dirBlock.direction == "X")
                        {
                            continue;
                        }

                        if (dirBlock.direction.Length != 1)
                        {
                            continue;
                        }
                    }
                    else if (block.direction == "W")
                    {
                        var dirBlock = blocks[l, w - 1];
                        if (dirBlock.direction == "X")
                        {
                            continue;
                        }

                        if (dirBlock.direction.Length != 1)
                        {
                            continue;
                        }
                    }

                    block.type = "door";

                }
            }
        }


        for (int l = 0; l < blocks.GetLength(0); l++)
        {
            for (int w = 0; w < blocks.GetLength(1); w++)
            {
                var block = blocks[l, w];

                if (block.type == "door")
                { 
                    if(block.direction == "N")
                    {
                        //blocks[l - 1, w ].type = "empty";
                        var num = 1;
                        while (blocks[l, w - num].type == "door" && isValid(l, w - num))
                        {
                            blocks[l, w - num].type = "wall";
                            blocks[l - 1, w - num].type = "wall";
                            num += 1;
                        }

                        num = -1;

                        while (blocks[l, w - num].type == "door" && isValid(l, w - num))
                        {
                            blocks[l, w - num].type = "wall";
                            blocks[l - 1, w - num].type = "wall";
                            num -= 1;
                        }
                    }

                    if (block.direction == "E")
                    {
                        //blocks[l, w + 1].type = "empty";
                        var num = 1;
                        while (blocks[l - num, w].type == "door" && isValid(l - num, w))
                        {
                            blocks[l - num, w].type = "wall";
                            blocks[l - num, w + 1].type = "wall";
                            num += 1;
                        }

                        num = -1;

                        while (blocks[l - num, w].type == "door" && isValid(l - num, w))
                        {
                            blocks[l - num, w].type = "wall";
                            blocks[l - num, w + 1].type = "wall";
                            num -= 1;
                        }
                    }


                }

            }
        }
    }

    public void RemoveEdgeRoom()
    {
        List<Room> rms = new List<Room>(rooms);

        for (int i = 0; i < rms.Count; i++)
        {
            var temp = rms[i];
            int randomIndex = Random.Range(i, rms.Count);
            rms[i] = rms[randomIndex];
            rms[randomIndex] = temp;
        }

        for (int r = 0; r < rms.Count; r++)
        {
            var room = rms[r];

            if ((float)room.blocks.Count / (blocks.GetLength(0) * blocks.GetLength(1)) > 0.35f){
                Debug.Log("Skipping room removal (too big)");
                continue;
            }

            string dir1 = "";
            string dir2 = "";

            //Debug.Log(room.GetLength() + ", " + blocks.GetLength(0) + " : " + room.GetWidth() + ", " + blocks.GetLength(1));
            if (room.GetLength() == blocks.GetLength(0) || room.GetWidth() == blocks.GetLength(1))
            {
                Debug.Log("Skipping room removal (full length room)");
                continue;
            }

            for (int b = 0; b<room.blocks.Count; b++)
            {
                var block = room.blocks[b];

                if (block.isEdge && block.direction.Length==1)
                {
                    dir1 = block.direction;
                    break;
                }
            }

            if (!dir1.Equals(""))
            {
                for (int b = 0; b < room.blocks.Count; b++)
                {
                    var block = room.blocks[b];

                    if (block.isEdge && block.direction.Length == 1 && !block.direction.Equals(dir1) && !Block.IsOppositeDirectionWall(dir1,block))
                    {
                        dir2 = block.direction;
                        break;
                    }
                }
                if (!dir2.Equals(""))
                {
                    //Debug.Log(dir1 + " : " + dir2);
                    for (int b = 0; b < room.blocks.Count; b++)
                    {
                        var block = room.blocks[b];

                        block.direction = "X";
                        block.type = "null";
                    }
                    rooms.Remove(room);
                    return;
                }
            }
        }


    }
}



public class Room
{
    public static int ROOMS = 0;
    public int id;
    public List<Block> blocks;
    public Block topLeftCorner;
    public Block bottomRightCorner;
    public Floor h;

    public RoomTemplate template;

    public Color wallColor;
    public Color floorColor;

    int length;
    int width;

    public Room(Floor house)
    {
        id = ROOMS;
        ROOMS++;
        this.h = house;

        wallColor = Random.ColorHSV();
        floorColor = Random.ColorHSV();
    }

    public void SetTemplate(RoomTemplate rt)
    {
        template = rt;
    }

    public RoomTemplate GetTemplate()
    {
        return template;
    }

    public void SetBlocks(List<Block> b, Block tLC, Block bRC)
    {
        blocks = b;
        for(int i = 0; i < blocks.Count; i++)
        {
            blocks[i].SetRoomID(id);
        }

        topLeftCorner = tLC;
        bottomRightCorner = bRC;

        int[] tlC_Coord = tLC.GetBlockLocation(h);
        int[] bRC_Coord = bRC.GetBlockLocation(h);

        length = Mathf.Abs(tlC_Coord[0] - bRC_Coord[0])+1;
        width = Mathf.Abs(tlC_Coord[1] - bRC_Coord[1])+1;
    }

    public void SetBlocks(Block tLC, Block bRC, Floor f)
    {
        //blocks = b;
        for (int i = 0; i < blocks.Count; i++)
        {
            blocks[i].SetRoomID(id);
        }

        topLeftCorner = tLC;
        bottomRightCorner = bRC;

        int[] tlC_Coord = tLC.GetBlockLocation(h);
        int[] bRC_Coord = bRC.GetBlockLocation(h);

        List<Block> blks = new List<Block>();

        for(int l = tlC_Coord[0]; l <  bRC_Coord[0]; l++)
        {
            for (int w = tlC_Coord[1]; w < bRC_Coord[1]; w++)
            {
                blks.Add(f.blocks[l,w]);
            }
        }

        length = Mathf.Abs(tlC_Coord[0] - bRC_Coord[0]) + 1;
        width = Mathf.Abs(tlC_Coord[1] - bRC_Coord[1]) + 1;
    }


    public int GetLength()
    {
        return length;
    }

    public int GetWidth()
    {
        return width;
    }

    public void CalcDimensions()
    {
        int[] tlC_Coord = topLeftCorner.GetBlockLocation(h);
        int[] bRC_Coord = bottomRightCorner.GetBlockLocation(h);

        length = Mathf.Abs(tlC_Coord[0] - bRC_Coord[0]) + 1;
        width = Mathf.Abs(tlC_Coord[1] - bRC_Coord[1]) + 1;

        DimCheck();
    }

    public void DimCheck(string s = "")
    {
        if (!s.Equals(""))
        {
            s = "From: " + s + " | ";
        }
        if (blocks.Count > GetWidth() * GetLength())
        {
            string p = "";
            p += s;
            p += " Room #" + id + " has MORE blocks than specified by corners! \n(" + blocks.Count + " blocks : " + (length * width) + " l*w)";
            //Debug.LogError(p);
        }

        if (blocks.Count < GetWidth() * GetLength())
        {
            string p = "";
            p += s;
            p += " Room #" + id + " has LESS blocks than specified by corners! \n(" + blocks.Count + " blocks : " + (length * width) + " l*w)";
            //Debug.LogError(p);
        }
    }
}

public class Block
{
    int roomID;
    public string type;
    public string direction;
    public bool isEdge;

    public Block()
    {
        roomID = 0;
        type = "";
        direction = "";
        isEdge = false;
    }

    public void SetRoomID(int id)
    {
        roomID = id;
    }

    public int GetRoomID()
    {
        return roomID;
    }

    public int[] GetBlockLocation(Floor h)
    {
        for (int l = 0; l < h.blocks.GetLength(0); l++)
        {
            for (int w = 0; w < h.blocks.GetLength(1); w++)
            {
                if(h.blocks[l,w] == this)
                {
                    int[] dim = { l, w };
                    return dim;
                }
            }
        }
        int[] d = { 0, 0 };
        Debug.LogError("GetBlockLocation() couldn't find location of given block! Defaulting to [0,0].");
        return d;
    }

    public GameObject ConvertBlock(Dictionary<string, GameObject> BLOCK_TYPES)
    {

        if (direction.Length == 1 && direction != "X")
            return BLOCK_TYPES["wall"];
        else if (direction.Length == 2)
            return BLOCK_TYPES["corner"];
        else if (direction == "X" && type == "empty")
            return BLOCK_TYPES["floor"];
        else
            Debug.LogError("Block type not found! | Type: " + type + ", Direction: " + direction);

        return null;
    }

    public bool IsOppositeDirectionWall(Block b)
    {
        if(direction == "N")
        {
            return b.direction.Equals("S");
        }
        else if (direction == "E")
        {
            return b.direction.Equals("W");
        }
        else if (direction == "S")
        {
            return b.direction.Equals("N");
        }
        else if (direction == "W")
        {
            return b.direction.Equals("W");
        }
        return false;
    }

    public static bool IsOppositeDirectionWall(string direction, Block b)
    {
        if (direction == "N")
        {
            return b.direction.Equals("S");
        }
        else if (direction == "E")
        {
            return b.direction.Equals("W");
        }
        else if (direction == "S")
        {
            return b.direction.Equals("N");
        }
        else if (direction == "W")
        {
            return b.direction.Equals("E");
        }
        return false;
    }
}

public class FurnitureBlock{

    public bool isEmpty = true;
    public Furniture furniture;

    public FurnitureBlock()
    {
        this.furniture = null;
        isEmpty = true;
    }

    public FurnitureBlock(Furniture f)
    {
        this.furniture = f;
        isEmpty = false;
    }

    public void SetFurniture(Furniture f)
    {
        this.furniture = f;
        isEmpty = true;
    }

}
