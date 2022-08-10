using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEnemy : MonoBehaviour
{
    [Header("Movement")]
    /// <summary>
    /// Character Walk Speed
    /// </summary>
    [SerializeField]
    private float speed = 3.5f;

    [Header("Movement")]
    [SerializeField]
    private GameObject target;

    [Header("Movement")]
    [SerializeField]
    private float personalSpace = 1f;

    [Header("Combat")]
    [SerializeField]
    private GameObject projectilePrefab;

    [Header("Combat")]
    [SerializeField]
    private float attackRange = 3f;

    [Header("Combat")]
    [SerializeField]
    private float fireCooldown = .5f;

    private float nextFire = .0f;

    void Start()
    {
    }

    void FixedUpdate()
    {

        var movement = handleMovement();
        handleAnimation(movement);
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
        if (movement.x < 0)
        {
            transform.localScale = Vector3.one;
        }
        else if (movement.x > 0)
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
            Instantiate(projectilePrefab, transform.position, transform.rotation);
        }
    }
}
