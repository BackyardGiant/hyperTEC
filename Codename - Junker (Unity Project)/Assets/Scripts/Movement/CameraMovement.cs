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

    [SerializeField, Header("Player"), Tooltip("The player object should go here")]
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

    [SerializeField, Header("Cameras"), Tooltip("This camera")]
    private Camera m_followCamera;
    [SerializeField, Tooltip("The look around camera")]
    private Transform m_orbitalCameraRig;
    [SerializeField, Tooltip("The ship object")]
    private GameObject m_focusObject;

    [SerializeField, Header("Crosshair")]
    private Crosshair m_crosshair;

    void Update()
    {
        if (Mathf.Abs(Input.GetAxis("LookX")) > 0 || Mathf.Abs(Input.GetAxis("LookY")) > 0)
        {
            m_crosshair.enabled = false;
            FollowCameraMovement(m_orbitalCameraRig.position, true);
        }
        else if (Vector3.Distance(m_trPlayerTransform.TransformPoint(m_cameraFollowPos), transform.position) > 25f)
        {
            m_crosshair.enabled = false;
            m_orbitalCameraRig.GetComponent<OrbitalCamera>().CurrentX = 0;
            m_orbitalCameraRig.GetComponent<OrbitalCamera>().CurrentY = 0;
            FollowCameraMovement(m_trPlayerTransform.TransformPoint(m_cameraFollowPos), true);
        }
        else
        {
            m_crosshair.enabled = true;
            m_orbitalCameraRig.GetComponent<OrbitalCamera>().CurrentX = 0;
            m_orbitalCameraRig.GetComponent<OrbitalCamera>().CurrentY = 0;
            FollowCameraMovement(m_trPlayerTransform.TransformPoint(m_cameraFollowPos), false);
        }
    }

    private void FollowCameraMovement(Vector3 _targetPosition, bool _lookAtPlayer)
    {
        if (_lookAtPlayer)
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * 100);

            transform.eulerAngles = m_orbitalCameraRig.eulerAngles;
        }
        else
        {
            float targetDistance = Vector3.Distance(transform.position, _targetPosition);

            // Smoothly move the camera towards that target position
            transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref m_velocity, Mathf.Clamp((m_smoothTime) / (GameManager.Instance.GameSpeed), 0.07f, 0.12f));

            transform.eulerAngles = m_trPlayerTransform.eulerAngles + m_trPlayerTransform.TransformDirection(m_offSet);
        }
    }
}
