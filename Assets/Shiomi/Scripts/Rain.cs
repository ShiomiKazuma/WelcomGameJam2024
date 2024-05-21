﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour
{
    [SerializeField] float _focePower = 100f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            GameObject player = collision.gameObject;
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            SpriteRenderer sprite = player.GetComponent<SpriteRenderer>();
            // ノックバック処理
            if (!sprite.flipX)
            {
                rb.AddForce(transform.right * _focePower);
            }
            else
            {
                rb.AddForce(-transform.right * _focePower);
            }
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }
}
