using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Enemy : GenericEnemy
{
    [Header("Movement")]
    [SerializeField]
    private float speed = 3.5f;
    [SerializeField]
    private float personalSpace = 1f;

    [Header("Combat")]
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private float attackRange = 1f;
    [SerializeField]
    private float fireCooldown = .5f;
    [SerializeField]
    private float attackDmg = 1f;
    [SerializeField]
    private float prjSpeed = 1f;

    [SerializeField]
    private AudioSource footsteps;

    public int[,] map;
    public int mapWidth;
    public int mapHeight;

    private float nextFire = .0f;
    private float lastDistance = .0f;
    private Animator characterAnimator;
    private bool isMoving = false;

    private void Awake()
    {
        characterAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        var movement = HandleMovement();
        handleAnimation(movement);
        handleSound();
    }

    void FixedUpdate()
    {
        handleCombat();
    }

    Vector3 HandleMovement()
    {
        var moveTarget = target.transform.position;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance < 10 && lastDistance > distance)
        {
            List<Coordinate> path = AStarAlgorithm.AStar(new Coordinate(transform.position), new Coordinate(target.transform.position), map, mapWidth, mapHeight);
            Debug.Log("path " + path);
            Debug.Log("path Count " + path.Count);
            if (path.Count > 0)
            {
                moveTarget = path[0].ToPosition();
                Debug.Log("moveTarget " + moveTarget);
            }
        }
        lastDistance = distance;

        var movement = (moveTarget - transform.position).normalized * Time.deltaTime * speed;

        if (distance >= attackRange)
        {
            transform.Translate(movement);
            isMoving = true;
        }
        else if (distance < personalSpace)
        {
            transform.Translate(-movement);
            isMoving = true;
        }
        else
        {
            movement = new Vector3(0, 0, 0);
            isMoving = false;
        }
        return movement;
    }

    private void handleSound()
    {
        if (isMoving && !footsteps.isPlaying)
        {
            // float volume =  Mathf.Clamp(1.0f-(distance/10f),0f,1.0f);
            footsteps.Play();
        }
        else
        {
            footsteps.Pause();
        }
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
        if (target == null)
            return;

        var distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance < attackRange && Time.time >= nextFire)
        {
            nextFire = Time.time + fireCooldown;

            var projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            var projectileController = projectile.GetComponent<GenericProjectile>();
            projectileController.damage = attackDmg;
            projectileController.speed = prjSpeed;

            var relative = projectile.transform.InverseTransformPoint(target.transform.position);
            var angle = 90 - Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
            projectile.transform.Rotate(0, 0, angle);
        }
    }
}
