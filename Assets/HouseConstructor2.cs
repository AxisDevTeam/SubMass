using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseConstructor2 : MonoBehaviour
{
    public int length;
    public int width;
    public Floor f;
    public House h;
    public int _currentSplitRoom = 0;
    // Start is called before the first frame update
    void Start()
    {
        h = new House(length,width);
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

        if (Input.GetKeyDown(KeyCode.W))
        {
            f.SplitVertical(_currentSplitRoom);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            f.SplitHorizontal(_currentSplitRoom);
        }

    }
}

public class House
{
    public List<Floor> floors;
    public House(int length, int width)
    {
        floors = new List<Floor>();
        var f1 = new Floor(length,width);
        floors.Add(f1);
    }

    public void BuildHouse()
    {

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

    public void GenerateBlockTypes()
    {
        bool isValid(int l, int w){
            if(l < 0 || w < 0 || l>blocks.GetLength(0)-1 || w > blocks.GetLength(1) - 1)
            {
                return false;
            }
            return true;
        }

        var s = "";

        for (int l = 0; l < blocks.GetLength(0); l++)
        {
            for (int w = 0; w < blocks.GetLength(1); w++)
            {
                blocks[l, w].type = "wall";
                if (isValid(l - 1, w) == false || blocks[l - 1, w].GetRoomID() != blocks[l, w].GetRoomID())
                {
                    s += "N";
                }
                else if (isValid(l + 1, w) == false || blocks[l + 1, w].GetRoomID() != blocks[l, w].GetRoomID())
                {
                    s += "S";
                }
                else if (isValid(l, w - 1) == false || blocks[l, w - 1].GetRoomID() != blocks[l, w].GetRoomID())
                {
                    s += "W";
                }
                else if (isValid(l, w + 1) == false || blocks[l, w + 1].GetRoomID() != blocks[l, w].GetRoomID())
                {
                    s += "E";
                }
                else
                {
                    blocks[l, w].type = "empty";
                }
                blocks[l, w].direction = s;
            }
        }
    }

    public void SplitVertical(int roomNum)
    {
        Room splitRoom = GetRoom(roomNum);
        var sLength = splitRoom.GetLength();
        var sWidth = splitRoom.GetWidth();

        int[] startPos = splitRoom.topLeftCorner.GetBlockLocation(this);

        List<Block> blocks1 = new List<Block>();
        List<Block> blocks2 = new List<Block>();

        Block[] corners1 = new Block[2];
        Block[] corners2 = new Block[2];

        if (sWidth % 2 == 0)
        {
            for (int l = startPos[0]; l < startPos[0] + sLength; l++)
            {
                for (int w = startPos[1]; w <= startPos[1] + (sWidth / 2); w++)
                {
                    blocks1.Add(blocks[l, w]);
                }
            }

            corners1[0] = blocks[startPos[0], startPos[1]];
            corners1[1] = blocks[(startPos[0] + sLength-1), (startPos[1] + (sWidth / 2)) - 1];
        }
        else
        {
            for (int l = startPos[0]; l < startPos[0] + sLength; l++)
            {
                for (int w = startPos[1]; w <= startPos[1] + (sWidth / 2); w++)
                {
                    blocks1.Add(blocks[l, w]);
                }
            }

            corners1[0] = blocks[startPos[0], startPos[1]];
            corners1[1] = blocks[(startPos[0] + sLength-1), (startPos[1] + (sWidth / 2))];
        }

        for (int l = startPos[0]; l < startPos[0] + sLength; l++)
        {
            for (int w = startPos[1] + Mathf.CeilToInt(sWidth / 2f); w < startPos[1] + sWidth; w++)
            {
                blocks2.Add(blocks[l, w]);
            }
        }

        corners2[0] = blocks[startPos[0], startPos[1]];
        corners2[1] = blocks[(startPos[0] + sLength-1), (startPos[1] + sWidth-1)];


        Room r1 = new Room(this);
        Room r2 = new Room(this);

        r1.SetBlocks(blocks1, corners1[0], corners1[1]);
        r2.SetBlocks(blocks2, corners2[0], corners2[1]);

        rooms.Remove(splitRoom);
        rooms.Add(r1);
        rooms.Add(r2);

        UpdateRoomData();
    }

    public void SplitHorizontal(int roomNum)
    {
        Room splitRoom = GetRoom(roomNum);
        var sLength = splitRoom.GetLength();
        var sWidth = splitRoom.GetWidth();

        int[] startPos = splitRoom.topLeftCorner.GetBlockLocation(this);

        List<Block> blocks1 = new List<Block>();
        List<Block> blocks2 = new List<Block>();

        Block[] corners1 = new Block[2];
        Block[] corners2 = new Block[2];

        if (sLength % 2 == 0)
        {
            for (int l = startPos[0]; l <= startPos[0] + (sLength / 2); l++)
            {
                for (int w = startPos[1]; w < startPos[1] + sWidth; w++)
                {
                    blocks1.Add(blocks[l, w]);
                }
            }

            corners1[0] = blocks[startPos[0], startPos[1]];
            corners1[1] = blocks[(startPos[0] + (sLength / 2)) - 1, (startPos[1] + sWidth - 1)];
        }
        else
        {
            for (int l = startPos[0]; l <= startPos[0] + (sLength / 2); l++)
            {
                for (int w = startPos[1]; w < startPos[1] + sWidth; w++)
                {
                    blocks1.Add(blocks[l, w]);
                }
            }

            corners1[0] = blocks[startPos[0], startPos[1]];
            corners1[1] = blocks[(startPos[0] + (sLength / 2)), (startPos[1] + sWidth - 1)];
        }

        for (int l = startPos[0] + Mathf.CeilToInt(sLength / 2f); l < startPos[0] + sLength; l++)
        {
            for (int w = startPos[1]; w < startPos[1] + sWidth; w++)
            {

                blocks2.Add(blocks[l, w]);
            }
        }

        corners2[0] = blocks[startPos[0], startPos[1]];
        corners2[1] = blocks[(startPos[0] + sLength - 1), (startPos[1] + sWidth - 1)];


        Room r1 = new Room(this);
        Room r2 = new Room(this);

        r1.SetBlocks(blocks1, corners1[0], corners1[1]);
        r2.SetBlocks(blocks2, corners2[0], corners2[1]);

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
            Debug.LogError(p);
        }

        if (blocks.Count < GetWidth() * GetLength())
        {
            string p = "";
            p += s;
            p += " Room #" + id + " has LESS blocks than specified by corners! \n(" + blocks.Count + " blocks : " + (length * width) + " l*w)";
            Debug.LogError(p);
        }
    }
}

public class Block
{
    int roomID;
    public string type;
    public string direction;

    public Block()
    {
        roomID = 0;
        type = "";
        direction = "";
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
}