using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemAmmo : Item
{
    public enum FireType
    {
        Projectile,
        Hitscan
    }

    public enum AmmoType
    {
        Bullet,
        Rocket,
        None //only used for ammo slot (NOT FOR AMMO ITEMS)
    }

    public FireType fireType;
    public AmmoType ammoType;
    public GameObject projectile; //implement projectile class
    public Health.DamageType damageType; //hitscan only
    public float damage;
    public float damageRadius; //the circular area enclosing damage targets
    ContactFilter2D filter;
    Collider2D[] results = new Collider2D[10];

    public void OnFire(Health target, GameObject origin)
    {
        if(fireType == FireType.Hitscan)
        {
            target.DealDamage(damage, damageType);
        }
        else
        {
            GameObject.Instantiate(projectile, origin.transform.position, Quaternion.identity);
            //TODO: set target in projectile class
        }
    }

    public void OnFire(Vector2 target, GameObject origin)
    {
        if(fireType == FireType.Hitscan)
        {
            int resultCount = Physics2D.OverlapCircle(target, damageRadius, filter, results);
            for(int i = 0; i < resultCount; i++)
            {
                Health health;
                if(results[i].TryGetComponent(out health))
                {
                    health.DealDamage(damage, damageType);
                }
            }
        }
        else
        {
            GameObject.Instantiate(projectile, origin.transform.position, Quaternion.identity);
        }
    }

    public void InitializeFilter()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = 1 << LayerMask.NameToLayer("Damageable");
    }
}
