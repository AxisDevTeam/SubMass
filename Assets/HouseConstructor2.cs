using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseConstructor2 : MonoBehaviour
{
    public int length;
    public int width;
    public _House h;
    public int _currentSplitRoom = 0;
    // Start is called before the first frame update
    void Start()
    {
        h = new _House(length,width);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            h.PrintRoom(_currentSplitRoom);

            Debug.Log(h.GetRoom(_currentSplitRoom).GetLength() + " : " + h.GetRoom(_currentSplitRoom).GetWidth());
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            h.SplitVertical(_currentSplitRoom);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            h.SplitHorizontal(_currentSplitRoom);
        }
    }
}

public class _House
{
    public _Block[,] blocks;
    public List<_Room> rooms;
    

    public _House(int length, int width)
    {
        List<_Block> b = new List<_Block>();
        // populate blocks list with new blocks of roomID 0
        blocks = new _Block[length, width];
        rooms = new List<_Room>();
        for (int l = 0; l < blocks.GetLength(0); l++)
        {
            for (int w = 0; w < blocks.GetLength(1); w++)
            {
                blocks[l, w] = new _Block();
                b.Add(blocks[l, w]);
            }
        }

        // add room 0
        var r = new _Room(this);
        r.SetBlocks(b,blocks[0,0],blocks[length-1,width-1]);
        rooms.Add(r);
    }

    public void SplitVertical(int roomNum)
    {
        _Room splitRoom = GetRoom(roomNum);
        var sLength = splitRoom.GetLength();
        var sWidth = splitRoom.GetWidth();

        int[] startPos = splitRoom.topLeftCorner.GetBlockLocation(this);

        List<_Block> blocks1 = new List<_Block>();
        List<_Block> blocks2 = new List<_Block>();

        _Block[] corners1 = new _Block[2];
        _Block[] corners2 = new _Block[2];

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


        _Room r1 = new _Room(this);
        _Room r2 = new _Room(this);

        r1.SetBlocks(blocks1, corners1[0], corners1[1]);
        r2.SetBlocks(blocks2, corners2[0], corners2[1]);

        rooms.Remove(splitRoom);
        rooms.Add(r1);
        rooms.Add(r2);
    }

    public void SplitHorizontal(int roomNum)
    {
        _Room splitRoom = GetRoom(roomNum);
        var sLength = splitRoom.GetLength();
        var sWidth = splitRoom.GetWidth();

        int[] startPos = splitRoom.topLeftCorner.GetBlockLocation(this);

        List<_Block> blocks1 = new List<_Block>();
        List<_Block> blocks2 = new List<_Block>();

        _Block[] corners1 = new _Block[2];
        _Block[] corners2 = new _Block[2];

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

        for (int l = startPos[0] + Mathf.CeilToInt(sLength / 2f); l < startPos[1] + sLength; l++)
        {
            for (int w = startPos[1]; w < startPos[1] + sWidth; w++)
            {
                blocks2.Add(blocks[l, w]);
            }
        }

        corners2[0] = blocks[startPos[0], startPos[1]];
        corners2[1] = blocks[(startPos[0] + sLength - 1), (startPos[1] + sWidth - 1)];


        _Room r1 = new _Room(this);
        _Room r2 = new _Room(this);

        r1.SetBlocks(blocks1, corners1[0], corners1[1]);
        r2.SetBlocks(blocks2, corners2[0], corners2[1]);

        rooms.Remove(splitRoom);
        rooms.Add(r1);
        rooms.Add(r2);
    }

    public _Room GetRoom(int idNum)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if(rooms[i].id == idNum)
            {
                return rooms[i];
            }
        }
        Debug.LogError("GetRoom() couldn't find given room! Defaulting to first available room.");
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

public class _Room
{
    public static int ROOMS = 0;
    public int id;
    public List<_Block> blocks;
    public _Block topLeftCorner;
    public _Block bottomRightCorner;
    public _House h;

    int length;
    int width;

    public _Room(_House house)
    {
        id = ROOMS;
        ROOMS++;
        this.h = house;
    }

    public void SetBlocks(List<_Block> b, _Block tLC, _Block bRC)
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
}

public class _Block
{
    int roomID;

    public _Block()
    {
        roomID = 0;
    }

    public void SetRoomID(int id)
    {
        roomID = id;
    }

    public int GetRoomID()
    {
        return roomID;
    }

    public int[] GetBlockLocation(_House h)
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