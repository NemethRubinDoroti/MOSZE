using System.Collections.Generic;
using UnityEngine;

// Szobák

public class Room
{
    public Vector2Int position;
    public int width;
    public int height;
    public RoomType type;
    public List<Vector2Int> doors;
    public List<Vector2Int> objectPositions;

    // Érdekes lehet ha többféle van?
    public enum RoomType
    {
        ControlRoom,
        Storage,
        Laboratory,
        Corridor,
        Special
    }

    public Room(Vector2Int position, int width, int height, RoomType type)
    {
        this.position = position;
        this.width = width;
        this.height = height;
        this.type = type;
        this.doors = new List<Vector2Int>();
        this.objectPositions = new List<Vector2Int>();
    }

    // Hol a közepe?
    public Vector2Int GetCenter()
    {
        return new Vector2Int(
            position.x + width / 2,
            position.y + height / 2
        );
    }

    // adott pont a szobában van?
    public bool IsInside(Vector2Int pos)
    {
        return pos.x >= position.x && pos.x < position.x + width &&
               pos.y >= position.y && pos.y < position.y + height;
    }

    // ajtó helyének hozzáadása
    public void AddDoor(Vector2Int doorPos)
    {
        if (!doors.Contains(doorPos))
        {
            doors.Add(doorPos);
        }
    }

    // fedi-e egymást két szoba?
    public bool Overlaps(Room other)
    {
        return position.x < other.position.x + other.width &&
               position.x + width > other.position.x &&
               position.y < other.position.y + other.height &&
               position.y + height > other.position.y;
    }
}

