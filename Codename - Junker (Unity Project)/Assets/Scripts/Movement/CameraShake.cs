using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Transform camTransform;

    private float shakeDuration = 0f;

    private float shakeAmount = 0.7f;
    private float decreaseFactor = 1.0f;

    Vector3 originalPos;

    private static CameraShake s_instance;

    public static CameraShake Instance { get => s_instance; set => s_instance = value; }

    void Awake()
    {
        //Singleton Implementation
        if (s_instance == null)
        {
            s_instance = this;
        }
        else
        {
            Destroy(s_instance.gameObject);
            s_instance = this;
        }

        camTransform = GetComponent<Transform>();
    }

    void Update()
    {
        originalPos = camTransform.position;
        if (shakeDuration > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
    }

    public void Shake(float _shakeDuration, float _amplitude)
    {
        shakeDuration = _shakeDuration;
        shakeAmount = _amplitude;
    }
    public void Shake(float _amplitude)
    {
        shakeDuration = 0.4f;
        shakeAmount = _amplitude;
    }
}
