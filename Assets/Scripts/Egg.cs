using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Egg : MonoBehaviour
{
    [SerializeField]
    private float fireCooldown = 8f;

    [SerializeField]
    private GameObject explosionPrefab;

    [SerializeField]
    private GameObject enemyPrefab;

    private float nextFire;

    void Start()
    {
        nextFire = Time.time + fireCooldown;
    }

    void FixedUpdate()
    {
        if (Time.time >= nextFire)
        {
            Instantiate(enemyPrefab, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    }
}
