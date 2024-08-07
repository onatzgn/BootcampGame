using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Vector2 InputVector { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsAttacking { get; private set; }

    private bool isGrounded = true;
    private bool canJump = true;

    void Update()
    {
        float h = 0f;
        float v = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            v = 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            v = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            h = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            h = 1f;
        }

        InputVector = new Vector2(h, v);
        
        if (canJump && isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            IsJumping = true;
            canJump = false; 
        }
        //SALDIRI İÇİN SAĞ TIK YAPTIRDIM
        IsAttacking = Input.GetMouseButtonDown(0);
    }
    
    public void SetGroundedState(bool grounded)
    {
        isGrounded = grounded;
        if (grounded)
        {
            canJump = true;
        }
    }
    
    public void FinishJump()
    {
        IsJumping = false;
    }
    
    public void FinishAttack()
    {
        IsAttacking = false;
    }
}