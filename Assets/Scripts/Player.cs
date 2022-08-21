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

        public void Start()
        {
        }

        public void FixedUpdate(){}

        private void Update() {
            handleMovement();
        }

        private void handleMovement(){
            var x = Input.GetAxis("Horizontal");
            var y = Input.GetAxis("Vertical");

            if (x > 0) transform.localScale = Vector3.one;
            else if (x < 0) transform.localScale = new Vector3(-1, 1, 1);

            transform.Translate(new Vector2(x, y) * Time.deltaTime * walkSpeed);
        }
    }
}