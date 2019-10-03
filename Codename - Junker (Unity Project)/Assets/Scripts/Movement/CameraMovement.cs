using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraPositions
{
    Normal
}

public class CameraMovement : MonoBehaviour
{
    [SerializeField, Header("Camera Position"), Tooltip("Select where the camera will follow the player from")]
    private CameraPositions m_cameraPosition;

    [SerializeField, Header("Player"),Tooltip("The player object should go here")]
    private Transform m_trPlayerTransform;
    [SerializeField, Tooltip("The offset of the angels")]
    private Vector3 m_offSet;
    [SerializeField, Tooltip("Camera distances")]
    private Vector3 m_cameraFollowPos;

    [SerializeField, Header("Smoothing"), Tooltip("Determines how quickly the z rotation will follow the ship")]
    private float m_zSmooth = 0.01f;
    [SerializeField, Tooltip("The smoothing of the camera follow")]
    private float m_smoothTime = 0.3F;
    private Vector3 m_velocity = Vector3.zero;

    void Update()
    {
        // Define a target position above and behind the target transform
        Vector3 targetPosition = m_trPlayerTransform.TransformPoint(m_cameraFollowPos);

        float targetDistance = Vector3.Distance(transform.position, targetPosition);

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref m_velocity, Mathf.Clamp((m_smoothTime - (targetDistance * 0.005f)) / (GameManager.Instance.GameSpeed), 0.07f, 0.12f));

        //transform.eulerAngles = m_trPlayerTransform.eulerAngles + m_offSet;

        transform.eulerAngles = m_trPlayerTransform.eulerAngles + m_trPlayerTransform.TransformDirection(m_offSet);
    }
}
