using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

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
    public GameObject enemyHeadCheck;

    private SpriteRenderer spriteRenderer;
    Vector2 moveInput;
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    CapsuleCollider2D myCapsuleCollider;
    BoxCollider2D myFeetCollider;
    float gravityScaleAtStart;
    bool isAlive;
    bool isControllable;

    private HealthBar healthBar;

    public bool isInWater;
    public float timeInWater;
    private float waterThreshold = 3f;

    public bool isOnLadder = false;

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
        isControllable = true;
        isInWater = false;
        enemyHeadCheck.SetActive(false);
        if (healthBar == null)
        {
            healthBar = FindObjectOfType<HealthBar>();
        }
    }

    void Update()
    {
        if (!isAlive || !isControllable)
        {
            return;
        }
        Run();
        ClimbLadder();
        FlipSprite();
        Hurt(0);
        Bouncing();
    }

    private void FixedUpdate()
    {
        if (isInWater)
        {
            timeInWater += Time.fixedDeltaTime;
            if (timeInWater >= waterThreshold)
            {
                timeInWater = 0f;
                Hurt(1);
            }
        }
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
        enemyHeadCheck.SetActive(false);
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
        if (!isAlive || !isControllable)
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
            enemyHeadCheck.SetActive(true);
            Debug.Log("jump");
        }
    }

    void OnMove(InputValue value)
    {
        if (!isAlive || !isControllable)
        {
            moveInput = Vector2.zero;
            return;
        }
        moveInput = value.Get<Vector2>();
    }

    private void GameOver()
    {
        isAlive = false;
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void Hurt(int option)
    {
        if (playerHealth == 0)
        {
            GameOver();
        }
        else
        {
            if (option == 0)
            {
                if ((myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Enemies")) && canPain))
                {
                    StartCoroutine(Pain());
                    BlinkSprites(2.0f, 0.15f);
                    StartCoroutine(DisableMovementForSeconds(0.5f));
                }
            }
            else
            {
                StartCoroutine(Pain());
                BlinkSprites(2.0f, 0.15f);
            }
        }

    }

    IEnumerator DisableMovementForSeconds(float seconds)
    {
        isControllable = false;
        myAnimator.SetBool("IsRunning", false);
        myAnimator.SetBool("IsClimbing", false);
        myRigidBody.velocity = Vector2.zero;
        moveInput = Vector2.zero;
        yield return new WaitForSeconds(seconds);
        isControllable = true;
    }

    IEnumerator Pain()
    {
        if (canPain)
        {
            playerHealth -= 1;
            healthBar.UpdateUI(playerHealth);
            if (playerHealth == 0)
            {
                GameOver();
                yield break;
            }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int layer = collision.gameObject.layer;
        if (layer == LayerMask.NameToLayer("Ground"))
        {
            if (!myAnimator.GetBool("IsClimbing"))
            {
                Debug.Log("touch ground");
                enemyHeadCheck.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int layer = collision.gameObject.layer;
        
        if (layer == LayerMask.NameToLayer("Climbing") && myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            Debug.Log("on ladder");
            enemyHeadCheck.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        int layer = collision.gameObject.layer;

        if (layer == LayerMask.NameToLayer("Climbing"))
        {
            if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && !myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
            {
                Debug.Log("out of ladder");
                enemyHeadCheck.SetActive(true);
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            isInWater = false;
            timeInWater = 0f;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            isInWater = true;
        }
    }
}
