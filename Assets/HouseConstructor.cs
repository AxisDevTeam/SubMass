using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseConstructor : MonoBehaviour
{
    public int length;
    public int width;
    public Floor f;
    public House h;
    public int _currentSplitRoom = 0;
    public int minRoomDimension = 3;

    public int roomSplitRepetitions = 25;

    public int variation;

    public GameObject NBlock;
    public GameObject SBlock;
    public GameObject EBlock;
    public GameObject WBlock;

    public GameObject NEBlock;
    public GameObject NWBlock;
    public GameObject SEBlock;
    public GameObject SWBlock;

    public static GameObject[] BLOCK_TYPES;




    // Start is called before the first frame update
    void Start()
    {
        BLOCK_TYPES = new GameObject[8];

        BLOCK_TYPES[0] = NBlock;
        BLOCK_TYPES[1] = SBlock;
        BLOCK_TYPES[2] = EBlock;
        BLOCK_TYPES[3] = WBlock;
        BLOCK_TYPES[4] = NEBlock;
        BLOCK_TYPES[5] = NWBlock;
        BLOCK_TYPES[6] = SEBlock;
        BLOCK_TYPES[7] = SWBlock;

        h = new House(length,width, BLOCK_TYPES);
        f = h.floors[0];
        

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            f.PrintRoom(_currentSplitRoom);

            Debug.Log(f.GetRoom(_currentSplitRoom).GetLength() + " : " + f.GetRoom(_currentSplitRoom).GetWidth());
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            f.PrintRoomDir();

            Debug.Log(f.GetRoom(_currentSplitRoom).GetLength() + " : " + f.GetRoom(_currentSplitRoom).GetWidth());
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            f.SplitVertical(_currentSplitRoom, minRoomDimension, variation);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            f.SplitHorizontal(_currentSplitRoom, minRoomDimension, variation);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            h.BuildHouse();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            h = new House(length,width,BLOCK_TYPES);
            for (int i = 0; i < roomSplitRepetitions; i++)
            {
                var rand = Random.Range(0, 10f);
                if (rand < 5)
                {
                    var room = Random.Range(0, h.floors[0].rooms.Count);
                    var id = h.floors[0].GetRoomID(room);
                    h.floors[0].SplitHorizontal(id,minRoomDimension,variation);
                }
                else
                {
                    var room = Random.Range(0, h.floors[0].rooms.Count);
                    var id = h.floors[0].GetRoomID(room);
                    h.floors[0].SplitVertical(id, minRoomDimension,variation);
                }
                
            }
            h.BuildHouse();
        }

    }
}

public class House
{
    public List<Floor> floors;
    GameObject[] BLOCK_TYPES;
    public House(int length, int width, GameObject[] BLOCK_TYPES)
    {
        floors = new List<Floor>();
        var f1 = new Floor(length,width);
        floors.Add(f1);
        this.BLOCK_TYPES = BLOCK_TYPES;
    }

    public void BuildHouse()
    {
        DestroyHouse();
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
                    if (floors[f].blocks[l, w].type.Equals("wall") || floors[f].blocks[l, w].type.Equals("door"))
                    {
                        
                        var go = floors[f].blocks[l, w].ConvertBlock(BLOCK_TYPES);

                        var obj = GameObject.Instantiate(go);
                        obj.transform.position = new Vector3(l, f, w);
                        obj.transform.rotation = Quaternion.Euler(new Vector3(-90,90,0));
                        obj.tag = "house";
                        obj.GetComponent<MeshRenderer>().materials[0].color = Color.red;
                        if (floors[f].blocks[l, w].type.Equals("door"))
                        {
                            obj.GetComponent<MeshRenderer>().materials[0].color = Color.white;
                        }
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
}

public class Floor
{
    public Block[,] blocks;
    public List<Room> rooms;
    

    public Floor(int length, int width)
    {
        List<Block> b = new List<Block>();
        // populate blocks list with new blocks of roomID 0
        blocks = new Block[length, width];
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

                if (l == 0 || w == 0 || l==blocks.GetLength(0)|| w==blocks.GetLength(0))
                {
                    blocks[l, w].isEdge = true;
                }
            }
        }



    }

    public void SplitVertical(int roomNum, int minLength, int maxVar)
    {
        Room splitRoom = GetRoom(roomNum);
        var sLength = splitRoom.GetLength();
        var sWidth = splitRoom.GetWidth();

        int variation = Random.Range(0, Mathf.Min(maxVar, sWidth / 2));

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
            corners1[1] = blocks[(startPos[0] + sLength-1), add];
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
            corners1[1] = blocks[(startPos[0] + sLength-1), add];
        }

        for (int l = startPos[0]; l < startPos[0] + sLength; l++)
        {
            for (int w = startPos[1] - variation + Mathf.CeilToInt(sWidth / 2f); w < startPos[1] + sWidth; w++)
            {
                blocks2.Add(blocks[l, w]);
            }
        }

        corners2[0] = blocks[startPos[0], add];
        corners2[1] = blocks[(startPos[0] + sLength-1), (startPos[1] + sWidth-1)];


        Room r1 = new Room(this);
        Room r2 = new Room(this);

        r1.SetBlocks(blocks1, corners1[0], corners1[1]);
        r2.SetBlocks(blocks2, corners2[0], corners2[1]);

        if (r1.GetLength() < minLength || r1.GetWidth() < minLength)
        {
            //Debug.Log("Room is too small! Cancelling split.");
            return;
        }

        if (r2.GetLength() < minLength || r2.GetWidth() < minLength)
        {
            //Debug.Log("Room is too small! Cancelling split.");
            return;
        }

        rooms.Remove(splitRoom);
        rooms.Add(r1);
        rooms.Add(r2);

        UpdateRoomData();
    }

    public void SplitHorizontal(int roomNum, int minLength, int maxVar)
    {
        Room splitRoom = GetRoom(roomNum);
        var sLength = splitRoom.GetLength();
        var sWidth = splitRoom.GetWidth();

        int variation = Random.Range(0, Mathf.Min(maxVar, sLength / 2));

        int[] startPos = splitRoom.topLeftCorner.GetBlockLocation(this);

        List<Block> blocks1 = new List<Block>();
        List<Block> blocks2 = new List<Block>();

        Block[] corners1 = new Block[2];
        Block[] corners2 = new Block[2];

        int add = -variation;
        if (sLength % 2 == 0)
        {
            for (int l = startPos[0]; l <= startPos[0] -variation + (sLength / 2); l++)
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


        Room r1 = new Room(this);
        Room r2 = new Room(this);

        r1.SetBlocks(blocks1, corners1[0], corners1[1]);
        r2.SetBlocks(blocks2, corners2[0], corners2[1]);

        if (r1.GetLength() < minLength || r1.GetWidth() < minLength)
        {
            //Debug.Log("Room is too small! Cancelling split.");
            return;
        }

        if (r2.GetLength() < minLength || r2.GetWidth() < minLength)
        {
            //Debug.Log("Room is too small! Cancelling split.");
            return;
        }

        rooms.Remove(splitRoom);
        rooms.Add(r1);
        rooms.Add(r2);

        UpdateRoomData();
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
        AddDoors();
    }

    public void AddDoors()
    {
        for(int i = 0; i < rooms.Count; i++)
        {
            var room = rooms[i];
            for(int b = 0; b < room.blocks.Count; b++)
            {
                if (room.blocks[b].type == "wall" && room.blocks[b].direction.Length == 1)
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
}



public class Room
{
    public static int ROOMS = 0;
    public int id;
    public List<Block> blocks;
    public Block topLeftCorner;
    public Block bottomRightCorner;
    public Floor h;

    int length;
    int width;

    public Room(Floor house)
    {
        id = ROOMS;
        ROOMS++;
        this.h = house;
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

    public GameObject ConvertBlock(GameObject[] BLOCK_TYPES)
    {
        
        if(direction=="N")
                return BLOCK_TYPES[0];
        else if(direction == "S")
                return BLOCK_TYPES[1];
        else if(direction == "E")
                return BLOCK_TYPES[2];
        else if(direction == "W")
                return BLOCK_TYPES[3];

        else if(direction == "NE")
                return BLOCK_TYPES[4];
        else if(direction == "NW")
                return BLOCK_TYPES[5];
        else if(direction == "SE")
                return BLOCK_TYPES[6];
        else if(direction == "SW")
                return BLOCK_TYPES[7];
        else
        {
            Debug.LogError("Block type not found! | Type: " + type +  ", Direction: " + direction);
        }


        return null;
    }
}