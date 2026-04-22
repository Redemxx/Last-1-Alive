using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int basehealth;

    private int health;
    public Action<GameObject> onDeath;
    public Action<GameObject> onDamaged;

    void Start()
    {
        health = basehealth;
    }

    public void TakeDamage(GameObject source, int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            onDeath.Invoke(source);
            return;
        }

        onDamaged?.Invoke(source);
    }

    public int GetHealth()
    {
        return health;
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > basehealth)
        {
            health = basehealth;
        }
    }
}
