using UnityEngine;

public class Combatant
{
    public Vector2Int position;
    public Stats stats;
    public bool isPlayer;
    public bool isAlive;
    public string id;
    
    public Combatant(Vector2Int position, Stats stats, bool isPlayer, string id)
    {
        this.position = position;
        this.stats = stats;
        this.isPlayer = isPlayer;
        this.id = id;
        this.isAlive = true;
    }
    
    public void TakeDamage(int damage)
    {
        if (!isAlive) return;
        
        stats.TakeDamage(damage);
        if (!stats.IsAlive())
        {
            isAlive = false;
        }
    }
    
    public void Heal(int amount)
    {
        if (!isAlive) return;
        stats.Heal(amount);
    }
    
    public void Move(Vector2Int newPosition)
    {
        if (!isAlive) return;
        position = newPosition;
    }
    
    public int Attack(Combatant target)
    {
        if (!isAlive || !target.isAlive) return 0;
        
        // Találat ellenőrzése, majd sebzés számítása
        int damage = 0;
        if (Random.Range(0, 100) < stats.accuracy)
        {
            damage = Mathf.Max(1, stats.attack - target.stats.defense);
            target.TakeDamage(damage);
        }
        
        return damage;
    }
    
    // Ez így csúnya :(
    public int CalculateDistance(Combatant other)
    {
        return GridUtils.CalculateDistance(position, other.position);
    }
}

