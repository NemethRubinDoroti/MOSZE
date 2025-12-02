using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Player2D player2D;
    private BoxCollider2D playerCollider;

    private MapGenerator cachedMapGenerator;
    private Tilemap cachedGroundTilemap;
    private Vector2 cachedColliderSize;
    private Vector2 cachedColliderOffset;
    private bool cacheInitialized = false;

    private void Awake()
    {
        playerCollider = GetComponent<BoxCollider2D>();
        if (playerCollider == null)
        {
            playerCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        if (playerCollider != null)
        {
            playerCollider.size = new Vector2(0.3f, 0.1f);
            playerCollider.offset = new Vector2(0f, -0.5f);
            playerCollider.isTrigger = true;
        }
    }

    private void Start()
    {
        player2D = GetComponent<Player2D>();
        if (player2D == null)
        {
            Debug.LogError("PlayerController2D requires Player2D component!");
        }

        // BoxCollider ellenőrzése
        if (playerCollider == null)
        {
            playerCollider = GetComponent<BoxCollider2D>();
            if (playerCollider == null)
            {
                playerCollider = gameObject.AddComponent<BoxCollider2D>();
            }
        }

        // Collider setup
        if (playerCollider != null)
        {
            playerCollider.size = new Vector2(0.3f, 0.1f);
            playerCollider.offset = new Vector2(0f, -0.5f);
            playerCollider.isTrigger = true;

            // Értékek cachelése
            cachedColliderSize = playerCollider.size;
            cachedColliderOffset = playerCollider.offset;
        }

        InitializeCache();
    }

    // teljesítmény javítás miatt cache-eljük a map generator és tilemap referenciákat
    private void InitializeCache()
    {
        if (GameManager2D.Instance != null && GameManager2D.Instance.mapGenerator != null)
        {
            cachedMapGenerator = GameManager2D.Instance.mapGenerator;
            cachedGroundTilemap = cachedMapGenerator.groundTilemap;
            cacheInitialized = true;
        }
        else
        {
            cacheInitialized = false;
        }
    }

    private void Update()
    {
        if (player2D == null) return;

        // Pause ellenőrzés
        if (GameManager2D.Instance != null &&
            GameManager2D.Instance.currentState != GameManager2D.GameState.Playing)
        {
            return;
        }

        // Cache inicializálása ha még nem történt meg
        if (!cacheInitialized)
        {
            InitializeCache();
        }

        HandleSmoothMovement();
    }

    private void HandleSmoothMovement()
    {
        Vector2 input = Vector2.zero;

        // Input
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) input.y += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) input.y -= 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) input.x -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) input.x += 1f;

        if (input != Vector2.zero)
        {
            input.Normalize();
            Vector3 movement = new Vector3(input.x, input.y, 0) * moveSpeed * Time.deltaTime;
            Vector3 newPosition = transform.position + movement;

            // Collision check
            if (CanMoveTo(newPosition))
            {
                transform.position = newPosition;
                if (player2D != null)
                {
                    player2D.position = Vector2Int.RoundToInt(newPosition);
                }
            }
        }
    }

    private bool CanMoveTo(Vector3 targetPosition)
    {
        // Korai return ha nincs cache
        if (!cacheInitialized || cachedMapGenerator == null)
        {
            return true; // Ha nincs map generator, engedjük a mozgást
        }

        // Használjuk a cache-elt értékeket
        Vector2 colliderSize = cachedColliderSize;
        Vector2 colliderOffset = cachedColliderOffset;

        // A collision box középpontja a targetPosition + offset
        float centerX = targetPosition.x + colliderOffset.x;
        float centerY = targetPosition.y + colliderOffset.y;
        float halfWidth = colliderSize.x * 0.5f;
        float halfHeight = colliderSize.y * 0.5f;

        // Ellenőrizzük a collision box 4 sarkát
        // Bal alsó
        Vector3Int cellPos = cachedGroundTilemap != null
            ? cachedGroundTilemap.WorldToCell(new Vector3(centerX - halfWidth, centerY - halfHeight, 0))
            : new Vector3Int(Mathf.FloorToInt(centerX - halfWidth), Mathf.FloorToInt(centerY - halfHeight), 0);
        if (!cachedMapGenerator.IsWalkable(new Vector2Int(cellPos.x, cellPos.y)))
            return false;

        // Jobb alsó
        cellPos = cachedGroundTilemap != null
            ? cachedGroundTilemap.WorldToCell(new Vector3(centerX + halfWidth, centerY - halfHeight, 0))
            : new Vector3Int(Mathf.FloorToInt(centerX + halfWidth), Mathf.FloorToInt(centerY - halfHeight), 0);
        if (!cachedMapGenerator.IsWalkable(new Vector2Int(cellPos.x, cellPos.y)))
            return false;

        // Bal felső
        cellPos = cachedGroundTilemap != null
            ? cachedGroundTilemap.WorldToCell(new Vector3(centerX - halfWidth, centerY + halfHeight, 0))
            : new Vector3Int(Mathf.FloorToInt(centerX - halfWidth), Mathf.FloorToInt(centerY + halfHeight), 0);
        if (!cachedMapGenerator.IsWalkable(new Vector2Int(cellPos.x, cellPos.y)))
            return false;

        // Jobb felső
        cellPos = cachedGroundTilemap != null
            ? cachedGroundTilemap.WorldToCell(new Vector3(centerX + halfWidth, centerY + halfHeight, 0))
            : new Vector3Int(Mathf.FloorToInt(centerX + halfWidth), Mathf.FloorToInt(centerY + halfHeight), 0);
        if (!cachedMapGenerator.IsWalkable(new Vector2Int(cellPos.x, cellPos.y)))
            return false;

        return true; // Minden sarok walkable
    }
}
