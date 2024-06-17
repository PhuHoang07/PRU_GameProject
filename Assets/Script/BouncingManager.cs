using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingManager : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.bounceSound);
        }
    }

    
}
