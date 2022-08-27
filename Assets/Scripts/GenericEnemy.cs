using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GenericEnemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float speed = 3.5f;
    [SerializeField]
    public GameObject target;
    [SerializeField]
    private float personalSpace = 1f;

    [Header("Combat")]
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private float attackRange = 3f;
    [SerializeField]
    private float fireCooldown = .5f;

    private float nextFire = .0f;

    private Animator characterAnimator;

    private void Awake()
    {
        characterAnimator = GetComponent<Animator>();
        if (target == null)
        {
            target = GameObject.FindWithTag("Player");
        }
    }

    void Start()
    {
    }

    void Update()
    {
        var movement = handleMovement();
        handleAnimation(movement);
    }

    void FixedUpdate()
    {
        handleCombat();
    }

    Vector3 handleMovement()
    {
        var distance = Vector3.Distance(transform.position, target.transform.position);
        var movement = (target.transform.position - transform.position).normalized * Time.deltaTime * speed;

        if (distance >= attackRange)
        {
            transform.Translate(movement);
        }
        else if (distance < personalSpace)
        {
            transform.Translate(-movement);
        }
        else
        {
            movement = new Vector3(0, 0, 0);
        }
        return movement;
    }

    void handleAnimation(Vector3 movement)
    {
        characterAnimator.SetFloat(Keys.ANIMATION_SPEED_KEY, movement.magnitude / Time.deltaTime);
        if (movement.x > 0)
        {
            transform.localScale = Vector3.one;
        }
        else if (movement.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void handleCombat()
    {
        var distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance < attackRange && Time.time >= nextFire)
        {
            nextFire = Time.time + fireCooldown;

            var projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            // var projectileController = projectile.GetComponent<GenericProjectile>();

            var relative = projectile.transform.InverseTransformPoint(target.transform.position);
            var angle = 90 - Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
            projectile.transform.Rotate(0, 0, angle);
        }
    }
}
