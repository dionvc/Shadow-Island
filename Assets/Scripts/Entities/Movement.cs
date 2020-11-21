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
    int downright;
    int downleft;
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
        up = Animator.StringToHash("RobotUp");
        down = Animator.StringToHash("RobotDown");
        left = Animator.StringToHash("RobotLeft");
        right = Animator.StringToHash("RobotRight");
        idle = Animator.StringToHash("RobotIdle");
        downleft = Animator.StringToHash("RobotDownLeft");
        downright = Animator.StringToHash("RobotDownRight");
        currentState = idle;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int horiCode = moveLeft + moveRight;
        int vertCode = moveUp + moveDown;
        playerBody.velocity = new Vector3(horiCode * playerSpeed, vertCode * playerSpeed);
        if (horiCode == 1 && vertCode == 1)
        {
            //up right
        }
        else if (horiCode == 1 && vertCode == 0)
        {
            ChangeAnimationState(right);
        }
        else if (horiCode == 1 && vertCode == -1)
        {
            ChangeAnimationState(downright);
        }
        else if (horiCode == 0 && vertCode == -1)
        {
            ChangeAnimationState(down);
        }
        else if (horiCode == -1 && vertCode == -1)
        {
            ChangeAnimationState(downleft);
        }
        else if (horiCode == -1 && vertCode == 0)
        {
            ChangeAnimationState(left);
        }
        else if (horiCode == -1 && vertCode == 1)
        {
            //up left
        }
        else if (horiCode == 0 && vertCode == 1)
        {
            ChangeAnimationState(up);
        }
        
        moveLeft = 0;
        moveRight = 0;
        moveUp = 0;
        moveDown = 0;
    }

    void Update()
    {
        if(Input.GetKey(PlayerData.Instance.keybinds["Move up"]))
        {
            moveUp = 1;
        }
        if(Input.GetKey(PlayerData.Instance.keybinds["Move Down"]))
        {
            moveDown = -1;
        }
        if(Input.GetKey(PlayerData.Instance.keybinds["Move Left"]))
        {
            moveLeft = -1;
        }
        if (Input.GetKey(PlayerData.Instance.keybinds["Move Right"]))
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
