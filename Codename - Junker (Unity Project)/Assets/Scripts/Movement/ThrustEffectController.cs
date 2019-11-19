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
    private Material[] m_planeMaterial;



    private float m_lerpSpeed = 0.4f;
    private float m_maximumEnginePower;
    private float m_currentEnginePower;
    private float m_currentAcceleration;
    private float m_maxAcceleration;
    private bool m_engineOff;
    private bool m_currentlyBoosting;


    void Start()
    {
        if (player != null)
        {
            m_engineOff = false;
            m_currentlyBoosting = false;

            m_maxLightIntensity = new float[m_engineGlow.Length];
            m_maxEmission = new float[m_engineParticles.Length];
            m_maxHighSpeedEmission = new float[m_highSpeedEngineParticles.Length];
            m_emissionLifetime = new float[m_engineParticles.Length];
            m_highSpeedEmissionLifetime = new float[m_highSpeedEngineParticles.Length];
            m_planeMaterial = new Material[m_emissionPlane.Length];

            //Play all engine sounds and set appropriate values;
            AudioManager.Instance.Play("EngineIdle");
            AudioManager.Instance.Play("EngineAccelerating");
            AudioManager.Instance.Volume("EngineAccelerating", 0.0f);
            AudioManager.Instance.Play("EngineOverworking");
            AudioManager.Instance.Volume("EngineOverworking", 0.0f);
            AudioManager.Instance.Play("EngineBacking");
            AudioManager.Instance.Volume("EngineBacking", 0.0f);



            //Set Plane Material and make it almost transparent.
            for (int i = 0; i < m_emissionPlane.Length; i++)
            {
                m_planeMaterial[i] = m_emissionPlane[i].GetComponent<MeshRenderer>().material;
                m_planeMaterial[i].color = new Color(m_planeMaterial[i].color.r, m_planeMaterial[i].color.g, m_planeMaterial[i].color.b, 2);
            }


            //Save the light intensity of all lights.
            for (int i = 0; i < m_engineGlow.Length; i++)
            {
                m_maxLightIntensity[i] = m_engineGlow[i].intensity;
                m_engineGlow[i].intensity = 0;
            }


            //Set the max emission and lifetime of all normal particle systems
            var _emission = m_engineParticles[0].emission;
            for (int i = 0; i < m_engineParticles.Length; i++)
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

    }


    // Update is called once per frame
    void Update()
    {
        if (player != null)
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
                for (int i = 0; i < m_engineGlow.Length; i++)
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
                for (int i = 0; i < m_emissionPlane.Length; i++)
                {
                    Color _emissionColour = m_planeMaterial[i].color;
                    float _transparency = 2 + _powerPercentage * 255;
                    Color _beforeColor = new Color(_emissionColour.r, _emissionColour.g, _emissionColour.b, _emissionColour.a);
                    Color _afterColor = new Color(_emissionColour.r, _emissionColour.g, _emissionColour.b, _transparency);
                    if (_emissionColour.a <= _transparency)
                    {
                        m_planeMaterial[i].color = Color.Lerp(_beforeColor, _afterColor, m_lerpSpeed);
                    }
                    else
                    {
                        m_planeMaterial[i].color = Color.Lerp(_beforeColor, _afterColor, m_lerpSpeed * 0.4f);
                    }

                }


            }

            if (m_engineOff == true)
            {
                for (int i = 0; i < m_emissionPlane.Length; i++)
                {
                    Color _emissionColour = m_planeMaterial[i].color;
                    Color _beforeColor = new Color(_emissionColour.r, _emissionColour.g, _emissionColour.b, _emissionColour.a);
                    Color _afterColor = new Color(_emissionColour.r, _emissionColour.g, _emissionColour.b, 0);
                    m_planeMaterial[i].color = Color.Lerp(_beforeColor, _afterColor, m_lerpSpeed * 0.5f);
                }
            }
            #endregion

            #region EngineSoundFX
            float _idleVolume = 0.3f + (0.4f * _powerPercentage);
            float _idlePitch = 0.3f + (0.1f * _powerPercentage);

            float _backingVolume = 0.2f * (_powerPercentage - 0.4f);
            float _acceleratingVolume = 0.2f * _powerPercentage;
            float _overworkingVolume = 0.2f * (_powerPercentage - 0.3f);

            float _engineIdleVolume = AudioManager.Instance.getVolume("EngineIdle");
            float _engineIdlePitch = AudioManager.Instance.getPitch("EngineIdle");
            float _currentBackingVolume = AudioManager.Instance.getVolume("EngineBacking");
            float _currentBackingPitch = AudioManager.Instance.getPitch("EngineBacking");
            float _currentAcceleratingVolume = AudioManager.Instance.getVolume("EngineAccelerating");
            float _currentOverworkingVolume = AudioManager.Instance.getVolume("EngineOverworking");


            if (m_engineOff == false)
            {
                AudioManager.Instance.Volume("EngineIdle", Mathf.Lerp(_engineIdleVolume, _idleVolume, .2f));
                AudioManager.Instance.Pitch("EngineIdle", Mathf.Lerp(_engineIdlePitch, _idlePitch, .2f));
                AudioManager.Instance.Volume("EngineBacking", Mathf.Lerp(_currentBackingVolume, _backingVolume, .2f));
                AudioManager.Instance.Volume("EngineAccelerating", Mathf.Lerp(_currentAcceleratingVolume, _acceleratingVolume, .05f));
                AudioManager.Instance.Volume("EngineOverworking", Mathf.Lerp(_currentOverworkingVolume, _overworkingVolume, .05f));
            }

            if (m_engineOff == true && m_currentlyBoosting == false && (_engineIdleVolume > 0.0f || _engineIdlePitch > 0.1f || _currentBackingVolume > 0.0f || _currentAcceleratingVolume > 0.0f || _currentOverworkingVolume > 0.0f))
            {
                AudioManager.Instance.Pitch("EngineIdle", Mathf.Lerp(AudioManager.Instance.getPitch("EngineIdle"), 0.1f, .1f));
                AudioManager.Instance.Volume("EngineIdle", Mathf.Lerp(AudioManager.Instance.getVolume("EngineIdle"), 0f, .002f));
                AudioManager.Instance.Pitch("EngineBacking", Mathf.Lerp(_currentBackingPitch, 0.1f, .1f));
                AudioManager.Instance.Volume("EngineBacking", Mathf.Lerp(_currentBackingVolume, 0, .002f));
                AudioManager.Instance.Volume("EngineAccelerating", Mathf.Lerp(_currentAcceleratingVolume, 0, .3f));
                AudioManager.Instance.Volume("EngineOverworking", Mathf.Lerp(_currentOverworkingVolume, 0, .3f));
            }

            #endregion
        }
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
        int _boostedCount = PlayerPrefs.GetInt("BoostedCount");
        PlayerPrefs.SetInt("BoostedCount", _boostedCount + 1);
        m_currentlyBoosting = true;
        AudioManager.Instance.Play("EngineBoost");

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
        for (int i = 0; i < m_emissionPlane.Length; i++)
        {
            Color _emissionColour = m_planeMaterial[i].color;
            Color _beforeColor = new Color(_emissionColour.r, _emissionColour.g, _emissionColour.b, _emissionColour.a);
            Color _afterColor = new Color(_emissionColour.r, _emissionColour.g, _emissionColour.b, 255);
            m_planeMaterial[i].color = Color.Lerp(_beforeColor, _afterColor, m_lerpSpeed);
        }


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
        m_currentlyBoosting = false;
    }
}
