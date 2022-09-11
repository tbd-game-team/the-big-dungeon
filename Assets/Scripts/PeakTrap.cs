using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;

using UnityEngine;


public class PeakTrap : MonoBehaviour
{


    public float minDist = 2;
    public float maxDist = 30;

    public AnimationClip clip;

    private AudioSource trapAudio;

    private Animator trapAnimator;
    private GameObject player;

    private BoxCollider2D trapCollider;
    private BoxCollider2D playerCollider;


    private bool isActive = false;
    private bool damageIsTaken = false;




    void Start()
    {
        player = GameObject.FindWithTag("Player");
        trapAnimator = gameObject.GetComponent<Animator>();
        trapAudio = gameObject.GetComponent<AudioSource>();
        trapCollider = gameObject.GetComponent<BoxCollider2D>();
        playerCollider = player.GetComponent<BoxCollider2D>();

        float randomOffset = Random.Range(2.0f, 8.0f);
        float waitTime = clip.length + randomOffset;
        InvokeRepeating("ActivateTrap", randomOffset, waitTime);
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist < minDist)
        {
            trapAudio.volume = 1;
        }
        else if (dist > maxDist)
        {
            trapAudio.volume = 0;
        }
        else
        {
            trapAudio.volume = 1 - ((dist - minDist) / (maxDist - minDist));
        }

        if (isActive)
        {
            if (playerCollider.IsTouching(trapCollider) && !damageIsTaken)
            {
                StartCoroutine(WaitingDamage(clip.length - clip.length / 2));
            }
        }
    }

    void ActivateTrap()
    {
        isActive = true;
        trapAnimator.SetTrigger("Activate");
        trapAudio.Play();
        trapAnimator.SetTrigger("Deactivate");
        StartCoroutine(Waiting(clip.length));
    }

    IEnumerator Waiting(float time)
    {
        yield return new WaitForSeconds(time);
        isActive = false;
        damageIsTaken = false;
    }


    IEnumerator WaitingDamage(float time)
    {
        yield return new WaitForSeconds(time);
        player.GetComponent<Player>().damage(1);
        damageIsTaken = true;
    }

}