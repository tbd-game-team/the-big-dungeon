using System.Collections;
using Assets.Scripts.Combat;
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
        public GameObject winPanel;
        private bool alive = true;

        [Header("Combat")]
        [SerializeField]
        private LayerMask enemyLayer;
        [SerializeField]
        private float attackRange = 1.5f;
        // Attack parameters
        public float invincibleTime;
        public Color collideColor;
        public cameraShake camShake;
        private Animator weaponAnimator;
        private GameObject weapon;
        private float invincibleTimer;
        public bool invincible;

        private AudioManager audioManager;
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
        }

        private void Update()
        {
            if (!playerIsSpawned)
            {
                // spawn position of player
                transform.position = SpawnPositionGenerator.GetPlayerPosition();
                playerIsSpawned = true;
            }

            if (alive && !GameManager.Instance.isPaused)
            {
                handleMovement();
                handleInvincibility();
                handleMovementSound();
                if (Input.GetMouseButtonDown(0))
                {
                    Attack();
                }
            }
        }

        /// <summary>
        /// @author: Florian Weber
        /// Manages player movement.
        /// </summary>
        /// <returns></returns>
        private void handleMovement()
        {
            var x = Input.GetAxis("Horizontal");
            var y = Input.GetAxis("Vertical");

            if (x != 0 && y != 0)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }

            if (x > 0) transform.localScale = Vector3.one;
            else if (x < 0) transform.localScale = new Vector3(-1, 1, 1);

            transform.Translate(new Vector2(x, y) * Time.deltaTime * walkSpeed);
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

        /// <summary>
        /// @author: Florian Weber, Neele Kemper
        /// Manages player attack
        /// </summary>
        /// <returns></returns>
        private void Attack()
        {
            weaponAnimator.SetTrigger("attack");
            audioManager.Play("PlayerSword");

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
            foreach (var hitCollider in colliders)
            {
                if (hitCollider.gameObject.tag != Keys.TAG_ENEMY)
                    continue;

                if (hitCollider.transform.position.x > transform.position.x)
                {
                    // Hit is to -x, but player looks at +x
                    if (transform.localScale != Vector3.one)
                    {
                        continue;
                    }
                }
                else
                {
                    // Hit is to +x, but player looks at -x
                    if (transform.localScale == Vector3.one)
                    {
                        continue;
                    }
                }

                var enemy = hitCollider.gameObject.gameObject.GetComponent<GenericEnemy>();
                enemy.OnBeingHit();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "Enemy")
            {
                var e = other.gameObject.GetComponent<GenericEnemy>();
                e.OnHitPlayer(this);
            }
            else if (other.gameObject.tag == "Coin")
            {
                audioManager.Play("PlayerCoinSelection");
                GameManager.Instance.pause();
                audioManager.Stop("PlayerFootsteps");
                audioManager.Play("PlayerDeath");
                winPanel.SetActive(true);
                snapshotGameOver.TransitionTo(2.0f);
                GameObject pauseBtn = GameObject.FindGameObjectWithTag("PauseButton");
                if (pauseBtn)
                {
                    pauseBtn.SetActive(false);
                }
            }
            else if (other.gameObject.tag == "HealthPotion")
            {
                restoreHealth(other);
            }
            else if (other.gameObject.tag == Keys.TAG_PROJECTILE)
            {
                var p = other.gameObject.GetComponent<Projectile>();
                p.OnHitPlayer(this);
            }
        }

        /// <summary>
        /// @author: Florian Weber
        /// Manages player getting damage
        /// </summary>
        /// <returns></returns>
        public void damage(int amount)
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
                    if (pauseBtn)
                    {
                        pauseBtn.SetActive(false);
                    }
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
                healthUi.updateHearts(health, true);
            }
        }

        private void restoreHealth(Collider2D potion)
        {
            if (health < healthUi.maxHealthPlayer && alive)
            {
                health += 1;
                audioManager.Play("PlayeDrinkPotion");
                healthUi.updateHearts(health, false);
            }
            else
            {
                audioManager.Play("PlayeShatterPotion");
            }
            Destroy(potion.gameObject);
        }

        /// <summary>
        /// @author: Florian Weber
        /// Manages player invincibility after getting hit
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// @author: Florian Weber
        /// Makes Player flashing; used after receiving damage
        /// </summary>
        /// <returns></returns>
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
