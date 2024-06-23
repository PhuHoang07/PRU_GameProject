using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    Rigidbody2D myRigidBody;

    [SerializeField] private float speed = 2f;
    [SerializeField] private float changeDirectionTime = 4f;

    private Rigidbody2D rb;
    private float timer;
    private bool movingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timer = changeDirectionTime;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            movingRight = !movingRight;
            timer = changeDirectionTime;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    void FixedUpdate()
    {
        float moveX = movingRight ? speed : -speed;
        rb.MovePosition(rb.position + new Vector2(moveX, 0f) * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                timer = -timer;
            }
        }
    }

}