using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int mapWidth = 100;
    public int mapHeight = 100;
    public int roomCount = 15;
    public int minRoomSize = 5;
    public int maxRoomSize = 15;

    [Header("Tiles")]
    public TileBase floorTile;
    public TileBase wallTile;
    public TileBase doorTile;

    [Header("References")]
    public Tilemap groundTilemap;
    public Tilemap wallTilemap;

    private int currentSeed;
    private List<Room> rooms;
    private bool[,] map;

    public void GenerateMap(int seed)
    {
        currentSeed = seed;
        Random.InitState(seed);

        rooms = new List<Room>();
        map = new bool[mapWidth, mapHeight];

        GenerateRooms();
        GenerateCorridors();
        PlaceTiles();
    }

    private void GenerateRooms()
    {
        int attempts = 0;
        int maxAttempts = roomCount * 10;

        while (rooms.Count < roomCount && attempts < maxAttempts)
        {
            int width = Random.Range(minRoomSize, maxRoomSize + 1);
            int height = Random.Range(minRoomSize, maxRoomSize + 1);
            int x = Random.Range(1, mapWidth - width - 1);
            int y = Random.Range(1, mapHeight - height - 1);

            Room newRoom = new Room(new Vector2Int(x, y), width, height, Room.RoomType.ControlRoom);

            bool overlaps = false;
            foreach (Room existingRoom in rooms)
            {
                if (newRoom.Overlaps(existingRoom))
                {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps)
            {
                rooms.Add(newRoom);
            }

            attempts++;
        }
    }

    private void GenerateCorridors()
    {
        // TODO: Folyosó generálás...
    }

    private void PlaceTiles()
    {
        if (groundTilemap == null || wallTilemap == null) return;

        // Meglévő csempék törlése
        groundTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        // Padló lehelyezése
        foreach (Room room in rooms)
        {
            for (int x = room.position.x; x < room.position.x + room.width; x++)
            {
                for (int y = room.position.y; y < room.position.y + room.height; y++)
                {
                    if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                    {
                        map[x, y] = true;
                        if (floorTile != null)
                        {
                            groundTilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
                        }
                    }
                }
            }
        }
    }

    public List<Room> GetRooms()
    {
        return rooms;
    }

    public bool IsWalkable(Vector2Int position)
    {
        if (position.x < 0 || position.x >= mapWidth || position.y < 0 || position.y >= mapHeight)
        {
            return false;
        }
        return map[position.x, position.y];
    }
}

