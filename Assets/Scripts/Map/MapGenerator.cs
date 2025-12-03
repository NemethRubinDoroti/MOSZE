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

    [Header("Enemy Spawning")]
    public EnemySpawner enemySpawner;
    public int minEnemiesPerRoom = 0;
    public int maxEnemiesPerRoom = 3;
    [Range(0f, 1f)]
    public float enemySpawnChance = 0.6f; // 60% esély, hogy egy szobában legyen ellenség
    public bool spawnBossInLastRoom = true;

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
        PlaceObjects();
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

    private void PlaceObjects()
    {
        // Meglévő ellenfelek törlése
        if (enemySpawner != null)
        {
            enemySpawner.ClearAllEnemies();
        }

        if (rooms == null || rooms.Count == 0)
        {
            return;
        }

        // Spawnolás
        for (int i = 0; i < rooms.Count; i++)
        {
            Room room = rooms[i];

            // Boss az utolsó szobába
            if (spawnBossInLastRoom && i == rooms.Count - 1)
            {
                SpawnEnemyInRoom(room, Enemy2D.EnemyType.Boss);
                continue;
            }

            // Random spawnolás a többi enemynek
            if (Random.Range(0f, 1f) < enemySpawnChance)
            {
                int enemiesToSpawn = Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom + 1);
                for (int j = 0; j < enemiesToSpawn; j++)
                {
                    Enemy2D.EnemyType enemyType = GetRandomEnemyType();
                    SpawnEnemyInRoom(room, enemyType);
                }
            }
        }
    }

    private void SpawnEnemyInRoom(Room room, Enemy2D.EnemyType enemyType)
    {
        if (enemySpawner == null)
        {
            Debug.LogWarning("[MapGenerator] Nincs EnemySpawner beállítva");
            return;
        }

        // Find a valid walkable position in the room
        Vector2Int? spawnPosition = FindValidSpawnPosition(room);

        if (spawnPosition.HasValue)
        {
            enemySpawner.SpawnEnemy(enemyType, spawnPosition.Value);
        }
        else
        {
            Debug.LogWarning($"[MapGenerator] Nincs érvényes spawn pont a szobában: ({room.position.x}, {room.position.y})");
        }
    }

    private Vector2Int? FindValidSpawnPosition(Room room)
    {
        int attempts = 0;
        int maxAttempts = 50;

        while (attempts < maxAttempts)
        {
            int x = Random.Range(room.position.x + 1, room.position.x + room.width - 1);
            int y = Random.Range(room.position.y + 1, room.position.y + room.height - 1);
            Vector2Int pos = new Vector2Int(x, y);

            if (IsWalkable(pos))
            {
                return pos;
            }

            attempts++;
        }

        return null;
    }

    // súlyozott random enemy spawn
    private Enemy2D.EnemyType GetRandomEnemyType()
    {
        int rand = Random.Range(0, 100);
        if (rand < 50) return Enemy2D.EnemyType.SecurityBot;
        if (rand < 80) return Enemy2D.EnemyType.PatrolBot;
        return Enemy2D.EnemyType.HeavyBot;
    }
}

