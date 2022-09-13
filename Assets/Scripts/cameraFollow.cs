using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public Transform follow;

    // Update is called once per frame
    void Update () {
        transform.position = follow.transform.position + new Vector3(0, 1, -5);
    }
}
