using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        [Header("Atributes")]
        [SerializeField]
        private int health = 5;

        [Header("Movement")]
        /// <summary>
        /// Character Walk Speed
        /// </summary>
        [SerializeField]
        private float walkSpeed = 7.5f;

        [Header("UI")]
        public HealthUi healthUi;
        public GameObject gameOverPanel;
        private bool alive = true;

        [Header("Combat")]
        // Attack parameters
        public float invincibleTime;
        public Color collideColor;
        public cameraShake camShake;
        private Animator weaponAnimator;
        private GameObject weapon;
        private float invincibleTimer;
        public bool invincible;

        private BoxCollider2D boxCollider;
        private Rigidbody2D rb;
        private Vector3 moveDelta;

        public void Start()
        {
            weapon = gameObject.transform.GetChild(0).gameObject;

            weaponAnimator = weapon.GetComponent<Animator>();
            boxCollider = GetComponent<BoxCollider2D>();
            rb = GetComponent<Rigidbody2D>();

            invincibleTimer = invincibleTime;
            invincible = false;
            
            // spawn position of player
            transform.position = ActorGenerator.GetPlayerPosition();
        }

        //public void Update(){}

        private void FixedUpdate() {
            if(alive && !GameManager.Instance.isPaused){
                handleMovement();
                handleInvincibility();

                if(Input.GetMouseButtonDown(0)){
                    attack();
                }
            }
        }

        /*
        * @author: Florian Weber, Neele Kemper
        * 
        */
        private void handleMovement(){
            var x = Input.GetAxis("Horizontal");
            var y = Input.GetAxis("Vertical");
            moveDelta = new Vector3(x, y, 0);

            // Swap spirit direction
            if (moveDelta.x > 0)
            {
                transform.localScale = Vector3.one;
            }
            else if (moveDelta.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }


            rb.MovePosition(transform.position + moveDelta * Time.deltaTime * walkSpeed);
        }

        private void attack(){
            weaponAnimator.SetTrigger("attack");
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.tag == "Enemy"){
                damage(1);
            } else if(other.gameObject.tag == "Coin"){
                // @Fabian: Todo
                Debug.Log("You win!");
            }
        }

        private void damage(int amount){
            if(!invincible){
                if(health - amount <= 0){
                    GameManager.Instance.pause();
                    alive = false;
                    health = 0;
                    gameOverPanel.SetActive(true);
                }else{
                    health -= amount;
                    invincible = true;
                    StartCoroutine("Flasher");
                }
                if(camShake != null){
                    StartCoroutine(camShake.Shake(0.4f, 0.3f));
                }
                healthUi.updateHearts(health);
            }
        }

        private void handleInvincibility(){
            if(invincibleTimer <= 0 && invincible){
                invincibleTimer = invincibleTime;
                invincible = false;
            }if(invincible){
                invincibleTimer -= Time.deltaTime;
            }
        }

        public int getHealth(){
            return health;
        }

        IEnumerator Flasher() 
         {
            var renderer = gameObject.GetComponent<SpriteRenderer>();
            var normalColor = renderer.color;
             for (int i = 0; i < 4; i++)
             {
              renderer.color = collideColor;
              yield return new WaitForSeconds(invincibleTime/8);
              renderer.color = normalColor; 
              yield return new WaitForSeconds(invincibleTime/8);
             }
          }
    }
}