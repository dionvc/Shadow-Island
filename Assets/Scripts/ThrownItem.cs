using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownItem : MonoBehaviour
{
    private enum ThrownState {
        Moving,
        Landed
    }
    [SerializeField] float damage = 20;
    [SerializeField] float damageRange = 5;
    [SerializeField] int damageLength = 1;
    [SerializeField] Health.DamageType damageType = Health.DamageType.Explosion;
    [SerializeField] LayerMask layermask;
    ContactFilter2D filter = new ContactFilter2D();
    ThrownState thrownState = ThrownState.Moving;
    [SerializeField] float thrownVelocity = 0.25f;
    Vector3 target;
    List<Collider2D> damageList = new List<Collider2D>(10);

    // Update is called once per frame
    void Update()
    {
        switch (thrownState) {
            case ThrownState.Moving:    
                Vector3 distance = (target - this.transform.position);
                if (distance.magnitude < 2 * thrownVelocity)
                {
                    thrownState = ThrownState.Landed;
                    filter.useLayerMask = true;
                    filter.layerMask = layermask;
                    this.GetComponent<SpriteRenderer>().enabled = false;
                }
                this.transform.position += (target - this.transform.position).normalized * thrownVelocity;
                break;
            case ThrownState.Landed:
                //get objects to damage
                Physics2D.OverlapCircle(this.transform.position, damageRange, filter, damageList);
                //deal damage
                Health health;
                for (int i = 0; i < damageList.Count; i++)
                {
                    if(damageList[i].gameObject.TryGetComponent(out health))
                    {
                        health.DealDamage(damage, damageType);
                    }
                }
                ParticleSystemPool.Instance.EmitExplosion(this.transform.position);
                damageLength--;
                if(damageLength <= 0)
                {
                    Destroy(this);
                }
                break;
        }
    }

    public void SetTarget(Vector3 target)
    {
        this.target = target;
    }
}
