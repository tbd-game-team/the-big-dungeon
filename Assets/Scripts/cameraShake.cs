using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// @author: Florian Weber
/// Manages Camera Shake.
public class cameraShake : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude){
        Vector3 originalPos = transform.localPosition;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float xOffset = Random.Range(-0.5f, 0.5f) * magnitude;
            float yOffset = Random.Range(-0.5f, 0.5f) * magnitude;

            transform.localPosition = new Vector3(xOffset, yOffset, originalPos.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }
}

// code found on Medium by Mohamed Hijazi 
// https://medium.com/nerd-for-tech/tip-of-the-day-simple-2d-camera-shake-in-unity-521d454ac89b