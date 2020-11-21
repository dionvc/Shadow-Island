using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testAI : MonoBehaviour
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
    [SerializeField] float satisfactionRadius = 0.5f;
    [SerializeField] GameObject startMarker;
    [SerializeField] GameObject endMarker;
    Pathing pather = null;
    PathNode currentPath = null;
    Rigidbody2D rb = null;
    List<Object> pathMarkers = new List<Object>();
    [SerializeField] GameObject marker;
    int leftup;
    int leftdown;
    int rightup;
    int rightdown;
    int idleup;
    int idledown;
    int currentState;
    Animator animator;
    Direction direction = Direction.down;
    // Start is called before the first frame update
    void Start()
    {
        pather = Pathing.Instance;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        leftup = Animator.StringToHash("CrabUpLeft");
        leftdown = Animator.StringToHash("CrabDownLeft");
        rightup = Animator.StringToHash("CrabUpRight");
        rightdown = Animator.StringToHash("CrabDownRight");
        idleup = Animator.StringToHash("CrabIdleUp");
        idledown = Animator.StringToHash("CrabIdleDown");
        currentState = idledown;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetMouseButtonDown(2))
        {
            Vector3 end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 start = transform.position;

            currentPath = pather.GetPath(end, start, 500, Mathf.CeilToInt(GetComponent<CircleCollider2D>().radius * 2));
            startMarker.transform.position = new Vector3((int)start.x + 0.5f, (int)start.y, 0);
            endMarker.transform.position = new Vector3((int)end.x + 0.5f, (int)end.y, 0);
            PathNode node = currentPath;
            for(int i = 0; i < pathMarkers.Count; i++)
            {
                Object.Destroy(pathMarkers[i]);
            }
            pathMarkers.Clear();
            if (currentPath != null)
            {
                while (node.parentNode != null)
                {
                    pathMarkers.Add(Instantiate(marker, node.coords + new Vector3(0.5f, 0.0f), Quaternion.identity));
                    node = node.parentNode;
                }
            }
            else
            {
                Debug.Log("Path could not be calculated at all!");
            }
        }
        else if (currentPath != null)
        {
            rb.velocity = 2 * Vector3.Normalize(currentPath.coords - transform.position + new Vector3(0.5f, 0.5f));
            if(Vector3.Magnitude(currentPath.coords - transform.position + new Vector3(0.5f, 0.5f)) < satisfactionRadius)
            {
                currentPath = currentPath.parentNode;
            }
        }
        else
        {
            rb.velocity = new Vector3(0, 0, 0);
        }
        if(rb.velocity.x == 0 && rb.velocity.y > 0)
        {
            if(rb.velocity.y > 0)
            {
                direction = Direction.up;
                ChangeAnimationState(rightup);
            }
            else if(rb.velocity.y < 0)
            {
                direction = Direction.down;
                ChangeAnimationState(rightdown);
            }
        }
        else if(rb.velocity.y == 0 && rb.velocity.x > 0)
        {
            if(rb.velocity.x > 0.1f)
            {
                direction = Direction.right;
                ChangeAnimationState(rightdown);
            }
            else if(rb.velocity.x < -0.1f)
            {
                direction = Direction.left;
                ChangeAnimationState(leftdown);
            }
        }

        else if(rb.velocity.x > 0 && rb.velocity.y > 0)
        {
            direction = Direction.rightup;
            ChangeAnimationState(rightup);
        }
        else if(rb.velocity.x < 0 && rb.velocity.y < 0)
        {
            direction = Direction.leftdown;
            ChangeAnimationState(leftdown);
        }
        else if(rb.velocity.x < 0 && rb.velocity.y > 0)
        {
            direction = Direction.leftup;
            ChangeAnimationState(leftup);
        }
        else if(rb.velocity.y < 0 && rb.velocity.x > 0)
        {
            direction = Direction.rightdown;
            ChangeAnimationState(rightdown);
        }
        else
        {
            if((int)direction >= 5)
            {
                ChangeAnimationState(idleup);
            }
            else
            {
                ChangeAnimationState(idledown);
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
