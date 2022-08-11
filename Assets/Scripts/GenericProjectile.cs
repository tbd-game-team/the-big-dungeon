using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class GenericProjectile : MonoBehaviour
{

    [SerializeField]
    private float speed = 40f;

    private Rigidbody2D rb;
    public GameObject explosion;
    private Vector2 screenBounds;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }

    void Start()
    {
        rb.mass = 0;
        rb.velocity = new Vector2(speed, 0);
    }

    void FixedUpdate()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.x < -.1 || viewPos.x > 1.1 || viewPos.y < -.1 || viewPos.y > 1.1)
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Instantiate(explosion, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    }
}
