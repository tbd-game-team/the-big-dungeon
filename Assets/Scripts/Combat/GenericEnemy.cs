using UnityEngine;

namespace Assets.Scripts.Combat
{
    [RequireComponent(typeof(Animator))]
    public abstract class GenericEnemy : MonoBehaviour
    {
        [SerializeField]
        public GameObject target;
        [SerializeField]
        public int[,] map;
        [SerializeField]
        public int mapWidth;
        [SerializeField]
        public int mapHeight;

        private void Awake()
        {
            if (target == null)
            {
                target = GameObject.FindWithTag("Player");
            }
        }

        public abstract void OnHitPlayer(Player player);
    }
}
