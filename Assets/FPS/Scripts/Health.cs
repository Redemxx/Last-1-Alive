using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float basehealth;

    private float health;
    public Action<GameObject> onDeath;
    public Action<GameObject> onDamaged;

    void Start()
    {
        health = basehealth;
    }

    public void TakeDamage(GameObject source, float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            onDeath.Invoke(source);
            return;
        }

        onDamaged?.Invoke(source);
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetBaseHealth()
    {
        return basehealth;
    }

    public void Heal(float amount)
    {
        health += amount;
        if (health > basehealth)
        {
            health = basehealth;
        }
    }

    public void ApplyModifier(float modifier)
    {
        basehealth = basehealth * (1f + modifier);
        basehealth = Mathf.CeilToInt(basehealth);
        health = basehealth;
    }
}
