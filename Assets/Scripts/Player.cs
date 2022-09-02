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
        private Animator weaponAnimator;
        private GameObject weapon;
        private float invincibleTimer;
        public bool invincible;

        


        public void Start()
        {
            weapon = gameObject.transform.GetChild(0).gameObject;
            weaponAnimator = weapon.GetComponent<Animator>();
            invincibleTimer = invincibleTime;
            invincible = false;
        }

        public void FixedUpdate(){}

        private void Update() {
            if(alive){
                handleMovement();
                handleInvincibility();

                if(Input.GetMouseButtonDown(0)){
                    attack();
                }
            }
        }

        private void handleMovement(){
            var x = Input.GetAxis("Horizontal");
            var y = Input.GetAxis("Vertical");

            if (x > 0) transform.localScale = Vector3.one;
            else if (x < 0) transform.localScale = new Vector3(-1, 1, 1);

            transform.Translate(new Vector2(x, y) * Time.deltaTime * walkSpeed);
        }

        private void attack(){
            weaponAnimator.SetTrigger("attack");
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.tag == "Enemy"){
                damage(1);
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