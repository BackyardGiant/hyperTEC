using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Speed
    [Header("Speed Settings")]
    [SerializeField, Tooltip("The maximum speed that the ship can fly")]
    private float m_maxSpeed;
    [SerializeField, Tooltip("The current speed of the ship, between 0 and Max Speed")]
    private float m_currentSpeed;
    [SerializeField, Tooltip("The current negative speed of the ship, between 0 and Max Speed")]
    private float m_negativeSpeed;
    [SerializeField, Tooltip("The acceleration of the ship")]
    private float m_acceleration;
    private float m_posativeClampedSpeed; // Forward force being applied
    private float m_negativeClampedSpeed; // Backwards force being applied
    #endregion

    #region Pitch, Yaw and Roll
    [Header("Pitch, Yaw and Roll speed")]
    [SerializeField, Tooltip("This controls the speed at which the ship rolls (Z-Axis)")]
    private float m_rollSpeed;
    [SerializeField, Tooltip("This controls the speed at which the ships yaw is affected (Y-Axis)")]
    private float m_yawSpeed;
    [SerializeField, Tooltip("This controls the speed at which the ships pitch is affected (Y-Axis)")]
    private float m_pitchSpeed;
    [Header("Invert Y axis")]
    [SerializeField]
    private bool m_invertY;
    private float m_invertScale = 1;
    #endregion

    #region Rigidbody
    private Rigidbody m_rbPlayer;
    #endregion

    private void Awake()
    {
        if (m_invertY)
        {
            m_invertScale = -1;
        }
    }

    private void Start()
    {
        m_rbPlayer = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //float _roll = -Input.GetAxis("Horizontal");
        //float _pitch = Input.GetAxis("Vertical");
        //float _yaw = Input.GetAxis("Yaw");

        //transform.Rotate(new Vector3(_pitch * (m_invertScale * m_pitchSpeed) * GameManager.Instance.GameSpeed, _yaw * m_yawSpeed * GameManager.Instance.GameSpeed, _roll * m_rollSpeed * GameManager.Instance.GameSpeed), Space.Self);

        //float _clampedSpeed = (Input.GetAxis("Throttle") + 1) / 2;

        if(Input.GetButton("Throttle Up"))
        {
            m_posativeClampedSpeed += m_acceleration / 100;
            m_posativeClampedSpeed = Mathf.Clamp(m_posativeClampedSpeed, 0, 1);

            m_currentSpeed = m_posativeClampedSpeed * m_maxSpeed;
        }
        if (Input.GetButton("Throttle Down"))
        {
            m_negativeClampedSpeed += m_acceleration / 500;
            m_negativeClampedSpeed = Mathf.Clamp(m_negativeClampedSpeed, 0, 1);

            m_negativeSpeed = m_negativeClampedSpeed * m_maxSpeed;
        }

        if(Input.GetButtonUp("Throttle Up"))
        {
            m_posativeClampedSpeed = 0;
            m_currentSpeed = 0;
        }
        if(Input.GetButtonUp("Throttle Down"))
        {
            m_posativeClampedSpeed = 0;
            m_negativeSpeed = 0;
        }

    }
    private void FixedUpdate()
    {
        float _roll = Input.GetAxis("Horizontal");
        float _pitch = Input.GetAxis("Vertical");
        float _yaw = 0;

        if (Input.GetButton("Yaw+"))
        {
            _yaw += 1;
        }
        if (Input.GetButton("Yaw-"))
        {
            _yaw -= 1;
        }

        // We accumulate torque forces into this variable:
        var torque = Vector3.zero;
        // Add torque for the pitch based on the pitch input.
        torque += _pitch * m_pitchSpeed * transform.right;
        // Add torque for the yaw based on the yaw input.
        torque += _yaw * m_yawSpeed * transform.up;
        // Add torque for the roll based on the roll input.
        torque += -_roll * m_rollSpeed * transform.forward;

        // This takes the total torque of pitch, yaw and roll and applies it as a force to the player rigidbody
        m_rbPlayer.AddTorque(torque);

        m_rbPlayer.AddForce(m_currentSpeed * transform.forward);

        m_rbPlayer.AddForce(m_negativeSpeed * (-m_rbPlayer.velocity.normalized));
    }
}
