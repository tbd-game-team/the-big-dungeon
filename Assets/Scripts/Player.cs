using UnityEngine;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        // Start is called before the first frame update
        public void Start()
        {
        }

        public void FixedUpdate()
        {
            var x = Input.GetAxis("Horizontal");
            var y = Input.GetAxis("Vertical");

            if (x > 0) transform.localScale = Vector3.one;
            else if (x < 0) transform.localScale = new Vector3(-1, 1, 1);

            transform.Translate(new Vector3(x, y, 0) * Time.deltaTime);
        }
    }
}