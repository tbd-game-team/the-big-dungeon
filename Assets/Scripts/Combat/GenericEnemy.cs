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
    private bool isMoving = false;

    private void Awake()
    {   
        characterAnimator = GetComponent<Animator>();
        if (target == null)
        {
            target = GameObject.FindWithTag("Player");
        }
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
        var distance = Vector3.Distance(transform.position, target.transform.position);

        var movement = (target.transform.position - transform.position).normalized * Time.deltaTime * speed;

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
        if(isMoving && !footsteps.isPlaying)
        {   
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
