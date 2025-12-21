using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Item Sprites")]
    public Sprite healSprite;
    public Sprite weaponSprite;
    public Sprite defenseSprite;
    public Sprite speedSprite;
    public Sprite accuracySprite;
    public Sprite treasureSprite;
    
    private List<Item2D> spawnedItems = new List<Item2D>();
    
    public Item2D SpawnItem(Item2D.ItemType type, Vector2Int position)
    {
        // Dinamikusan létrehozunk egy új GameObject-et
        GameObject itemObj = new GameObject($"Item_{type}_{position.x}_{position.y}");
        itemObj.transform.position = new Vector3(position.x, position.y, 0);
        
        Item2D item = itemObj.GetComponent<Item2D>();
        if (item == null)
        {
            item = itemObj.AddComponent<Item2D>();
        }
        
        // Beállítjuk a típust és pozíciót
        item.itemType = type;
        item.position = position;
        
        // Beállítjuk a sprite-ot típus szerint
        Sprite spriteToUse = GetSpriteForType(type);
        if (spriteToUse != null)
        {
            item.itemSprite = spriteToUse;
            if (item.spriteRenderer != null)
            {
                item.spriteRenderer.sprite = spriteToUse;
            }
        }
        
        spawnedItems.Add(item);
        
        return item;
    }
    
    private Sprite GetSpriteForType(Item2D.ItemType type)
    {
        switch (type)
        {
            case Item2D.ItemType.Heal:
                return healSprite;
            case Item2D.ItemType.Weapon:
                return weaponSprite;
            case Item2D.ItemType.Defense:
                return defenseSprite;
            case Item2D.ItemType.Speed:
                return speedSprite;
            case Item2D.ItemType.Accuracy:
                return accuracySprite;
            case Item2D.ItemType.Treasure:
                return treasureSprite;
            default:
                return null;
        }
    }
    
    public void ClearAllItems()
    {
        foreach (Item2D item in spawnedItems)
        {
            if (item != null && item.gameObject != null)
            {
                Destroy(item.gameObject);
            }
        }
        spawnedItems.Clear();
    }
    
    public List<Item2D> GetSpawnedItems()
    {
        spawnedItems.RemoveAll(i => i == null);
        return spawnedItems;
    }
    
    public int GetItemCount()
    {
        return GetSpawnedItems().Count;
    }
}

