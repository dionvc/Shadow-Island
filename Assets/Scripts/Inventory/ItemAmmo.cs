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
    public Health.DamageType damageType; //hitscan only - but used for tooltip so should be set for projectiles just for tooltip
    public float damage;
    public float damageRadius; //the circular area enclosing damage targets
    public float firingRate; //base firing rate in terms of ticks to pass per fire - can be modified by multiplier of firing entity
    ContactFilter2D filter;
    [SerializeField] LayerMask damageableLayers;
    Collider2D[] results = new Collider2D[10];

    public void OnFire(Health target, GameObject origin)
    {
        if(fireType == FireType.Hitscan)
        {
            target.DealDamage(damage, damageType, origin);
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
                    health.DealDamage(damage, damageType, origin);
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
        filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = damageableLayers;
    }
}
