using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

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

        public AudioManager audioManager;
        public AudioMixerSnapshot snapshotGameOver;

        private BoxCollider2D boxCollider;
        private Rigidbody2D rb;
        private Vector3 moveDelta;

        private bool isMoving;
        private Vector2 lastPosition;
        private bool playerIsSpawned = false;


        public void Start()
        {
            weapon = gameObject.transform.GetChild(0).gameObject;

            weaponAnimator = weapon.GetComponent<Animator>();
            boxCollider = GetComponent<BoxCollider2D>();
            rb = GetComponent<Rigidbody2D>();


            invincibleTimer = invincibleTime;
            invincible = false;


            lastPosition = rb.position;

            audioManager = FindObjectOfType<AudioManager>();
        }


        private void FixedUpdate()
        {
            if (alive && !GameManager.Instance.isPaused)
            {
                handleMovement();

            }
        }

        private void Update()
        {   
            if(!playerIsSpawned)
            {
                // spawn position of player
                transform.position = SpawnPositionGenerator.GetPlayerPosition();
                playerIsSpawned = true;
            }

            if (alive && !GameManager.Instance.isPaused)
            {
                handleInvincibility();
                handleMovementSound();
                if (Input.GetMouseButtonDown(0))
                {   
                    attack();
                }
            }
        }

        /// <summary>
        /// @author: Florian Weber, Neele Kemper
        /// 
        /// </summary>
        /// <returns></returns>
        private void handleMovement()
        {
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

            if (rb.position != lastPosition)
            {

                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
            lastPosition = rb.position;



        }

        /// <summary>
        /// @author: Neele Kemper
        /// Manages the footsteps sounds of the player when moving.
        /// </summary>
        /// <returns></returns>
        private void handleMovementSound()
        {
            if (isMoving && !audioManager.IsPlaying("PlayerFootsteps"))
            {
                audioManager.Play("PlayerFootsteps");
            }
            else if (!isMoving)
            {
                audioManager.Stop("PlayerFootsteps");
            }
        }

        private void attack()
        {
            weaponAnimator.SetTrigger("attack");
            audioManager.Play("PlayerSword");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "Enemy")
            {
                damage(1);
            }
            else if (other.gameObject.tag == "Coin")
            {
                // @Fabian: Todo
                Debug.Log("You win!");
                audioManager.Play("PlayerCoinSelection");
            } 
            else if(other.gameObject.tag == "HealthPotion")
            {
                restoreHealth(other);
            }
        }

        private void damage(int amount)
        {
            if (!invincible)
            {
                if (health - amount <= 0)
                {
                    GameManager.Instance.pause();
                    alive = false;
                    audioManager.Stop("PlayerFootsteps");
                    audioManager.Play("PlayerDeath");
                    health = 0;
                    gameOverPanel.SetActive(true);
                    snapshotGameOver.TransitionTo(2.0f);
                    GameObject pauseBtn = GameObject.FindGameObjectWithTag("PauseButton");
                    pauseBtn.SetActive(false);

                }
                else
                {
                    health -= amount;
                    invincible = true;
                    audioManager.Play("PlayerPain");
                    StartCoroutine("Flasher");
                }
                if (camShake != null)
                {
                    StartCoroutine(camShake.Shake(0.4f, 0.3f));
                }
                healthUi.updateHearts(health);
            }
        }

        private void restoreHealth(Collider2D potion)
        {
            if (health< healthUi.maxHealthPlayer && alive)
            {
                health+=1;
                audioManager.Play("PlayeDrinkPotion");
                healthUi.updateHearts(health);
            } 
            else 
            {
                audioManager.Play("PlayeShatterPotion");
            } 
            Destroy(potion.gameObject);
        }

        private void handleInvincibility()
        {
            if (invincibleTimer <= 0 && invincible)
            {
                invincibleTimer = invincibleTime;
                invincible = false;
            }
            if (invincible)
            {
                invincibleTimer -= Time.deltaTime;
            }
        }

        public int getHealth()
        {
            return health;
        }

        IEnumerator Flasher()
        {
            var renderer = gameObject.GetComponent<SpriteRenderer>();
            var normalColor = renderer.color;
            for (int i = 0; i < 4; i++)
            {
                renderer.color = collideColor;
                yield return new WaitForSeconds(invincibleTime / 8);
                renderer.color = normalColor;
                yield return new WaitForSeconds(invincibleTime / 8);
            }
        }
    }
}