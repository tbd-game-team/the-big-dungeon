using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GenericEnemy : MonoBehaviour
{
    [SerializeField]
    public GameObject target;

    private void Awake()
    {
        if (target == null)
        {
            target = GameObject.FindWithTag("Player");
        }
    }
}
