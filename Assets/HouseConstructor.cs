using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseConstructor : MonoBehaviour {

    public House house1;
    public int width = 10;
    public int length = 10;


    // Start is called before the first frame update
    void Start()
    {
        house1 = new House(length,width);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            house1.HousePrinter();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            house1.rooms[0].SplitRoomVertical(ref house1);
        }
    }

    
}

public class House
{
    int length;
    int width;
    public List<Room> rooms = new List<Room>();

    public House(int length, int width)
    {
        this.length = length;
        this.width = width;
        rooms.Add(new Room(length, width));
    }

    public void HousePrinter()
    {
        Block[,] blocks = new Block[length,width];
        Debug.Log("Count: " + rooms.Count);
        Debug.Log("Block Count: " + rooms[0].blocks[0,0]);
        foreach (Room room in rooms)
        {
            Debug.Log("Room # " + room.id);
            for (int l = 0; l < room.blocks.GetLength(0); l++)
            {
                for (int w = 0; w < room.blocks.GetLength(1); w++)
                {
                    blocks[room.blocks[l, w].length, room.blocks[l, w].width] = new Block(1,1,1);
                    Debug.Log(room.blocks[l, w].length + " : " + room.blocks[l, w].width);
                }
            }
        }


        string prSt = "";
        for (int l1 = 0; l1 < blocks.GetLength(0); l1++)
        {
            for (int w1 = 0; w1 < blocks.GetLength(1); w1++)
            {
                prSt += blocks[l1,w1].toString();
            }
            prSt += "\n";
        }

        Debug.Log(prSt);
    }
}

public class Room
{
    public static int ID_COUNT;
    public Block[,] blocks;
    public int id;

    public Room(int length, int width)
    {
        blocks = new Block[length, width];
        id = ID_COUNT;
        ID_COUNT++;

        for (int l = 0; l < blocks.GetLength(0); l++)
        {
            for (int w = 0; w < blocks.GetLength(1); w++)
            {
                blocks[l, w] = new Block(l, w, id);
            }
        }
    }

    public Room(int length, int width, Block[,] blocks)
    {
        this.blocks = blocks;
        id = ID_COUNT;
        ID_COUNT++;

        for (int l = 0; l < blocks.GetLength(0); l++)
        {
            for (int w = 0; w < blocks.GetLength(1); w++)
            {
                blocks[l, w] = new Block(l,w, id);
            }
        }
    }

    public void SplitRoomVertical(ref House h)
    {
        List<Room> r = new List<Room>();
        Block[,] r1Blocks = new Block[getLength(), getWidth() / 2];
        float newL = getLength();
        float newW = getWidth() / 2;

        for (int l = 0; l < newL; l++)
        {
            for (int w = 0; w < newW; w++)
            {
                r1Blocks[l, w] = blocks[l, w];
            }
        }
        Room r1 = new Room(getLength(), getWidth()/2, r1Blocks);

        Block[,] r2Blocks = new Block[getLength(), getWidth() / 2];
        for (int l = (int)newL; l < getLength(); l++)
        {
            for (int w = (int)newW; w < getWidth(); w++)
            {
                for (int l2 = 0; l < newL; l++)
                {
                    for (int w2 = 0; w < newW; w++)
                    {
                        r2Blocks[l2, w2] = blocks[l, w];
                    }
                }
            }
        }
        Room r2 = new Room(getLength(), getWidth() / 2, r2Blocks);

        Debug.Log("r1 " + r1.blocks.GetLength(0));
        Debug.Log("r1 " + r1.blocks.GetLength(1));
        Debug.Log("r2 " + r2.blocks.GetLength(0));
        Debug.Log("r2 " + r2.blocks.GetLength(1));

        Debug.Log("Count: " + r.Count);
        Debug.Log("Block Count r1: " + r1.blocks[0, 0].roomID);
        Debug.Log("Block Count r2: " + r2.blocks[0, 0].roomID);
        r.Add(r1);
        r.Add(r2);

        var index = h.rooms.IndexOf(this);
        h.rooms.InsertRange(index, r);
        h.rooms.Remove(this);
        Debug.Log("House room count (func)" + h.rooms.Count);

        Debug.Log("Count: " + h.rooms.Count);
        Debug.Log("Block Count: " + h.rooms[0].blocks[0, 0]);
    }

    public int getLength()
    {
        return blocks.GetLength(0);
    }

    public int getWidth()
    {
        return blocks.GetLength(1);
    }
}

public class Block
{
    public int length;
    public int width;
    public int roomID;
    public Block(int l, int w, int id)
    {
        length = l;
        width = w;
        roomID = id;
    }

    public string toString()
    {
        return length + width + roomID + "";
    }
}
