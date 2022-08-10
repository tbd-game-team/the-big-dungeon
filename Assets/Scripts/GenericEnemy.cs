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

    [Header("Combat")]
    [SerializeField]
    private GameObject projectilePrefab;

    [Header("Combat")]
    [SerializeField]
    private float fireCooldown = .5f;

    private float nextFire = .0f;

    void Start()
    {
    }

    void FixedUpdate()
    {
        var dir = transform.LookAt(target.transform);
        var x = 1;
        var y = 1;

        handleAnimation(x, y);
        handleMovement(x, y);
        handleCombat(x, y);
    }

    void handleAnimation(float x, float y)
    {
        if (x > 0)
        {
            transform.localScale = Vector3.one;
        }
        else if (x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void handleMovement(float x, float y)
    {
        transform.Translate(new Vector3(x, y, 0).normalized * Time.deltaTime * speed);
    }

    void handleCombat()
    {
        float distance = Vector3.Distance(object1.transform.position, object2.transform.position);

        if (distance < 100 && Time.time >= nextFire)
        {
            nextFire = Time.time + fireRate;
            Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        }
    }
}
