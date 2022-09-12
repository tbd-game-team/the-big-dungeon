using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GenericEnemy : MonoBehaviour
{
    [SerializeField]
    public GameObject target;

    public int[,] map;
    public int mapWidth;
    public int mapHeight;

    public float onTouchDmg = 0;

    private void Awake()
    {
        if (target == null)
        {
            target = GameObject.FindWithTag("Player");
        }
    }

    public virtual void OnHitPlayer(Player player)
    {
        Destroy(gameObject);
        player.damage(Mathf.CeilToInt(onTouchDmg));
    }
}
