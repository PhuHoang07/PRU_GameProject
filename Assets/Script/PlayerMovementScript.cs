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

    private float maxHealth = 3;
    public float playerHealth;
    public float KBForce;
    public float KBCounter;
    public float KBTotalTime;
    public float horizontal;
    public bool canPain;

    public bool KnockFromRight;
    public bool isDead;

    public SpriteRenderer spriteRenderer;
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
        playerHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        canPain = true;
    }

    void Update()
    {
        if (!isAlive)
        {
            return;
        }
        horizontal = Input.GetAxis("Horizontal");
        Run();
        ClimbLadder();
        FlipSprite();
        Die();
        Bouncing();
    }

    private void FixedUpdate()
    {
        if (KBCounter <= 0)
        {
            myRigidBody.velocity = new Vector2(horizontal * runSpeed, myRigidBody.velocity.y);
        }
        else
        {
            if (KnockFromRight == true)
            {
                myRigidBody.velocity = new Vector2(-KBForce, KBForce);
            }
            if (KnockFromRight == false)
            {
                myRigidBody.velocity = new Vector2(KBForce, KBForce);
            }

            KBCounter -= Time.deltaTime;
        }
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
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            Time.timeScale = 0;
        }
        if (myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Enemies")) && canPain)
        {
            StartCoroutine(Pain());
            BlinkSprites(1.0f, 0.15f);
        }
    }

    IEnumerator Pain()
    {
        if (canPain)
        {
            playerHealth -= 1;
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

    public void BlinkSprites(float duration, float timeBtwBlinks)
    {
        StartCoroutine(BlinderSpritesCoroutine(duration, timeBtwBlinks));
    }

    IEnumerator BlinderSpritesCoroutine(float duration, float timeBtwBlinks)
    {
        float elapsedTime = 0f;
        float blinkElapsed = timeBtwBlinks;
        bool visible = false;
        while (elapsedTime < duration)
        {
            if (blinkElapsed >= timeBtwBlinks)
            {
                ToggleSprites(visible);
                visible = !visible;
                blinkElapsed = 0f;
            }
            blinkElapsed += Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        ToggleSprites(true);
    }

    private void ToggleSprites(bool visible)
    {
        spriteRenderer.enabled = visible;
    }
}
