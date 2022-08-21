using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dashMove : MonoBehaviour
{
    private Rigidbody2D rb; 
    
    private float dashTimer;
    private float pauseTimer;
    
    // For Dash Direction
    private float x = 0;
    private float y = 0;

    // Dash properties
    public float dashSpeed;
    public float dashTime;
    public float TimeBetweenDash;

    [Header("Info")]
    [SerializeField]
    private bool dashing = false;

    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dashTimer = dashTime;
        dashing = false;
        pauseTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        handleDash();
    }

    void handleDash(){
        if(!dashing && pauseTimer <= 0){
            // right click
            if(Input.GetMouseButtonDown(1)){
                pauseTimer = TimeBetweenDash;
                dashing = true;
                x = Input.GetAxis("Horizontal");
                y = Input.GetAxis("Vertical");
            }
        } else {
            if(dashTimer <= 0){
                dashing = false;
                pauseTimer -= Time.deltaTime;
                if(pauseTimer<= 0){
                    dashTimer = dashTime;
                }
                rb.velocity = Vector2.zero;
                x = 0;
                y = 0;
            }else{
                dashTimer -= Time.deltaTime;
                rb.velocity = new Vector2(x, y) * dashSpeed;
            }
        }
    }
}
