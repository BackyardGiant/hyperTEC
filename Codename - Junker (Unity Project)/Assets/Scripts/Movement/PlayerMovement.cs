using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum ControlType { RollLeftAxis, YawLeftAxis }

    #region Speed
    [Header("Speed Settings")]
    [SerializeField, Tooltip("The maximum speed that the ship can fly with the best engine")]
    private float m_maxSpeedHigh;
    [SerializeField, Tooltip("The maximum speed that the ship can fly with the worst engine")]
    private float m_maxSpeedLow;
    [SerializeField]
    private float m_maxSpeed;
    [SerializeField, Tooltip("The maximum speed that the ship can accelerate with the best engine")]
    private float m_maxAccelerationHigh;
    [SerializeField, Tooltip("The maximum speed that the ship can accelerate with the best engine")]
    private float m_maxAccelerationLow;
    [SerializeField]
    private float m_maxAcceleration;
    [SerializeField, Tooltip("The current speed of the ship, between 0 and Max Speed")]
    private float m_currentSpeed;
    [SerializeField, Tooltip("The current negative speed of the ship, between 0 and Max Speed")]
    private float m_negativeSpeed;
    [SerializeField, Tooltip("The acceleration of the ship")]
    private float m_acceleration;
    [SerializeField, Tooltip("The decleration of the ship")]
    private float m_decleration;
    private float m_posativeClampedSpeed; // Forward force being applied
    private float m_negativeClampedSpeed; // Backwards force being applied


    #endregion

    #region Pitch, Yaw and Roll
    [Header("Pitch, Yaw and Roll speed")]
    [SerializeField, Tooltip("The speed of the massive boost with the best engine")]
    private float m_handlingHigh;
    [SerializeField, Tooltip("The speed of the massive boost with the worst engine")]
    private float m_handlingLow;
    [SerializeField, Tooltip("The speed of the massive boost with the worst engine")]
    private float m_handling;
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

    #region Damping
    [SerializeField, Header("Damping"), Tooltip("The speed that damping is applied")]
    private float m_dampingSpeed;
    [SerializeField, Tooltip("The correction force to counter act damping in the posative direction")]
    private float m_correctionForce;
    [SerializeField, Tooltip("The of angle at which damping will be applied (1 is same direction)"), Range(0.5f, 1)]
    private float m_dampingAngleThreshold;
    #endregion

    #region Slow Mo Manager
    private Vector3 m_velocityBeforeSlow;
    private Vector3 m_angleVelocityBeforeSlow;
    private Vector3 m_velocityAfterSlow;
    private Vector3 m_angleVelocityAfterSlow;
    private float m_velocityDifferenceBetweenSlow;
    private float m_angleVelocityDifferenceBetweenSlow;
    #endregion

    #region Super Boost
    [SerializeField, Header("Super Boost"), Tooltip("The speed of the massive boost with the best engine")]
    private float m_boostSpeedHigh;
    [SerializeField, Tooltip("The speed of the massive boost with the worst engine")]
    private float m_boostSpeedLow;
    [SerializeField, Tooltip("The speed of the massive boost")]
    private float m_boostSpeed;
    [SerializeField, Tooltip("The speed of the massive boost")]
    private float m_boostDamping;
    [SerializeField, Tooltip("The maximum velocity of the ship")]
    private float m_maxBoostSpeed;
    [SerializeField, Tooltip("The time the boost is active for")]
    private float m_boostTime;
    private bool m_killedEngine;
    private bool m_engageBoost;
    private bool m_boostOn;
    #endregion

    #region Engine Particle System
    [Header("Particle System Events")]
    public GameEvent boostOn;
    public GameEvent engineOff;
    #endregion

    [SerializeField, Header("Controls and Engine")]
    private ControlType m_controlScheme;

    [SerializeField, Header("UI correction")]
    private float m_uiCorrection;

    [SerializeField]
    private bool m_inUserTesting;

    public float CurrentSpeed { get => m_currentSpeed; }
    public float MaxAcceleration { get => m_maxAcceleration; }
    public ControlType ControlScheme { get => m_controlScheme; set => m_controlScheme = value; }
    public bool InvertY { get => m_invertY; set => m_invertY = value; }

    private void Awake()
    {
        if (m_invertY)
        {
            m_invertScale = -1;
        }

        m_rbPlayer = gameObject.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        m_rbPlayer.inertiaTensor = new Vector3(0.2f, 0.2f, 0.2f); //Used to make the player always react the same to torque no matter the size or shape of the collider
        if (PlayerPrefs.GetInt("ControlScheme") == 0)
        {
            m_controlScheme = ControlType.RollLeftAxis;
        }
        if (PlayerPrefs.GetInt("ControlScheme") == 1)
        {
            m_controlScheme = ControlType.YawLeftAxis;
        }
        UpdateValues();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GameSpeed != 0 && PlayerInventoryManager.Instance.EquippedEngine != null)
        {
            // Restets posative force application to the ship (If you let go of go forwards it stops applying force)
            if (Input.GetButtonUp("Throttle Up"))
            {
                m_posativeClampedSpeed = 0;
                m_currentSpeed = 0;
            }
            if (Input.GetButtonUp("Throttle Down"))
            {
                m_posativeClampedSpeed = 0;
                m_negativeSpeed = 0;
            }

            if (Input.GetAxis("MacroEngine") > 0.1f && m_killedEngine)
            {
                m_killedEngine = false;
                m_engageBoost = true;
            }

            if (Input.GetAxis("MacroEngine") < -0.1f && !m_killedEngine && !m_boostOn && !m_engageBoost)
            {
                engineOff.Raise();
                m_killedEngine = true;
            }

            if (Input.GetButton("Throttle Up"))
            {
                m_posativeClampedSpeed += m_acceleration / 100;
                m_posativeClampedSpeed = Mathf.Clamp(m_posativeClampedSpeed, 0, 1);

                m_currentSpeed = m_posativeClampedSpeed * m_maxAcceleration;

            }
            if (Input.GetButton("Throttle Down"))
            {
                m_negativeClampedSpeed += m_decleration / 100;
                m_negativeClampedSpeed = Mathf.Clamp(m_negativeClampedSpeed, 0, 1);

                m_negativeSpeed = m_negativeClampedSpeed * m_maxAcceleration;
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                GameManager.Instance.SetSlowMo(0.5f);
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                GameManager.Instance.SetNormalSpeed();
            }
        }
    }

    private void FixedUpdate()
    {
        if (m_inUserTesting)
        {
            UpdateValues();
        }

        if (m_engageBoost)
        {
            boostOn.Raise();
            m_boostOn = true;
            m_rbPlayer.velocity = new Vector3(0,0,0);
            m_engageBoost = false;
            StartCoroutine(boostTimer());
        }

        float _pitch = 0;
        float _yaw = 0;
        float _roll = 0;

        switch (m_controlScheme)
        {
            case ControlType.YawLeftAxis:
                if (InvertY)
                {
                    _pitch = -Input.GetAxis("Vertical");
                }
                else if (!InvertY)
                {
                    _pitch = Input.GetAxis("Vertical");
                }
                _yaw = Input.GetAxis("Horizontal");
                _roll = Input.GetAxis("Axis12");

                if (Input.GetButton("RightBumper"))
                {
                    _roll += 1;
                }
                if (Input.GetButton("LeftBumper"))
                {
                    _roll -= 1;
                }
                break;
            case ControlType.RollLeftAxis:
                if (InvertY)
                {
                    _pitch = -Input.GetAxis("Vertical");
                }
                else if (!InvertY)
                {
                    _pitch = Input.GetAxis("Vertical");
                }
                _roll = Input.GetAxis("Horizontal");
                _yaw = Input.GetAxis("Axis12");

                if (Input.GetButton("RightBumper"))
                {
                    _yaw += 1;
                }
                if (Input.GetButton("LeftBumper"))
                {
                    _yaw -= 1;
                }
                break;

        }


        // We accumulate torque forces into this variable:
        var torque = Vector3.zero;
        if (PlayerInventoryManager.Instance.EquippedEngine != null)
        {
            // Add torque for the pitch based on the pitch input.
            torque += _pitch * m_pitchSpeed * m_handling * transform.right;
            // Add torque for the yaw based on the yaw input.
            torque += _yaw * m_yawSpeed * m_handling * transform.up;
            // Add torque for the roll based on the roll input.
            torque += -_roll * m_rollSpeed * m_handling * transform.forward;
        }
        else
        {
            // Add torque for the pitch based on the pitch input.
            torque += _pitch * m_pitchSpeed * m_handling * transform.right * 0.33f;
            // Add torque for the yaw based on the yaw input.
            torque += _yaw * m_yawSpeed * m_handling * transform.up * 0.33f;
            // Add torque for the roll based on the roll input.
            torque += -_roll * m_rollSpeed * m_handling * transform.forward * 0.33f;
        }

        if (m_boostOn)
        {
            if (m_rbPlayer.velocity.magnitude < m_maxBoostSpeed)
            {
                m_rbPlayer.AddForce(m_boostSpeed * transform.forward * GameManager.Instance.GameSpeed);
            }
            m_rbPlayer.AddForce(transform.up * GameManager.Instance.GameSpeed * m_uiCorrection / 80f);
        }

        if (!m_boostOn && !m_engageBoost)
        {
            // This takes the total torque of pitch, yaw and roll and applies it as a force to the player rigidbody
            m_rbPlayer.AddTorque(torque * GameManager.Instance.GameSpeed);
        }

        if (!m_boostOn && !m_killedEngine && !m_engageBoost)
        {
            ApplyDamping();

            if (m_rbPlayer.velocity.magnitude < m_maxSpeed)
            {
                m_rbPlayer.AddForce(m_currentSpeed * transform.forward * GameManager.Instance.GameSpeed);
            }
            else if (m_rbPlayer.velocity.magnitude > m_maxSpeed + 10f)
            {
                m_rbPlayer.AddForce(m_boostDamping * (-m_rbPlayer.velocity.normalized) * GameManager.Instance.GameSpeed);
            }

            if (m_currentSpeed > 0)
            {
                m_rbPlayer.AddForce(transform.up * GameManager.Instance.GameSpeed * m_uiCorrection / 100f);
            }

            m_rbPlayer.AddForce(m_negativeSpeed * (-m_rbPlayer.velocity.normalized) * GameManager.Instance.GameSpeed);
        }
    }

    private void ApplyDamping()
    {
        Vector3 _velcoity = m_rbPlayer.velocity;
        Vector3 _targetVector = m_currentSpeed * transform.forward * GameManager.Instance.GameSpeed;
        float _dotProduct = Vector3.Dot(_velcoity.normalized, _targetVector.normalized);
        if (_targetVector.magnitude > 0.1f)
        {
            if (_dotProduct < m_dampingAngleThreshold)
            {
                m_rbPlayer.AddForce(m_dampingSpeed * -_velcoity * GameManager.Instance.GameSpeed); //damping force
                m_rbPlayer.AddForce(m_correctionForce * transform.forward * GameManager.Instance.GameSpeed); //correction force
            }
        }
    }

    /// <summary>
    /// Saves forces before slow mo
    /// </summary>
    public void RespondToPreSlowMo()
    {
        m_velocityBeforeSlow = m_rbPlayer.velocity;
        m_angleVelocityBeforeSlow = m_rbPlayer.angularVelocity;
    }

    /// <summary>
    /// Applies slow mo and saves forces after it has been applied
    /// </summary>
    public void RespondToPostSlowMo()
    {
        m_rbPlayer.velocity *= GameManager.Instance.GameSpeed;
        m_rbPlayer.angularVelocity *= GameManager.Instance.GameSpeed;

        m_velocityAfterSlow = m_rbPlayer.velocity;
        m_angleVelocityAfterSlow = m_rbPlayer.angularVelocity;

        m_velocityDifferenceBetweenSlow = m_velocityBeforeSlow.magnitude - m_velocityAfterSlow.magnitude;
        m_angleVelocityDifferenceBetweenSlow = m_angleVelocityBeforeSlow.magnitude - m_angleVelocityAfterSlow.magnitude;
    }

    /// <summary>
    /// Returns lost force to object using the difference between before and after slow mo
    /// </summary>
    public void RespondToNormalSpeed()
    {
        m_rbPlayer.velocity += m_rbPlayer.velocity.normalized * m_velocityDifferenceBetweenSlow;
        m_rbPlayer.angularVelocity += m_rbPlayer.angularVelocity.normalized * m_angleVelocityDifferenceBetweenSlow;
    }

    public IEnumerator boostTimer()
    {
        yield return new WaitForSeconds(m_boostTime);
        m_boostOn = false;
    }

    //Used for user testing
    public void UpdateValues()
    {
        EngineData _engine = null;

        try
        {
            _engine = PlayerInventoryManager.Instance.EquippedEngine;
        }
        catch { }

        float _rangeMaxSpeed = (m_maxSpeedHigh - m_maxSpeedLow);
        float _rangeMaxAccleration = (m_maxAccelerationHigh - m_maxAccelerationLow);
        float _rangeBoost = (m_boostSpeedHigh - m_boostSpeedLow);
        float _rangeHandling = (m_handlingHigh - m_handlingLow);

        float _maxSpeedPercentage;
        float _maxAcclerationPercentage;
        float _boostPercentage;
        float _handlingPercentage;

        if (_engine != null)
        {
            _maxSpeedPercentage = _engine.TopSpeed / 100;
            _maxAcclerationPercentage = _engine.Acceleration / 100;
            _boostPercentage = _engine.BoostPower / 100;
            _handlingPercentage = _engine.Handling / 100;

            m_maxSpeed = m_maxSpeedLow + (_maxSpeedPercentage * _rangeMaxSpeed);
            m_maxAcceleration = m_maxAccelerationLow + (_maxAcclerationPercentage * _rangeMaxAccleration);
            m_boostSpeed = m_boostSpeedLow + (_boostPercentage * _rangeBoost);
            m_handling = m_handlingLow + (_handlingPercentage * _rangeHandling);

        }
    }
}
