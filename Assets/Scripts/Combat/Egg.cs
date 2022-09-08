using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class Egg : MonoBehaviour
{
    [SerializeField]
    private float fireCooldown = 8f;

    [SerializeField]
    private GameObject explosionPrefab;

    [SerializeField]
    private GameObject enemyPrefab;

    private float nextFire;

    private Animator characterAnimator;

    void Awake()
    {
        characterAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        nextFire = Time.time + fireCooldown;
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        var eggBreakAnimTime = 2;
        if (Time.time >= nextFire)
        {
            var enemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
            var enemyController = enemy.GetComponent<GenericEnemy>();
            enemyController.target = GameObject.FindWithTag("Player");

            Destroy(this.gameObject);
        }
        else if (Time.time >= nextFire - eggBreakAnimTime)
        {
            characterAnimator.SetBool(Keys.ANIMATION_CRACK_KEY, true);
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
