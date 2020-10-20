using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerBody;
    [SerializeField] float playerSpeed;
    int up;
    int down;
    int idle;
    int left;
    int right;
    int currentState;
    Animator animator;
    int moveLeft = 0;
    int moveRight = 0;
    int moveUp = 0;
    int moveDown = 0;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        up = Animator.StringToHash("RobotWalkUp");
        down = Animator.StringToHash("RobotWalkDown");
        left = Animator.StringToHash("RobotWalkLeft");
        right = Animator.StringToHash("RobotWalkRight");
        idle = Animator.StringToHash("RobotIdle");
        currentState = idle;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float hori = (moveLeft + moveRight) * playerSpeed;
        float vert = (moveUp + moveDown) * playerSpeed;
        playerBody.velocity = new Vector3(hori , vert);
        if (Mathf.Abs(vert) >= Mathf.Abs(hori) && vert != 0)
        {
            if (vert > 0)
            {
                ChangeAnimationState(up);
            }
            else
            {
                ChangeAnimationState(down);
            }
        }
        else if (Mathf.Abs(hori) >= Mathf.Abs(vert) && hori != 0)
        {
            if (hori < 0)
            {
                ChangeAnimationState(left);
            }
            else
            {
                ChangeAnimationState(right);
            }
        }
        else
        {
            ChangeAnimationState(idle);
        }
        moveLeft = 0;
        moveRight = 0;
        moveUp = 0;
        moveDown = 0;
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            moveUp = 1;
        }
        if(Input.GetKey(KeyCode.S))
        {
            moveDown = -1;
        }
        if(Input.GetKey(KeyCode.A))
        {
            moveLeft = -1;
        }
        if(Input.GetKey(KeyCode.D))
        {
            moveRight = 1;
        }
    }

    void ChangeAnimationState(int state)
    {
        if(state == currentState)
        {
            return;
        }
        animator.Play(state);
        currentState = state;
    }
}
