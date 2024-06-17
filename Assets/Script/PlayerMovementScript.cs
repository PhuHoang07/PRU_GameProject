using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] float playerHp;
    [SerializeField] bool canPain = true;

    Vector2 moveInput;
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    CapsuleCollider2D myCapsuleCollider;
    BoxCollider2D myFeetCollier;
    float gravityScaleAtStart;
    bool isAlive = true;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myCapsuleCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollier = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidBody.gravityScale;
        playerHp = 10f;
    }

    void Update()
    {
        if (!isAlive)
        {
            return;
        }
        Run();
        ClimbLadder();
        FlipSprite();
        Die();
        Bouncing();
    }

    void ClimbLadder()
    {
        if (!myFeetCollier.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myRigidBody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("IsClimbing", false);
            return;
        }

        Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, moveInput.y * climbSpeed);
        myRigidBody.velocity = climbVelocity;
        myRigidBody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("IsClimbing", playerHasVerticalSpeed);
    }

    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
        }

    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("IsRunning", playerHasHorizontalSpeed);
    }

    void OnJump(InputValue inputValue)
    {
        if (!isAlive)
        {
            return;
        }
        if (!(myFeetCollier.IsTouchingLayers(LayerMask.GetMask("Ground")) || myFeetCollier.IsTouchingLayers(LayerMask.GetMask("Climbing"))))
        {
            return;
        }
        if (inputValue.isPressed)
        {
            myRigidBody.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    void OnMove(InputValue value)
    {
        if (!isAlive)
        {
            return;
        }
        moveInput = value.Get<Vector2>();
    }

    void Die()
    {
        if (playerHp <= 0)
        {
            playerHp = 0;
            Time.timeScale = 0;
        }
        if (myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Enemies")) && canPain)
        {
            StartCoroutine(Pain());
        }
    }

    IEnumerator Pain()
    {
        if (canPain)
        {
            playerHp -= 1;
            canPain = false;
        }
       
        yield return new WaitForSeconds(2.0f);
        canPain = true;
    }

    void Bouncing()
    {
        if (myFeetCollier.IsTouchingLayers(LayerMask.GetMask("Bouncing")) && !(myFeetCollier.IsTouchingLayers(LayerMask.GetMask("Ground"))))
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.bounceSound);
        }
    }
}
