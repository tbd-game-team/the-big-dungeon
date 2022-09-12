using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;

using UnityEngine;


public class PeakTrap : MonoBehaviour
{

    [Header("Audio Settings")]
    [SerializeField]
    private float minDist = 1;
    [SerializeField]
    private float maxDist = 20;
    [SerializeField]
    private float maxVolume = 0.8f;

    [Header("Animation")]
    [SerializeField]
    private AnimationClip clip;

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

        // activate each trap in a random rhythm.
        float randomOffset = Random.Range(0.5f, 3.0f);
        float waitTime = clip.length + randomOffset;
        InvokeRepeating("ActivateTrap", randomOffset, waitTime);
    }

    void Update()
    {
        handleVolume();
        handleTrap();
    }

    
    /// <summary>
    /// @author: Neele Kemper
    /// Fade 2D audio by distance
    /// </summary>
    /// <returns></returns>
    private void handleVolume()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist < minDist)
        {
            trapAudio.volume = maxVolume;
        }
        else if (dist > maxDist)
        {
            trapAudio.volume = 0;
        }
        else
        {
            trapAudio.volume = maxVolume - ((dist - minDist) / (maxDist - minDist));
        }
    }

    private void handleTrap()
    {
        if (isActive)
        {
            if (playerCollider.IsTouching(trapCollider) && !damageIsTaken)
            {
                StartCoroutine(WaitingDamage(clip.length / 4));
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