using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerBody;
    [SerializeField] float playerSpeed;
    int up;
    int down;
    int left;
    int right;
    int downright;
    int downleft;

    int idleleft;
    int idleup;
    int idledown;
    int idleright;
    int idledownleft;
    int idledownright;
    int idleupleft;
    int idleupright;
    int currentState;
    Animator animator;
    int moveLeft = 0;
    int moveRight = 0;
    int moveUp = 0;
    int moveDown = 0;
    int prevHoriCode = 0;
    int prevVertCode = -1;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        up = Animator.StringToHash("RobotUp");
        down = Animator.StringToHash("RobotDown");
        left = Animator.StringToHash("RobotLeft");
        right = Animator.StringToHash("RobotRight");
        downleft = Animator.StringToHash("RobotDownLeft");
        downright = Animator.StringToHash("RobotDownRight");

        idleleft = Animator.StringToHash("RobotIdleLeft");
        idleright = Animator.StringToHash("RobotIdleRight");
        idledown = Animator.StringToHash("RobotIdleDown");
        idleup = Animator.StringToHash("RobotIdleUp");
        idleupleft = Animator.StringToHash("RobotIdleUpLeft");
        idleupright = Animator.StringToHash("RobotIdleUpRight");
        idledownleft = Animator.StringToHash("RobotIdleDownLeft");
        idledownright = Animator.StringToHash("RobotIdleDownRight");

        currentState = idledown;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (moveLeft != 0 || moveRight != 0 || moveUp != 0 || moveDown != 0) //move stuff
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
            prevHoriCode = horiCode;
            prevVertCode = vertCode;
        }
        else //idle stuff
        {
            if (prevHoriCode == 1 && prevVertCode == 1)
            {
                ChangeAnimationState(idleupright);
            }
            else if (prevHoriCode == 1 && prevVertCode == 0)
            {
                ChangeAnimationState(idleright);
            }
            else if (prevHoriCode == 1 && prevVertCode == -1)
            {
                ChangeAnimationState(idledownright);
            }
            else if (prevHoriCode == 0 && prevVertCode == -1)
            {
                ChangeAnimationState(idledown);
            }
            else if (prevHoriCode == -1 && prevVertCode == -1)
            {
                ChangeAnimationState(idledownleft);
            }
            else if (prevHoriCode == -1 && prevVertCode == 0)
            {
                ChangeAnimationState(idleleft);
            }
            else if (prevHoriCode == -1 && prevVertCode == 1)
            {
                ChangeAnimationState(idleupleft);
            }
            else if (prevHoriCode == 0 && prevVertCode == 1)
            {
                ChangeAnimationState(idleup);
            }
            playerBody.velocity = Vector2.zero;
        }
        
        moveLeft = 0;
        moveRight = 0;
        moveUp = 0;
        moveDown = 0;
    }

    void Update()
    {
        if(Input.GetKey(PlayerData.Instance.keybinds["Move Up"]))
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
