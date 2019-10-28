using System.Collections;
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
    [SerializeField]
    private bool m_saveData;
    [SerializeField, Tooltip ("How high the multiplier. Default is 2.")]
    private float m_maximumMultiplier;
    [SerializeField, Tooltip("Response Text")]
    private GameObject m_responseText;

    private string BASE_URL = "https://docs.google.com/forms/d/e/1FAIpQLSc2P23z1TFQ4UZMUfm4YGQWZ4LXAAkZbrzmrvus0I0PfYHqew/formResponse";
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

        ResetValues();

        m_maxSpeedSlider.maxValue = m_maximumMultiplier;
        m_accelerationSlider.maxValue = m_maximumMultiplier;
        m_dampingSlider.maxValue = m_maximumMultiplier;
        m_rollSlider.maxValue = m_maximumMultiplier;
        m_pitchSlider.maxValue = m_maximumMultiplier;
        m_yawSlider.maxValue = m_maximumMultiplier;

        if(m_saveData == false)
        {
            Debug.Log("<color=red><b>Note:</b></color>Usability data is <b>NOT</b> saving to spreadsheet");
        }
        else
        {
            Debug.Log("<color=green><b>Note:</b></color>Usability data <b>IS</b> saving to spreadsheet");
        }
    }

    IEnumerator Post(float maxspeed, float acceleration, float damping, float rollspeed, float pitchspeed, float yawspeed)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.1419638895", maxspeed.ToString());
        form.AddField("entry.231350589", acceleration.ToString());
        form.AddField("entry.873326976", damping.ToString());
        form.AddField("entry.1235456469", rollspeed.ToString());
        form.AddField("entry.624428431", pitchspeed.ToString());
        form.AddField("entry.2000891573", yawspeed.ToString());
        byte[] rawData = form.data;
        WWW www = new WWW(BASE_URL,rawData);
        yield return www;

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

        if (m_saveData == false)
        {
            m_responseText.GetComponent<TextMeshProUGUI>().text = "Thank you for testing! Data has not been saved";
            m_responseText.GetComponent<TextMeshProUGUI>().fontSize = 15;
            m_responseText.GetComponent<Animator>().Play("UsabilityTestingTextResponse");
        }
        else
        {
            StartCoroutine(Post(m_maxSpeed, m_acceleration, m_damping, m_rollSpeed, m_pitchSpeed, m_yawSpeed));
            m_responseText.GetComponent<TextMeshProUGUI>().text = "Thank you for testing!";
            m_responseText.GetComponent<TextMeshProUGUI>().fontSize = 36;
            m_responseText.GetComponent<Animator>().Play("UsabilityTestingTextResponse");
        }
        if (_reset == true)
        {
            ResetValues();
        }
        m_displayPanel.SetActive(false);
        m_showingVariables = false;
    }
    public void ResetValues()
    {
        m_maxSpeedSlider.value = 0.1f;

        m_accelerationSlider.value = 0.1f;

        m_dampingSlider.value = 0.1f;

        m_rollSlider.value = 0.1f;

        m_pitchSlider.value = 0.1f;

        m_yawSlider.value = 0.1f;
    }
}
