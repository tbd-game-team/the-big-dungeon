using UnityEngine;

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Projectile being shot by ranged enemies
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class Projectile : MonoBehaviour
    {

        [SerializeField]
        public float speed = 40f;
        [SerializeField]
        public float damage = 0f;
        [SerializeField]
        private GameObject explosionPreset;

        private void Awake()
        {
        }

        void FixedUpdate()
        {
            // Kinematic, simple motion that is predictable to the player
            transform.position += transform.right * speed * Time.deltaTime;

            // In case the projectile is not in camera view, it can safely be destroyed for performance
            Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
            if (viewPos.x < -.1 || viewPos.x > 1.1 || viewPos.y < -.1 || viewPos.y > 1.1)
            {
                Destroy(this.gameObject);
            }
        }

        public void OnHitPlayer(Player player)
        {
            Instantiate(explosionPreset, transform.position, transform.rotation);
            Destroy(gameObject);
            player.damage(Mathf.CeilToInt(damage));
        }
    }
}
