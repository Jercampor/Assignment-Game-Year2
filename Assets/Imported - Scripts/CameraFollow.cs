using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float height = 15f;
    private Vector3 shakeOffset;
    private float shakeDuration;
    private float shakeMagnitude;

    public void Shake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }

    void LateUpdate()
    {
        Vector3 targetPos = new Vector3(player.position.x, height, player.position.z);

        if (shakeDuration > 0)
        {
            shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * shakeMagnitude,
                0f,
                Random.Range(-1f, 1f) * shakeMagnitude
            );
            shakeDuration -= Time.deltaTime;
        }
        else
        {
            shakeOffset = Vector3.zero;
        }

        transform.position = targetPos + shakeOffset;
    }
}