using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrustEffectController : MonoBehaviour
{
    public PlayerMovement player;

    [SerializeField,Header ("Engine Panels")]
    private GameObject[] m_emissionPlane;
    [SerializeField, Header("Engine Light Effects")]
    private Light[] m_engineGlow;
    [SerializeField, Header("Normal Particle Effects")]
    private ParticleSystem[] m_engineParticles;
    [SerializeField, Header("High Speed Particle Effects")]
    private ParticleSystem[] m_highSpeedEngineParticles;

    private float[] m_maxLightIntensity;
    private float[] m_maxEmission;
    private float[] m_maxHighSpeedEmission;
    private float[] m_emissionLifetime;
    private float[] m_highSpeedEmissionLifetime;

    private float m_lerpSpeed = 0.4f;
    private Material m_planeMaterial;
    private float m_maximumEnginePower;
    private float m_currentEnginePower;
    private float m_currentAcceleration;
    private float m_maxAcceleration;
    private bool m_engineOff;


    void Start()
    {
        m_engineOff = false;

        m_maxLightIntensity = new float[m_engineGlow.Length];
        m_maxEmission = new float[m_engineParticles.Length];
        m_maxHighSpeedEmission = new float[m_highSpeedEngineParticles.Length];
        m_emissionLifetime = new float[m_engineParticles.Length];
        m_highSpeedEmissionLifetime = new float[m_highSpeedEngineParticles.Length];




        //Set Plane Material and make it almost transparent.
        m_planeMaterial = m_emissionPlane[0].GetComponent<MeshRenderer>().material;
        m_planeMaterial.color = new Color(m_planeMaterial.color.r, m_planeMaterial.color.g, m_planeMaterial.color.b, 2);

        //Save the light intensity of all lights.
        for (int i = 0; i < m_engineGlow.Length; i++)
        {
            Debug.Log("", m_engineGlow[i]);
            m_maxLightIntensity[i] = m_engineGlow[i].intensity;
            m_engineGlow[i].intensity = 0;
        }


        //Set the max emission and lifetime of all normal particle systems
        var _emission = m_engineParticles[0].emission;
        for (int i=0; i < m_engineParticles.Length; i++)
        {
            m_maxEmission[i] = m_engineParticles[i].emission.rateOverTime.constant;
            m_emissionLifetime[i] = m_engineParticles[i].main.startLifetime.constant;

            //Set emission to 0
            _emission = m_engineParticles[i].emission;
            _emission.rateOverTime = 0;
        }

        //Set the max emission of all high speed particle systems
        var _highSpeedEmission = m_highSpeedEngineParticles[0].emission;
        for (int i = 0; i < m_highSpeedEngineParticles.Length; i++)
        {
            m_maxHighSpeedEmission[i] = m_highSpeedEngineParticles[i].emission.rateOverTime.constant;
            m_highSpeedEmissionLifetime[i] = m_highSpeedEngineParticles[i].main.startLifetime.constant;

            //Set emission to 0
            _highSpeedEmission = m_highSpeedEngineParticles[i].emission;
            _highSpeedEmission.rateOverTime = 0;

        }

        //Set the max acceleration to 
        m_maxAcceleration = player.MaxAcceleration;
        m_maximumEnginePower = player.MaxAcceleration * 100f;
        m_currentAcceleration = 0;

    }


    // Update is called once per frame
    void Update()
    {
        float _currentAcceleration = player.CurrentSpeed;

        //Simulates a new cap of the highest possible power. This lengthens the time that the charging up animation occurs.
        if (_currentAcceleration > 0 && m_currentEnginePower < m_maximumEnginePower)
        {
            m_currentEnginePower += _currentAcceleration;
        }
        else if (_currentAcceleration >= m_maximumEnginePower)
            {
                m_currentEnginePower = m_maximumEnginePower;
            }
        else if (_currentAcceleration == 0)
            {
                m_currentEnginePower = 0.0f;
            }
        float _powerPercentage = m_currentEnginePower / m_maximumEnginePower;

        #region Engine VFX
        if (m_engineOff == false)
        {
            //Lerp colour of Blue Light. Charging engine up animates quicker than cooling down.
            float _intensity;
            for (int i=0; i< m_engineGlow.Length; i++)
            {
                _intensity = m_maxLightIntensity[i] * _powerPercentage;
                if (m_engineGlow[i].intensity <= _intensity)
                {
                    m_engineGlow[i].intensity = Mathf.Lerp(m_engineGlow[i].intensity, _intensity, m_lerpSpeed);
                }
                else
                {
                    m_engineGlow[i].intensity = Mathf.Lerp(m_engineGlow[i].intensity, _intensity, m_lerpSpeed * 0.4f);
                }
            }



            //Lerp the normal particle system. Charging up engine has a higher increase than cooling down.
            var _emission = m_engineParticles[0].emission;
            
            for (int i = 0; i < m_engineParticles.Length; i++)
            {
                _emission = m_engineParticles[i].emission;
                float _currentEmissionRate = _emission.rateOverTime.constant;
                float _targetEmissionRate = m_maxEmission[i] * _powerPercentage;
                _emission.rateOverTime = Mathf.Lerp(_emission.rateOverTime.constant, m_maxEmission[i] * _powerPercentage, m_lerpSpeed);
                if (_currentEmissionRate <= _targetEmissionRate)
                {
                    _emission.rateOverTime = Mathf.Lerp(_currentEmissionRate, _targetEmissionRate, m_lerpSpeed);
                }
                else
                {
                    _emission.rateOverTime = Mathf.Lerp(_currentEmissionRate, _targetEmissionRate, m_lerpSpeed * 0.4f);
                }

            }


            //Lerp the high speed particle system. Charging up engine has a higher increase than cooling down.
            var _highSpeedEmission = m_highSpeedEngineParticles[0].emission;

            for (int i = 0; i < m_highSpeedEngineParticles.Length; i++)
            {
                _highSpeedEmission = m_highSpeedEngineParticles[i].emission;
                float _currentHighSpeedEmissionRate = _highSpeedEmission.rateOverTime.constant;
                float _targetHighSpeedEmissionRate = m_maxHighSpeedEmission[i] * (_powerPercentage - 0.5f);
                if (_currentHighSpeedEmissionRate <= _targetHighSpeedEmissionRate && _powerPercentage > 0.5)
                {
                    _highSpeedEmission.rateOverTime = Mathf.Lerp(_currentHighSpeedEmissionRate, _targetHighSpeedEmissionRate, m_lerpSpeed);
                }
                else
                {
                    _highSpeedEmission.rateOverTime = Mathf.Lerp(_currentHighSpeedEmissionRate, _targetHighSpeedEmissionRate, m_lerpSpeed * 0.4f);
                }
            }


            //Lerp colour of Blue Panel. Charging engine up animates quicker than cooling down.
            Color _emissionColour = m_planeMaterial.color;
            float _transparency = 2 + _powerPercentage * 255;
            Color _beforeColor = new Color(_emissionColour.r, _emissionColour.g, _emissionColour.b, _emissionColour.a);
            Color _afterColor = new Color(_emissionColour.r, _emissionColour.g, _emissionColour.b, _transparency);
            if (_emissionColour.a <= _transparency)
            {
                m_planeMaterial.color = Color.Lerp(_beforeColor, _afterColor, m_lerpSpeed);
            }
            else
            {
                m_planeMaterial.color = Color.Lerp(_beforeColor, _afterColor, m_lerpSpeed * 0.4f);
            }
        }

        if( m_engineOff == true)
        {
            Color _emissionColour = m_planeMaterial.color;
            Color _beforeColor = new Color(_emissionColour.r, _emissionColour.g, _emissionColour.b, _emissionColour.a);
            Color _afterColor = new Color(_emissionColour.r, _emissionColour.g, _emissionColour.b, 0);
            m_planeMaterial.color = Color.Lerp(_beforeColor, _afterColor, m_lerpSpeed * 0.5f);
        }
            #endregion
    }

    public void EngineOff()
    {
        m_engineOff = true;


        var _emission = m_engineParticles[0].emission;
        for (int i = 0; i < m_engineParticles.Length; i++)
        {
            //Set emission to 0
            _emission = m_engineParticles[i].emission;
            _emission.rateOverTime = 0;
        }



        var _highSpeedEmission = m_highSpeedEngineParticles[0].emission;
        for (int i = 0; i < m_highSpeedEngineParticles.Length; i++)
        {
            //Set emission to 0
            _highSpeedEmission = m_highSpeedEngineParticles[i].emission;
            _highSpeedEmission.rateOverTime = 0;

        }


        for (int i = 0; i< m_engineGlow.Length; i++)
        {
            m_engineGlow[i].intensity = Mathf.Lerp(m_engineGlow[i].intensity, 0, m_lerpSpeed * 3);
        }
    }

    public void BoostOn()
    {
        for (int i = 0; i < m_engineGlow.Length; i++)
        {
            m_engineGlow[i].intensity = Mathf.Lerp(m_engineGlow[i].intensity, 50000, 0.1f);
        }


        //Boost normal Particles
        var _emission = m_engineParticles[0].emission;
        var _emissionMain = m_engineParticles[0].main;
        for(int i = 0;i <m_engineParticles.Length; i++)
        {
            _emission = m_engineParticles[i].emission;
            _emissionMain = m_engineParticles[i].main;
            _emission.rateOverTime = 1000;
            _emissionMain.startLifetime = _emissionMain.startLifetime.constant * 2f;
            _emissionMain.simulationSpeed = 2.5f;
        }

        //Boost High speed 
        var _highSpeedEmission = m_highSpeedEngineParticles[0].emission;
        var _highSpeedEmissionMain = m_highSpeedEngineParticles[0].main;
        for (int i = 0; i < m_highSpeedEngineParticles.Length; i++)
        {
            _highSpeedEmission = m_highSpeedEngineParticles[i].emission;
            _highSpeedEmissionMain = m_highSpeedEngineParticles[i].main;
            _highSpeedEmission.rateOverTime = 800;
            _highSpeedEmissionMain.startLifetime = _emissionMain.startLifetime.constant * 2f;
            _highSpeedEmissionMain.simulationSpeed = 2.5f;
        }

        //make panel super blue
        Color _emissionColour = m_planeMaterial.color;
        Color _beforeColor = new Color(_emissionColour.r, _emissionColour.g, _emissionColour.b, _emissionColour.a);
        Color _afterColor = new Color(_emissionColour.r, _emissionColour.g, _emissionColour.b, 255);
        m_planeMaterial.color = Color.Lerp(_beforeColor, _afterColor, m_lerpSpeed);

        //Reset everything to normal once boost is done.
        Invoke("ResetParticles", 0.6f);
    }

    private void ResetParticles()
    {
        m_engineOff = false;

        var _emissionMain = m_engineParticles[0].main;
        for (int i = 0; i < m_engineParticles.Length; i++)
        {
            _emissionMain = m_engineParticles[i].main;
            _emissionMain.simulationSpeed = 1f;
            _emissionMain.startLifetime = m_emissionLifetime[i];
        }

        //Boost High speed 
        var _highSpeedEmissionMain = m_highSpeedEngineParticles[0].main;
        for (int i = 0; i < m_highSpeedEngineParticles.Length; i++)
        {
            _highSpeedEmissionMain = m_highSpeedEngineParticles[i].main;
            _highSpeedEmissionMain.simulationSpeed = 1f;
            _highSpeedEmissionMain.startLifetime = m_highSpeedEmissionLifetime[i];
        }

    }
}
