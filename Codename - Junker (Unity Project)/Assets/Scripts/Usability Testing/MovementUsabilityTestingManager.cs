﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class MovementUsabilityTestingManager : MonoBehaviour
{
    private static MovementUsabilityTestingManager s_instance;

    public static MovementUsabilityTestingManager Instance { get => s_instance; }
    public float MaxSpeed { get => m_maxSpeed; }
    public float Acceleration { get => m_acceleration;}
    public float Damping { get => m_damping;}
    public float RollSpeed { get => m_rollSpeed;}
    public float PitchSpeed { get => m_pitchSpeed;}
    public float YawSpeed { get => m_yawSpeed;}
    
    [SerializeField, Header("Configuration")]
    private GameObject m_displayPanel;
    [SerializeField, Tooltip ("How high the multiplier. Default is 2.")]
    private float m_maximumMultiplier;
    [SerializeField, Header("Sliders")]
    private Slider m_maxSpeedSlider;
    [SerializeField]
    private Slider m_accelerationSlider, m_dampingSlider, m_rollSlider, m_pitchSlider, m_yawSlider;
    [SerializeField, Header("ValueDisplays")]
    private TextMeshProUGUI m_maxSpeedText;
    [SerializeField]
    private TextMeshProUGUI m_accelerationText, m_dampingText, m_rollText, m_pitchText, m_yawText;

    private float m_maxSpeed, m_acceleration, m_damping, m_rollSpeed, m_pitchSpeed, m_yawSpeed;
    private bool m_showingVariables;
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

    }

    // Start is called before the first frame update
    void Start()
    {
        m_showingVariables = false;
        m_displayPanel.SetActive(false);

        m_maxSpeedSlider.maxValue = m_maximumMultiplier;
        m_accelerationSlider.maxValue = m_maximumMultiplier;
        m_dampingSlider.maxValue = m_maximumMultiplier;
        m_rollSlider.maxValue = m_maximumMultiplier;
        m_pitchSlider.maxValue = m_maximumMultiplier;
        m_yawSlider.maxValue = m_maximumMultiplier;
    }

        // Update is called once per frame
    void Update()
    {
        m_maxSpeed = m_maxSpeedSlider.value;
        m_maxSpeed = Mathf.Round(m_maxSpeed * 100f) / 100f;

        m_acceleration = m_accelerationSlider.value;
        m_acceleration = Mathf.Round(m_acceleration * 100f) / 100f;

        m_damping = m_dampingSlider.value;
        m_damping = Mathf.Round(m_damping * 100f) / 100f;

        m_rollSpeed = m_rollSlider.value;
        m_rollSpeed = Mathf.Round(m_rollSpeed * 100f) / 100f;

        m_pitchSpeed = m_pitchSlider.value;
        m_pitchSpeed = Mathf.Round(m_pitchSpeed * 100f) / 100f;

        m_yawSpeed = m_yawSlider.value;
        m_yawSpeed = Mathf.Round(m_yawSpeed * 100f) / 100f;

        m_maxSpeedText.text = m_maxSpeed.ToString();
        m_accelerationText.text = m_acceleration.ToString();
        m_dampingText.text = m_damping.ToString();
        m_rollText.text = m_rollSpeed.ToString();
        m_pitchText.text = m_pitchSpeed.ToString();
        m_yawText.text = m_yawSpeed.ToString();

    }
    public void toggleVariables()
    {
        if (m_showingVariables == false) { m_displayPanel.SetActive(true); m_showingVariables = true; }
        else { m_displayPanel.SetActive(false); m_showingVariables = false; }
    }
    public void SaveValues(bool _reset)
    {
         string _results = m_maxSpeed + "," + m_acceleration + "," + m_damping + "," + m_rollSpeed + "," + m_pitchSpeed + "," + m_yawSpeed;
         string path = "Assets/Testing Results/MovementSystemTesting.txt";

         StreamWriter writer = new StreamWriter(path, true);
         writer.WriteLine(_results);
         writer.Close();
        if(_reset == true)
        {
            resetValues();
        }
    }
    public void resetValues()
    {
        m_maxSpeedSlider.value = 1;

        m_accelerationSlider.value = 1;

        m_dampingSlider.value = 1;

        m_rollSlider.value = 1;

        m_pitchSlider.value = 1;

        m_yawSlider.value = 1;
    }
}
