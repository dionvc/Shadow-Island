using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAI : MonoBehaviour
{
    enum Direction
    {
        down = 0,
        rightdown = 1,
        left = 2,
        right = 3,
        leftdown = 4,
        up = 5,
        leftup = 6,
        rightup = 7
    }
    enum AIState
    {
        MovingFormation,
        Pursuing,
        Idle
    }
    enum ActionState
    {
        Attacking,
        Moving
    }

    //variables for agent behavior
    [SerializeField] float satisfactionRadius = 0.5f;
    [SerializeField] float stuckSensitivity = 2.0f;
    [SerializeField] float entitySpeed = 3.0f;
    [SerializeField] float pursueDistance = 12.0f;
    [SerializeField] float pursueEuclidian = 3.0f;
    [SerializeField] int checkFrequency = 50;
    [SerializeField] float attackDistance = 1.0f;
    [SerializeField] float attackDamage = 5.0f;
    [SerializeField] int attackSpacing = 10;
    [SerializeField] int attackLength = 50;
    [SerializeField] Health.DamageType attackType;
    ContactFilter2D attackFilter;
    ContactFilter2D filter;
    
    //formation tracking
    PathNode formationPath = null;
    PathNode pathToFormation = null;

    //movement and alliance
    Rigidbody2D rb = null;
    Alliance alliance;
    Vector3 stuckCheck;
    Collider2D[] checkAreaResults;


    //counter for checking nearby
    int counter = 0;
    int attackCounter = 0;

    //states
    AIState aiState = AIState.Idle;
    ActionState actionState = ActionState.Moving;

    //personal path is path for self when pursuing
    PathNode personalPath = null;
    GameObject pursueTarget;
    int agentSize = 1;
    #region display
    int leftup;
    int leftdown;
    int rightup;
    int rightdown;
    int idleup;
    int idledown;
    int currentState;
    int attackup;
    int attackdown;
    Animator animator;
    Direction direction = Direction.down;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        leftup = Animator.StringToHash("CrabUpLeft");
        leftdown = Animator.StringToHash("CrabDownLeft");
        rightup = Animator.StringToHash("CrabUpRight");
        rightdown = Animator.StringToHash("CrabDownRight");
        idleup = Animator.StringToHash("CrabIdleUp");
        idledown = Animator.StringToHash("CrabIdleDown");
        attackdown = Animator.StringToHash("CrabAttackDown");
        attackup = Animator.StringToHash("CrabAttackUp");
        currentState = idledown;
        stuckCheck = this.transform.position;
        attackFilter = new ContactFilter2D();
        attackFilter.useLayerMask = true;
        attackFilter.layerMask = (1 << LayerMask.NameToLayer("Damageable")) | (1 << LayerMask.NameToLayer("ConveyorBelt"));
        filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = 1 << LayerMask.NameToLayer("Damageable");
        agentSize = Mathf.CeilToInt(GetComponent<CircleCollider2D>().radius * 2);
        checkAreaResults = new Collider2D[32];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (actionState == ActionState.Attacking)
        {
            rb.velocity = Vector3.zero;
            attackCounter++;
            if (attackCounter % attackSpacing == 0)
            {
                int attackHits = Physics2D.OverlapCircle(this.transform.position, attackDistance, attackFilter, checkAreaResults);
                for (int i = 0; i < attackHits; i++)
                {
                    Health health = checkAreaResults[i].GetComponent<Health>();
                    if (health.alliance != alliance.allianceCode)
                    {
                        health.DealDamage(attackDamage, attackType, this.gameObject);
                    }
                }
            }
            if(attackCounter >= attackLength)
            {
                actionState = ActionState.Moving;
                attackCounter = 0;
            }
        }
        else if (actionState == ActionState.Moving)
        {
            if (aiState == AIState.Idle)
            {
                rb.velocity = Vector2.zero;
                if(formationPath != null && pathToFormation == null && (formationPath.coords - this.transform.position + new Vector3(0.5f, 0.5f)).magnitude > satisfactionRadius)
                {
                    pathToFormation = Pathing.Instance.GetPath(this.transform.position, formationPath.coords, 500, agentSize);
                }
                else if(formationPath != null && pathToFormation != null)
                {
                    aiState = AIState.MovingFormation;
                }

                //search for nearby targets otherwise remain idle/search for nearby formation and join if there is
                //or if something has attacked it then set that to pursuetarget
                counter++;
                if (counter % checkFrequency == 0)
                {
                    int nearby = Physics2D.OverlapCircle(this.transform.position, pursueDistance, filter, checkAreaResults);
                    for (int i = 0; i < nearby; i++)
                    {
                        if (alliance.hostileTowards.Contains(checkAreaResults[i].GetComponent<Health>().alliance))
                        {
                            aiState = AIState.Pursuing;
                            pursueTarget = checkAreaResults[i].gameObject;
                        }
                    }
                }
                
            }
            else if (aiState == AIState.Pursuing)
            {
                //use direction vector to target if target is close else use pathing
                //remain pursuing unless target is far away in which case path to last formation node and then continue along formation
                
                if (pursueTarget != null)
                {
                    Vector2 pursueVector = (pursueTarget.transform.position - this.transform.position);
                    if (pursueVector.magnitude < pursueDistance)
                    {
                        if (pursueVector.magnitude < attackDistance)
                        {
                            actionState = ActionState.Attacking;
                            rb.velocity = Vector3.zero;
                        }
                        else if (pursueVector.magnitude < pursueEuclidian)
                        {
                            rb.velocity = entitySpeed * pursueVector.normalized;
                            personalPath = null;
                        }
                        else if (personalPath != null)
                        {
                            Vector2 moveVector = Vector3.Normalize(personalPath.coords - transform.position + new Vector3(0.5f, 0.5f));
                            rb.velocity = entitySpeed * moveVector;
                            if (Vector3.Magnitude(personalPath.coords - transform.position + new Vector3(0.5f, 0.5f)) < satisfactionRadius)
                            {
                                personalPath = personalPath.nextNode;
                            }
                        }
                        else
                        {
                            personalPath = Pathing.Instance.GetPath(this.transform.position, pursueTarget.transform.position, 100, agentSize);
                        }
                    }
                    else
                    {
                        personalPath = null;
                        aiState = AIState.Idle;
                    }
                }
                else
                {
                    personalPath = null;
                    aiState = AIState.Idle;
                }
            }
            else if (aiState == AIState.MovingFormation)
            {
                if(pathToFormation != null && formationPath != null) //pathing to formation if it is not near formation
                {
                    Vector2 moveVector = Vector3.Normalize(pathToFormation.coords - transform.position + new Vector3(0.5f, 0.5f));
                    rb.velocity = entitySpeed * moveVector;
                    if (Vector3.Magnitude(pathToFormation.coords - transform.position + new Vector3(0.5f, 0.5f)) < satisfactionRadius)
                    {
                        pathToFormation = pathToFormation.nextNode;
                    }
                }
                else if (formationPath != null) //continue along formation path
                {
                    Vector2 moveVector = Vector3.Normalize(formationPath.coords - transform.position + new Vector3(0.5f, 0.5f));
                    rb.velocity = entitySpeed * moveVector;
                    if (Vector3.Magnitude(formationPath.coords - transform.position + new Vector3(0.5f, 0.5f)) < satisfactionRadius)
                    {
                        formationPath = formationPath.nextNode;
                    }
                }
                else
                {
                    //if no more path then switch to idle
                    rb.velocity = new Vector3(0, 0, 0);
                    aiState = AIState.Idle;
                }

                //check for targets nearby, if target found then switch to pursuing
                counter++;
                if (counter % checkFrequency == 0)
                {
                    int nearby = Physics2D.OverlapCircle(this.transform.position, pursueDistance, filter, checkAreaResults);
                    float closest = 50;
                    for(int i = 0; i < nearby; i++)
                    {
                        float dist = (checkAreaResults[i].transform.position - this.transform.position).magnitude;
                        if (alliance.hostileTowards.Contains(checkAreaResults[i].GetComponent<Health>().alliance) &&
                            dist < closest)
                        {
                            aiState = AIState.Pursuing;
                            pursueTarget = checkAreaResults[i].gameObject;
                            closest = dist;
                        }
                    }
                }
                //check if was attacked recently and notify formation
                //if stuck on neutral object try moving perpendicular
                //if still stuck then destroy

            }
        }


        #region display logic
        if (actionState == ActionState.Moving)
        {
            if (rb.velocity.x == 0 && rb.velocity.y > 0)
            {
                if (rb.velocity.y > 0)
                {
                    direction = Direction.up;
                    ChangeAnimationState(rightup);
                }
                else if (rb.velocity.y < 0)
                {
                    direction = Direction.down;
                    ChangeAnimationState(rightdown);
                }
            }
            else if (rb.velocity.y == 0 && rb.velocity.x > 0)
            {
                if (rb.velocity.x > 0.1f)
                {
                    direction = Direction.right;
                    ChangeAnimationState(rightdown);
                }
                else if (rb.velocity.x < -0.1f)
                {
                    direction = Direction.left;
                    ChangeAnimationState(leftdown);
                }
            }

            else if (rb.velocity.x > 0 && rb.velocity.y > 0)
            {
                direction = Direction.rightup;
                ChangeAnimationState(rightup);
            }
            else if (rb.velocity.x < 0 && rb.velocity.y < 0)
            {
                direction = Direction.leftdown;
                ChangeAnimationState(leftdown);
            }
            else if (rb.velocity.x < 0 && rb.velocity.y > 0)
            {
                direction = Direction.leftup;
                ChangeAnimationState(leftup);
            }
            else if (rb.velocity.y < 0 && rb.velocity.x > 0)
            {
                direction = Direction.rightdown;
                ChangeAnimationState(rightdown);
            }
            else
            {
                if ((int)direction >= 5)
                {
                    ChangeAnimationState(idleup);
                }
                else
                {
                    ChangeAnimationState(idledown);
                }
            }
        }
        else if(actionState == ActionState.Attacking)
        {
            if ((int)direction >= 5)
            {
                ChangeAnimationState(attackup);
            }
            else
            {
                ChangeAnimationState(attackdown);
            }
        }
        #endregion
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

    public void SetAlliance(Alliance alliance)
    {
        this.alliance = alliance;
        this.GetComponent<Health>().alliance = alliance.allianceCode;
    }
    public void SetFormationPath(PathNode path)
    {
        pathToFormation = Pathing.Instance.GetPath(this.transform.position, path.coords, 200, GetAgentSize());
        formationPath = path;
    }

    public int GetAgentSize()
    {
        return Mathf.CeilToInt(GetComponent<CircleCollider2D>().radius * 2);
    }
}
