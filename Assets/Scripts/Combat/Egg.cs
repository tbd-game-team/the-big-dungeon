using UnityEngine;

namespace Assets.Scripts.Combat
{

    /// <summary>
    /// Enemy type that can be destroyed by walking over it, but in case of failing to do so will spawn another enemy
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Animator))]
    public class Egg : GenericEnemy
    {
        [Header("\"Combat\"")]
        [SerializeField]
        private float timeToBreakMin = 5f;
        [SerializeField]
        private float timeToBreakMax = 15f;
        [SerializeField]
        private GameObject explosionPrefab;
        [SerializeField]
        private GameObject enemyPrefab;

        [Header("Audio Settings")]
        [SerializeField]
        private float minDist = 1;
        [SerializeField]
        private float maxDist = 20;
        [SerializeField]
        private float maxVolume = 0.8f;
        [SerializeField]
        private AudioSource hatchAudio;
        [SerializeField]
        private AudioSource deathAudio;

        private const float EggBreakAnimTime = 1.5f;

        private bool alive = true;
        private float timeToBreak;
        private float breakTimestamp;
        private Animator characterAnimator;

        void Awake()
        {
            characterAnimator = GetComponent<Animator>();

            // Break time is randomized in given range
            timeToBreak = Random.Range(timeToBreakMin, timeToBreakMax);
        }

        void Start()
        {
            breakTimestamp = Time.time + timeToBreak;
        }

        private void handleVolume()
        {
            float dist = Vector3.Distance(transform.position, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y));
            if (dist < minDist)
            {
                hatchAudio.volume = maxVolume;
                deathAudio.volume = maxVolume;
            }
            else if (dist > maxDist)
            {
                hatchAudio.volume = 0;
                deathAudio.volume = 0;
            }
            else
            {
                hatchAudio.volume = maxVolume - ((dist - minDist) / (maxDist - minDist));
                deathAudio.volume = maxVolume - ((dist - minDist) / (maxDist - minDist));
            }
        }

        void Update()
        {
            handleVolume();

            if (Time.time >= breakTimestamp - EggBreakAnimTime)
            {
                characterAnimator.SetBool(Keys.ANIMATION_CRACK_KEY, true);
                hatchAudio.Play();
            }

            if (!alive
                && !(characterAnimator.GetCurrentAnimatorStateInfo(0).length > characterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime)
                && !deathAudio.isPlaying)
            {
                Destroy(this.gameObject);
            }
        }

        void FixedUpdate()
        {
            if (Time.time >= breakTimestamp)
            {
                Hatch();
            }
        }

        /// <summary>
        /// Breaking the egg by walking over it destroys it.
        /// </summary>
        public override void OnHitPlayer(Player player)
        {
            alive = false;
            hatchAudio.Stop();
            deathAudio.Play();
            characterAnimator.SetBool(Keys.ANIMATION_CRACK_KEY, true);

            Instantiate(explosionPrefab, transform.position, transform.rotation);

            Destroy(this.gameObject);
        }

        /// <summary>
        /// In case the egg is left alone it hatches an enemy.
        /// </summary>
        private void Hatch()
        {
            var enemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
            var enemyController = enemy.GetComponent<GenericEnemy>();
            enemyController.target = GameObject.FindWithTag(Keys.TAG_PLAYER);
            enemyController.map = map;
            enemyController.mapHeight = mapHeight;
            enemyController.mapWidth = mapWidth;

            Destroy(this.gameObject);
        }
    }
}
