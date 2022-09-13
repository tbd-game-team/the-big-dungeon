using System.Collections;
using UnityEngine;

/// <summary>
/// @author: Neele Kemper
/// destroy the skeleton on hit.
/// </summary>
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
            audioSource.Play();
            StartCoroutine(DestroySkeleton());
        }
    }

    IEnumerator DestroySkeleton()
        {
            yield return new WaitForSeconds(audioSource.clip.length);
            Debug.Log("Destroy");
            Destroy(this.gameObject);
        }
}
