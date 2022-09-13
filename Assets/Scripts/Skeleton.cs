using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    private AudioSource audioSource;
    void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Weapon")
        {
            Debug.Log("Is hit");
            Debug.Log("audioSource.isPlaying: "+audioSource.isPlaying);
            audioSource.Play();
            Debug.Log("audioSource.isPlaying: "+audioSource.isPlaying);
            Destroy(this.gameObject);


        }
    }
}
