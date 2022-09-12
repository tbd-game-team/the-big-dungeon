using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : GenericEnemy
{
    [Header("Movement")]
    [SerializeField]
    private float speed = 3.5f;
    [SerializeField]
    private float personalSpace = 1f;
    [SerializeField]
    private LayerMask wallLayer;
    public string searchMode = "";

    [Header("Combat")]
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private float attackRange = 1f;
    [SerializeField]
    private float seeRange = 15f;
    [SerializeField]
    private float fireCooldown = .5f;
    [SerializeField]
    private float attackDmg = 1f;
    [SerializeField]
    private float prjSpeed = 1f;

    [Header("Audio Settings")]
    [SerializeField]
    private float minDist = 1;
    [SerializeField]
    private float maxDist = 20;
    [SerializeField]
    private float maxVolume = 0.8f;
    [SerializeField]
    private AudioSource footsteps;

    private float nextFire = .0f;
    private Animator characterAnimator;
    private Rigidbody2D rb2d;
    private bool isMoving = false;

    private void Awake()
    {
        characterAnimator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        if (target == null)
        {
            target = GameObject.FindWithTag("Player");
        }

        onTouchDmg = 1;
    }

    void Update()
    {
        var movement = HandleMovement();
        handleAnimation(movement);
        handleSound();
        handleVolume();
    }

    void FixedUpdate()
    {
        handleCombat();
    }

    Vector3 HandleMovement()
    {
        if (target == null)
        {
            Debug.Log("no enemy");
            searchMode = "no enemy";
            return transform.position;
        }
        var moveTarget = transform.position;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > seeRange)
        {
            searchMode = "no enemy in sight";
            isMoving = false;
        }
        else if (distance < attackRange && distance > personalSpace)
        {
            searchMode = "attacking";
            isMoving = false;
        }
        else
        {
            isMoving = true;

            // Determine where to move to
            if (distance < personalSpace)
            {
                moveTarget = transform.position - (target.transform.position - transform.position);
                searchMode = "fleeing";
            }
            else
            {
                moveTarget = target.transform.position;
                searchMode = "approaching";
            }

            // Find a way
            Debug.Log("distance " + distance);
            // Only the walls of the tile map / blocking layer are interesting
            // Physics2D.queriesStartInColliders = true;
            RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, moveTarget - transform.position, float.PositiveInfinity, wallLayer);
            Debug.DrawRay(transform.position, moveTarget - transform.position, Color.red);
            if (hitInfo.collider != null && hitInfo.distance < distance)
            {
                Debug.Log("hitInfo.collider " + hitInfo.collider);
                List<Coordinate> path = AStarAlgorithm.AStar(new Coordinate(transform.position), new Coordinate(target.transform.position), map, mapWidth, mapHeight, 15);

                Debug.DrawRay(transform.position, Vector3.down); // ok
                Debug.DrawRay(new Coordinate(transform.position).ToCentralPosition(), Vector3.up);
                Debug.Log("path from " + new Coordinate(transform.position) + " - " + new Coordinate(transform.position).ToCentralPosition());
                Debug.Log("path to " + new Coordinate(target.transform.position) + " - " + new Coordinate(target.transform.position).ToCentralPosition());
                if (path.Count > 0)
                {
                    Debug.Log("path " + path.Count);
                    foreach (var coord in path)
                    {
                        Debug.DrawRay(coord.ToCentralPosition(), new Vector3(1, 1, 0));
                        Debug.Log("path: " + coord.ToCentralPosition());
                    }

                    moveTarget = path[path.Count - 1].ToCentralPosition();
                    searchMode += "/" + path.Count + " tiles";
                }
                else
                {
                    Debug.Log("no way");
                    moveTarget = transform.position;
                    searchMode += "/no way";
                }
                Debug.Log("move from " + transform.position + " to " + moveTarget + "/" + target.transform.position);
                searchMode += "/searching";
            }
            else
            {
                moveTarget = target.transform.position;
                Debug.Log("direct move ");
                searchMode += "/direct";
            }
            isMoving = true;
        }

        var movement = (moveTarget - transform.position).normalized * Time.deltaTime * speed;
        rb2d.MovePosition(transform.position + movement);

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

    /// <summary>
    /// @author: Neele Kemper
    /// Fade 2D audio by distance
    /// </summary>
    /// <returns></returns>
    private void handleVolume()
    {
        float dist = Vector3.Distance(transform.position, target.transform.position);
        if (dist < minDist)
        {
            footsteps.volume = maxVolume;
        }
        else if (dist > maxDist)
        {
            footsteps.volume = 0;
        }
        else
        {
            footsteps.volume = maxVolume - ((dist - minDist) / (maxDist - minDist));
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
