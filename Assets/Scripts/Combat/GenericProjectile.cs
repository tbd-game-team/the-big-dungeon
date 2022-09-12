using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GenericProjectile : MonoBehaviour
{

    public float speed = 40f;
    public float damage = 0f;

    public GameObject explosion;
    private Vector2 screenBounds;

    private void Awake()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }

    void FixedUpdate()
    {
        transform.position += transform.right * speed * Time.deltaTime;

        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.x < -.1 || viewPos.x > 1.1 || viewPos.y < -.1 || viewPos.y > 1.1)
        {
            Destroy(this.gameObject);
        }
    }

    public void OnHitPlayer(Player player)
    {
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
        player.damage(Mathf.CeilToInt(damage));
    }

}
