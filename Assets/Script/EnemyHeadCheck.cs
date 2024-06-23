using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeadCheck : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = player.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerCheck>())
        {
            rb.velocity = new Vector2(rb.velocity.x, 40);
            rb.AddForce(Vector2.up * 300f);
        }
    }
}
