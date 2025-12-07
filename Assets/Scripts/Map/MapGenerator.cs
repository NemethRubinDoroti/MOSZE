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

    [Header("Hostage Spawning")]
    public HostageSpawner hostageSpawner;
    public int minHostagesPerRoom = 0;
    public int maxHostagesPerRoom = 2;
    [Range(0f, 1f)]
    public float hostageSpawnChance = 0.4f; // 40% esély, hogy egy szobában legyen túsz

    [Header("Item Spawning")]
    public ItemSpawner itemSpawner;
    public int minItemsPerRoom = 0;
    public int maxItemsPerRoom = 2;
    [Range(0f, 1f)]
    public float itemSpawnChance = 0.5f; // 50% esély, hogy egy szobában legyen item

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
                FillRoom(newRoom);
            }

            attempts++;
        }
    }

    private void FillRoom(Room room)
    {
        for (int x = room.position.x; x < room.position.x + room.width; x++)
        {
            for (int y = room.position.y; y < room.position.y + room.height; y++)
            {
                if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                {
                    map[x, y] = true;
                }
            }
        }
    }

    private void GenerateCorridors()
    {
        ConnectRooms();
    }

    private void ConnectRooms()
    {
        if (rooms.Count < 2) return;

        // Szobák összekötése
        for (int i = 0; i < rooms.Count - 1; i++)
        {
            Vector2Int start = rooms[i].GetCenter();
            Vector2Int end = rooms[i + 1].GetCenter();

            BuildLShapedCorridor(start, end);
        }
    }

    private void BuildLShapedCorridor(Vector2Int start, Vector2Int end)
    {
        int x = start.x;
        int y = start.y;

        // Függőleges
        while (x != end.x)
        {
            if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
            {
                map[x, y] = true;
            }
            x += (end.x > start.x) ? 1 : -1;
        }

        // Vízszintes
        while (y != end.y)
        {
            if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
            {
                map[x, y] = true;
            }
            y += (end.y > start.y) ? 1 : -1;
        }
    }

    private void PlaceTiles()
    {
        if (groundTilemap == null || wallTilemap == null) return;

        // Meglévő csempék törlése
        groundTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        // Padló lehelyezése (szobák és folyosók)
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (map[x, y] && floorTile != null)
                {
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
                }
            }
        }

        // Falak lehelyezése
        PlaceWalls();
    }

    // falazás
    private void PlaceWalls()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (!map[x, y])
                {
                    bool adjacentToFloor = false;
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            int nx = x + dx;
                            int ny = y + dy;
                            if (nx >= 0 && nx < mapWidth && ny >= 0 && ny < mapHeight)
                            {
                                if (map[nx, ny])
                                {
                                    adjacentToFloor = true;
                                    break;
                                }
                            }
                        }
                        if (adjacentToFloor) break;
                    }

                    if (adjacentToFloor && wallTile != null)
                    {
                        wallTilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
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

        // Meglévő túszok törlése
        if (hostageSpawner != null)
        {
            hostageSpawner.ClearAllHostages();
        }

        // Meglévő tárgyak törlése
        if (itemSpawner != null)
        {
            itemSpawner.ClearAllItems();
        }

        if (rooms == null || rooms.Count == 0)
        {
            return;
        }

        int totalHostages = 0;

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

            // Random spawn ellenségeknek
            if (Random.Range(0f, 1f) < enemySpawnChance)
            {
                int enemiesToSpawn = Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom + 1);
                for (int j = 0; j < enemiesToSpawn; j++)
                {
                    Enemy2D.EnemyType enemyType = GetRandomEnemyType();
                    SpawnEnemyInRoom(room, enemyType);
                }
            }

            // Random spawn túszoknak
            if (Random.Range(0f, 1f) < hostageSpawnChance)
            {
                int hostagesToSpawn = Random.Range(minHostagesPerRoom, maxHostagesPerRoom + 1);
                for (int j = 0; j < hostagesToSpawn; j++)
                {
                    SpawnHostageInRoom(room);
                    totalHostages++;
                }
            }

            // Random spawn tárgyaknak
            if (Random.Range(0f, 1f) < itemSpawnChance)
            {
                int itemsToSpawn = Random.Range(minItemsPerRoom, maxItemsPerRoom + 1);
                for (int j = 0; j < itemsToSpawn; j++)
                {
                    Item2D.ItemType itemType = GetRandomItemType();
                    SpawnItemInRoom(room, itemType);
                }
            }
        }

        // Túszok számlálásához
        if (HostageManager.Instance != null)
        {
            HostageManager.Instance.InitializeHostageCount(totalHostages);
        }
    }

    private void SpawnEnemyInRoom(Room room, Enemy2D.EnemyType enemyType)
    {
        if (enemySpawner == null)
        {
            Debug.LogWarning("[MapGenerator] Nincs EnemySpawner beállítva");
            return;
        }

        // Érvényes spawn keresés
        Vector2Int? spawnPosition = FindValidSpawnPosition(room);

        if (spawnPosition.HasValue)
        {
            enemySpawner.SpawnEnemy(enemyType, spawnPosition.Value);
        }
        else
        {
            Debug.LogWarning($"[MapGenerator] Nincs érvényes spawn pont az ellenfélnek: ({room.position.x}, {room.position.y})");
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

    private void SpawnHostageInRoom(Room room)
    {
        if (hostageSpawner == null)
        {
            Debug.LogWarning("[MapGenerator] HostageSpawner nincs hozzárendelve");
            return;
        }

        // Érvényes spawn keresés
        Vector2Int? spawnPosition = FindValidSpawnPosition(room);

        if (spawnPosition.HasValue)
        {
            hostageSpawner.SpawnHostage(spawnPosition.Value);
        }
        else
        {
            Debug.LogWarning($"[MapGenerator] Nincs megfelelő spawn a túsznak: ({room.position.x}, {room.position.y})");
        }
    }

    // súlyozott random enemy spawn
    private Enemy2D.EnemyType GetRandomEnemyType()
    {
        int rand = Random.Range(0, 100);
        if (rand < 50) return Enemy2D.EnemyType.SecurityBot;
        if (rand < 80) return Enemy2D.EnemyType.PatrolBot;
        return Enemy2D.EnemyType.HeavyBot;
    }

    private void SpawnItemInRoom(Room room, Item2D.ItemType itemType)
    {
        if (itemSpawner == null)
        {
            Debug.LogWarning("[MapGenerator] ItemSpawner nincs hozzárendelve");
            return;
        }

        // Érvényes spawn keresés
        Vector2Int? spawnPosition = FindValidSpawnPosition(room);

        if (spawnPosition.HasValue)
        {
            itemSpawner.SpawnItem(itemType, spawnPosition.Value);
        }
        else
        {
            Debug.LogWarning($"[MapGenerator] Nincs megfelelő spawn a tárgynak: ({room.position.x}, {room.position.y})");
        }
    }

    private Item2D.ItemType GetRandomItemType()
    {
        int rand = Random.Range(0, 100);
        
        // 30% Heal
        if (rand < 30) return Item2D.ItemType.Heal;
        // 15% Weapon
        if (rand < 45) return Item2D.ItemType.Weapon;
        // 15% Defense
        if (rand < 60) return Item2D.ItemType.Defense;
        // 15% Speed
        if (rand < 75) return Item2D.ItemType.Speed;
        // 15% Accuracy
        if (rand < 90) return Item2D.ItemType.Accuracy;
        // 10% Treasure
        return Item2D.ItemType.Treasure;
    }
}

