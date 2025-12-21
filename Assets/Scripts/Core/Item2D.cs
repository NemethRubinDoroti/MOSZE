using UnityEngine;
using UnityEngine.Tilemaps;

public class Item2D : MonoBehaviour
{
    public Vector2Int position;
    public SpriteRenderer spriteRenderer;
    
    public enum ItemType
    {
        Heal,      // +50 HP a játékosnak (harc közben használható)
        Weapon,    // +10% permanent attack boost (összegyűjtéskor aktiválódik)
        Defense,   // +10% permanent defense boost (összegyűjtéskor aktiválódik)
        Speed,     // +10% permanent speed boost (összegyűjtéskor aktiválódik)
        Accuracy,  // +10% permanent accuracy boost (összegyűjtéskor aktiválódik)
        Treasure   // +200 pont (instant)
    }
    
    public ItemType itemType;
    
    [Header("Visuals")]
    public Sprite itemSprite;
    public int sortingOrder = 3;
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        // Sprite beállítása ha van
        if (itemSprite != null)
        {
            spriteRenderer.sprite = itemSprite;
        }
        
        spriteRenderer.sortingOrder = sortingOrder;
        
        position = Vector2Int.RoundToInt(transform.position);
        
        // Vizuális pozíció frissítése
        UpdateVisualPosition();
    }
    
    private void Update()
    {
        Vector2Int currentPos = Vector2Int.RoundToInt(transform.position);
        if (currentPos != position)
        {
            position = currentPos;
            UpdateVisualPosition();
        }
    }
    
    private void UpdateVisualPosition()
    {
        if (GameManager2D.Instance != null && GameManager2D.Instance.mapGenerator != null)
        {
            Tilemap groundTilemap = GameManager2D.Instance.mapGenerator.groundTilemap;
            if (groundTilemap != null)
            {
                Vector3 worldPos = groundTilemap.CellToWorld(new Vector3Int(position.x, position.y, 0));
                transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(position.x, position.y, transform.position.z);
            }
        }
        else
        {
            transform.position = new Vector3(position.x, position.y, transform.position.z);
        }
    }
    

    public void OnCollected()
    {
        if (ItemManager.Instance != null)
        {
            ItemManager.Instance.CollectItem(this);
        }
        
        // Gyűjtés hang lejátszása
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCollect();
        }
        
        Destroy(gameObject);
    }
}

