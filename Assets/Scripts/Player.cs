using UnityEngine;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        [Header("Atributes")]
        [SerializeField]
        private int health = 5;
        [SerializeField]
        private int power = 5; 

        [Header("Movement")]
        /// <summary>
        /// Character Walk Speed
        /// </summary>
        [SerializeField]
        private float walkSpeed = 7.5f;

        [Header("Attack")]
        // Attack parameters
        private Animator weaponAnimator;
        private GameObject weapon;
        private bool attacking = false;


        public void Start()
        {
            weapon = gameObject.transform.GetChild(0).gameObject;
            weaponAnimator = weapon.GetComponent<Animator>();
        }

        public void FixedUpdate(){}

        private void Update() {
            handleMovement();

            if(Input.GetMouseButtonDown(0)){
                    attack();
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
                print("Player damaged");
                damage(1);
            }
        }

        private void damage(int amount){
            if(health - amount <= 0){
                // dead
            }else{
                health -= amount;
            }
        }
    }
}