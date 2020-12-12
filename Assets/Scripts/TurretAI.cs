using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAI : MonoBehaviour
{
    enum TurretState
    {
        turningToTarget, //turning is allowed but not firing
        hasTarget, //turning is allowed and firing
        coolingdown //turning is allowed but not firing
    }

    [SerializeField] int animationPropertyIndex = 0;
    [SerializeField] float attackRange = 10.0f;
    [SerializeField] float damagePerHit = 1.0f;
    [SerializeField] LayerMask mask;
    [SerializeField] Vector2 flareOffset;
    [SerializeField] Vector2 flareDistance;
    [SerializeField] ParticleSystemPool.ParticleType flareType;
    [SerializeField] Health.DamageType damageType;
    Collider2D[] results;
    Health target;
    AmmoSlot ammoInventory;
    int[] hashCodes;
    float turretRotation = 0; //0 corresponds to right and increasing moves counter clockwise so turretrotation/45 -> correct index for hashcodes
    float angleDivider = 45.0f;
    int currentState = 0;
    Animator animator;
    int counter = 0;
    [SerializeField] int firingRate = 25;
    // Start is called before the first frame update
    void Start()
    {
        results = new Collider2D[24];
        hashCodes = AnimationPropertiesPool.Instance.animationDictionary[animationPropertyIndex].animationHashCodes;
        animator = GetComponent<Animator>();
        currentState = hashCodes[0];
        target = null;
        angleDivider = 360.0f / hashCodes.Length;
        ammoInventory = GetComponent<AmmoSlot>();
        animator.speed = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target == null || target.gameObject == null)
        {
            int count = Physics2D.OverlapCircleNonAlloc(this.transform.position, attackRange, results, mask);
            for(int i = 0; i < count; i++)
            {
                if(results[i].TryGetComponent(out target) && target.alliance != gameObject.GetComponent<Health>().alliance)
                {
                    break;
                }
                else
                {
                    target = null;
                }
            }
        }
        else
        {
            Vector2 targetVector = target.transform.position - this.transform.position;
            counter++;
            animator.speed = 15.0f / firingRate;
            if (targetVector.magnitude > attackRange)
            {
                target = null;
                animator.speed = 0.0f;
            }
            else if(ammoInventory.inventoryReadOnly[0] != null && counter > firingRate)
            {
                //change this to interface with ammo item (ammo item should have some sort of onfire method which either does a hitscan or spawns
                //a projectile, there also needs to be some sort of check to make sure ammo inserted is correct ammo for turret)

                ItemAmmo ammo = ammoInventory.inventoryReadOnly[0].item as ItemAmmo;
                if (ammo != null)
                {
                    ammo.OnFire(target, this.gameObject);
                    ammoInventory.DecrementStack(0);
                    counter = 0;
                    turretRotation = Mathf.Rad2Deg * Mathf.Atan2(targetVector.y, targetVector.x);

                    if (turretRotation < 0)
                    {
                        turretRotation += 360;
                    }
                    int rotationIncrement = Mathf.RoundToInt(turretRotation / angleDivider);
                    int rotationSnap = rotationIncrement * 45;
                    ParticleSystemPool.Instance.EmitParticle(flareType, ((Vector2)this.transform.position + flareOffset) + (Vector2)(Quaternion.AngleAxis(rotationSnap, Vector3.forward) * flareDistance), -rotationSnap, 1);
                    ChangeAnimationState(hashCodes[rotationIncrement % hashCodes.Length]);
                }
            }
            else if(ammoInventory.inventoryReadOnly[0] == null)
            {
                animator.speed = 0;
            }
        }
    }

    void ChangeAnimationState(int state)
    {
        if (state == currentState)
        {
            return;
        }
        animator.Play(state);
        currentState = state;
    }
}
