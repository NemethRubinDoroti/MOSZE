using UnityEngine;

[System.Serializable]
public class Stats : MonoBehaviour
{
    // Statok inicializálása
    public int maxHealth = 100;
    public int currentHealth = 100;
    public int attack = 15;
    public int defense = 8;
    public int speed = 12;
    public int accuracy = 85;

    // Sebzésődés eljárás
    public void TakeDamage(int damage)
    {
        int actualDamage = Mathf.Max(1, damage - defense);
        currentHealth -= actualDamage;
        currentHealth = Mathf.Max(0, currentHealth);
    }

    // Gyógyulás eljárás
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    // ÉLEK!?
    public bool IsAlive()
    {
        return currentHealth > 0;
    }
}

