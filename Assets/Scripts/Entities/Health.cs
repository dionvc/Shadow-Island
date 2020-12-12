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
        Physical,
        Fire,
        Acid
    }
    [SerializeField] float health = 80.0f;
    [SerializeField] float maxHealth = 100.0f;
    [SerializeField] float resistanceExplosion = 80;
    [SerializeField] float resistancePhysical = 80;
    [SerializeField] float resistanceIce = 80;
    [SerializeField] float resistancePoison = 80;
    [SerializeField] float resistanceFire = 80;
    [SerializeField] float resistanceAcid = 80;
    [SerializeField] ParticleSystemPool.ParticleType damageParticle = ParticleSystemPool.ParticleType.none;
    [SerializeField] Color bloodColor = new Color(1, 1, 1);
    HealthBar healthBar;
    public int alliance { get; set; } = 0; //0 is neutral always

    private void Start()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        healthBar.Initialize();
        healthBar.UpdateBar(health / maxHealth);
    }

    public void DealDamage(float damage, DamageType type)
    {
        float resistanceFactor = 0;
        switch(type)
        {
            case (DamageType.Explosion):
                resistanceFactor = resistanceExplosion / 100;
                break;
            case (DamageType.Acid):
                resistanceFactor = resistanceAcid / 100;
                break;
            case (DamageType.Physical):
                resistanceFactor = resistancePhysical / 100;
                break;
            case (DamageType.Poison):
                resistanceFactor = resistancePoison / 100;
                break;
            case (DamageType.Ice):
                resistanceFactor = resistanceIce / 100;
                break;
            case (DamageType.Fire):
                resistanceFactor = resistanceFire / 100;
                break;
        }
        health -= (damage * (1 - resistanceFactor));
        healthBar.UpdateBar(health / maxHealth);
        ParticleSystemPool.Instance.EmitParticle(damageParticle, this.transform.position, 1, true, bloodColor);
        if(health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
