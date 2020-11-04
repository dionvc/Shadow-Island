using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public enum DamageType
    {
        Explosion,
        Poison,
        Ice,
        Physical
    }
    [SerializeField] float health = 80.0f;
    [SerializeField] float maxHealth = 100.0f;
    [SerializeField] float resistanceExplosion = 80;
    HealthBar healthBar;

    private void Start()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        healthBar.Initialize();
        healthBar.UpdateBar(health / maxHealth);
    }

    public void DealDamage(float damage, DamageType type)
    {
        health -= damage;
        healthBar.UpdateBar(health / maxHealth);
        if(health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
