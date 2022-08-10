using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEnemy : MonoBehaviour
{
    [SerializeField] private float speed = 7.5f;
    [SerializeField] private GameObject projectilePrefab;

    private Animator characterAnimator;
    void Start()
    {

    }

    void FixedUpdate()
    {
        var x = 1;
        var y = 1;

        handleAnimation(x, y);
        handleMovement(x, y);
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
}
