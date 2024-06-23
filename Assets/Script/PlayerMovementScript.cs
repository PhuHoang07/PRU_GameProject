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
    public bool canPain;

    public float knockbackDuration = 1f;
    public float knockbackForce = 20f;

    public GameObject gameOverScreen;

    private SpriteRenderer spriteRenderer;
    Vector2 moveInput;
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    CapsuleCollider2D myCapsuleCollider;
    BoxCollider2D myFeetCollider;
    float gravityScaleAtStart;
    bool isAlive;

    private HealthBar healthBar;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myCapsuleCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidBody.gravityScale;
        playerHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        canPain = true;
        isAlive = true;
        if (healthBar == null)
        {
            healthBar = FindObjectOfType<HealthBar>();
        }
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
        Hurt(0);
        Bouncing();
    }

    void ClimbLadder()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
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

        if (!(myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) || myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))))
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

    public void Hurt(int option)
    {
        if (playerHealth == 0)
        {
            isAlive = false;
            gameOverScreen.SetActive(true);
            Time.timeScale = 0;
        }
        if (option == 0)
        {
            if ((myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Enemies")) && canPain))
            {
                StartCoroutine(Pain());
                BlinkSprites(2.0f, 0.15f);
                Knockback();
            }
        }
        else
        {
            StartCoroutine(Pain());
            BlinkSprites(2.0f, 0.15f);
            Knockback();
        }
    }


    IEnumerator Pain()
    {
        if (canPain)
        {
            playerHealth -= 1;
            healthBar.UpdateUI(playerHealth);
            canPain = false;
        }

        yield return new WaitForSeconds(2.0f);
        canPain = true;
    }

    void Bouncing()
    {
        if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Bouncing")) && !(myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))))
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

    private void Knockback()
    {
        Vector2 knockbackDirection = (new Vector2(transform.position.x, transform.position.y) - myCapsuleCollider.ClosestPoint(transform.position)).normalized;
        myRigidBody.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        StartCoroutine(KnockbackCoroutine());
    }

    IEnumerator KnockbackCoroutine()
    {
        float originalGravity = myRigidBody.gravityScale;
        myRigidBody.gravityScale = 0;
        yield return new WaitForSeconds(knockbackDuration);
        myRigidBody.gravityScale = originalGravity;
    }
}
