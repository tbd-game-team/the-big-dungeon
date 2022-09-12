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
        [SerializeField]
        private float timeToBreakMin = 5f;
        [SerializeField]
        private float timeToBreakMax = 15f;
        [SerializeField]
        private GameObject explosionPrefab;
        [SerializeField]
        private GameObject enemyPrefab;

        private const float EggBreakAnimTime = 1.5f;

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

        void Update()
        {
            if (Time.time >= breakTimestamp - EggBreakAnimTime)
            {
                characterAnimator.SetBool(Keys.ANIMATION_CRACK_KEY, true);
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
