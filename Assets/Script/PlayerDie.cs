using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDie : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    public float health = 3;
    public float maxHealth;
    public GameObject gameOverCanvas;
    public SpriteRenderer spriteRenderer;
    public static Action onNextLevel;
    public static Action onPlayerDie;
    public GameObject victoryCanvas;

    public bool canBeDamaged = true;


    public bool deathTriggered = false;

    [SerializeField]
    private PlayerMovementScript playerMovementScript;

    public static Action<float> onHPLost;
    private void Start()
    {
        maxHealth = health;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        onHPLost?.Invoke(maxHealth);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        UpdateHealthBar();
        if (health <= 0 && !deathTriggered)
        {
            //Die();
            deathTriggered = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemies") && !playerMovementScript.isDead)
        {
            PlayerLostHP(collision.transform.position.x);

        }
        //if (collision.gameObject.CompareTag("Trap") && !playerMovementController.isDead)
        //{
        //    PlayerLostHP(collision.transform.position.x);
        //}
        //if (collision.gameObject.CompareTag("Flag") && !playerMovementController.isDead)
        //{
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //    FindObjectOfType<AudioManager>().Play("FinishLevel");
        //    onNextLevel?.Invoke();
        //}
    }


    //public void Die()
    //{
    //    FindObjectOfType<AudioManager>().Play("PlayerDeath");
    //    playerMovementController.isDead = true;
    //    anim.SetBool("IsDead", true);
    //    StartCoroutine(ShowGameOverCanvasAfterDelay());
    //    onPlayerDie?.Invoke();
    //    if (victoryCanvas != null)
    //    {
    //        victoryCanvas.SetActive(false);
    //    }
    //}

    //IEnumerator ShowGameOverCanvasAfterDelay()
    //{
    //    yield return new WaitForSeconds(1.75f);
    //    gameOverCanvas.SetActive(true);
    //}

    //private void RestartLevel()
    //{
    //    //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //}
    void UpdateHealthBar()
    {
        float fillAmount = Mathf.Clamp(health / maxHealth, 0, 1);
    }

    private void Die()
    {
        Time.timeScale = 0;
    }

    public void PlayerLostHP(float collisionX)
    {
        if (canBeDamaged == false)
        {
            return;
        }
        canBeDamaged = false;
        BlinkSprites(1f, 0.15f);
        health -= 1;
        PlayerKnockBack(collisionX);
        //FindObjectOfType<AudioManager>().Play("PlayerTakeDamage");
        onHPLost?.Invoke(health);
        if (health <= 0)
        {
            Die();
        }
        Invoke("EnableIphone", 1f);
    }

    public void EnableIphone()
    {
        canBeDamaged = true;
    }

    public void PlayerKnockBack(float collisionX)
    {
        playerMovementScript.KBCounter = playerMovementScript.KBTotalTime;
        if (collisionX <= transform.position.x)
        {
            playerMovementScript.KnockFromRight = false;
        }
        if (collisionX > transform.position.x)
        {
            playerMovementScript.KnockFromRight = true;
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
