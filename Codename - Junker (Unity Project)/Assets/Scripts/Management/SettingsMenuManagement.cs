using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SettingsMenuManagement : MonoBehaviour
{
    [SerializeField, Tooltip("Main Menu Object")]
    private GameObject m_mainMenu;
    [SerializeField, Tooltip("All of the items on the menu to sort through")]
    private GameObject[] m_menuOptions;
    [SerializeField, Tooltip("All of the audio values")]
    private TextMeshProUGUI[] m_values;
    [SerializeField, Tooltip("All of the audio up arrows")]
    private GameObject[] m_upArrows;
    [SerializeField, Tooltip("All of the audio down arrows")]
    private GameObject[] m_downArrows;
    [SerializeField, Tooltip("UI sounds. First should be the scroll, second should be select")]
    private AudioClip[] m_sounds;
    [SerializeField, Tooltip("Audio Mixers")]
    private AudioMixer m_mixer;
    [SerializeField]
    private TextMeshProUGUI m_controlSchemeValue;
    [SerializeField]
    private GameObject m_controlScheme1;
    [SerializeField]
    private GameObject m_controlScheme2;
    [SerializeField]
    private TextMeshProUGUI m_invertControlValue;


    private bool m_readyForInput;
    private int m_selectedIndex;
    private AudioSource m_UIAudio;
    private bool m_audioCooldown;

    // Start is called before the first frame update
    void Start()
    {
        m_selectedIndex = 0;
        m_audioCooldown = false;
        m_selectedIndex = 0;
        m_UIAudio = this.GetComponent<AudioSource>();

        m_values[0].text = PlayerPrefs.GetInt("masterVolume").ToString();
        m_values[1].text = PlayerPrefs.GetInt("musicVolume").ToString();
        m_values[2].text = PlayerPrefs.GetInt("fxVolume").ToString();
        m_values[3].text = PlayerPrefs.GetInt("uiVolume").ToString();
        m_controlSchemeValue.text = (PlayerPrefs.GetInt("ControlScheme") + 1).ToString();

        if(PlayerPrefs.GetInt("Invert", 0) == 0)
        {
            m_invertControlValue.text = "YES";
        }
        else if (PlayerPrefs.GetInt("Invert", 0) == 1)
        {
            m_invertControlValue.text = "NO";
        }


        m_mixer.SetFloat("masterVol", -80 + PlayerPrefs.GetInt("masterVolume"));
        m_mixer.SetFloat("musicVol", -80 + PlayerPrefs.GetInt("musicVolume"));
        m_mixer.SetFloat("fxVol", -80 + PlayerPrefs.GetInt("fxVolume"));
        m_mixer.SetFloat("uiVol", -80 + PlayerPrefs.GetInt("uiVolume"));

    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetAxis("MacroEngine") == 0) && (Input.GetAxis("Vertical") == 0))
        {
            m_readyForInput = true;
        }
        //Arrow Control
        for(int i = 0; i < m_values.Length; i++)
        {
            if (m_values[i].text == "0")
            {
                m_downArrows[i].SetActive(false);
            }
            else if (m_values[i].text == "80")
            {
                m_upArrows[i].SetActive(false);
            }
            else
            {
                m_downArrows[i].SetActive(true);
                m_upArrows[i].SetActive(true);
            }
        }
        if (m_readyForInput)
        {
            //If you've added an item to the menu and it's not selecting, it's because it doesn't exist here.
            switch (m_selectedIndex)
            {
                case 0:
                    m_controlScheme1.SetActive(false);
                    m_controlScheme2.SetActive(false);
                    AnimateBar(0);
                    CloseBar(1);
                    CloseBar(2);
                    CloseBar(3);
                    CloseBar(4);
                    CloseBar(5);
                    CloseBar(6);
                    break;
                case 1:
                    m_controlScheme1.SetActive(false);
                    m_controlScheme2.SetActive(false);
                    AnimateBar(1);
                    CloseBar(0);
                    CloseBar(2);
                    CloseBar(3);
                    CloseBar(4);
                    CloseBar(5);
                    break;
                case 2:
                    m_controlScheme1.SetActive(false);
                    m_controlScheme2.SetActive(false);
                    AnimateBar(2);
                    CloseBar(0);
                    CloseBar(1);
                    CloseBar(3);
                    CloseBar(4);
                    CloseBar(5);
                    break;
                case 3:
                    m_controlScheme1.SetActive(false);
                    m_controlScheme2.SetActive(false);
                    AnimateBar(3);
                    CloseBar(0);
                    CloseBar(1);
                    CloseBar(2);
                    CloseBar(4);
                    CloseBar(5);
                    break;
                case 4:
                    AnimateBar(4);
                    CloseBar(0);
                    CloseBar(1);
                    CloseBar(2);
                    CloseBar(3);
                    CloseBar(5);
                    if (m_controlSchemeValue.text == "1")
                    {
                        m_controlScheme2.SetActive(false);
                        m_controlScheme1.SetActive(true);
                    }
                    else
                    {
                        m_controlScheme1.SetActive(false);
                        m_controlScheme2.SetActive(true);
                    }
                    break;
                case 5:
                    m_controlScheme1.SetActive(false);
                    m_controlScheme2.SetActive(false);
                    AnimateBar(5);
                    CloseBar(0);
                    CloseBar(1);
                    CloseBar(2);
                    CloseBar(3);
                    CloseBar(4);
                    CloseBar(6);
                    break;
                case 6:
                    m_controlScheme1.SetActive(false);
                    m_controlScheme2.SetActive(false);
                    AnimateBar(6);
                    CloseBar(0);
                    CloseBar(1);
                    CloseBar(2);
                    CloseBar(3);
                    CloseBar(4);
                    CloseBar(5);
                    break;
            }
            if ((Input.GetAxis("MacroEngine") < -0.3) || (Input.GetAxis("Vertical") < -0.3))
            {
                if (m_selectedIndex == m_menuOptions.Length - 1)
                {
                    m_selectedIndex = 0;
                }
                else
                {
                    m_selectedIndex++;
                }
                m_readyForInput = false;
                m_UIAudio.clip = m_sounds[0];
                m_UIAudio.Play();
            }
            else if ((Input.GetAxis("MacroEngine") > 0.3) || (Input.GetAxis("Vertical") > 0.3))
            {
                if (m_selectedIndex == 0)
                {
                    m_selectedIndex = m_menuOptions.Length - 1;
                }
                else
                {
                    m_selectedIndex--;
                }
                m_readyForInput = false;
                m_UIAudio.clip = m_sounds[0];
                m_UIAudio.Play();
            }
            if (m_selectedIndex != m_menuOptions.Length && m_audioCooldown == false)
            {
                if ((Input.GetAxis("MacroEngine") < 0) || (Input.GetAxis("Horizontal") < -0.3))
                {
                    if (m_values[m_selectedIndex].text == "0")
                    {
                        m_values[m_selectedIndex].text = "0";
                    }
                    else
                    {
                        int _value = int.Parse(m_values[m_selectedIndex].text);
                        _value--;
                        m_values[m_selectedIndex].text = _value.ToString();
                        m_UIAudio.clip = m_sounds[1];
                        m_UIAudio.Play();
                        StartCoroutine("audioCooldown");
                    }
                }
                else if ((Input.GetAxis("MacroEngine") > 0) || (Input.GetAxis("Horizontal") > 0))
                {
                    if (m_values[m_selectedIndex].text == "80")
                    {
                        m_values[m_selectedIndex].text = "80";
                    }
                    else
                    {
                        int _value = int.Parse(m_values[m_selectedIndex].text);
                        _value++;
                        m_values[m_selectedIndex].text = _value.ToString();
                        m_UIAudio.clip = m_sounds[1];
                        m_UIAudio.Play();
                        StartCoroutine("audioCooldown");
                    }
                }
            }

            PlayerPrefs.SetInt("masterVolume", int.Parse(m_values[0].text));
            PlayerPrefs.SetInt("musicVolume", int.Parse(m_values[1].text));
            PlayerPrefs.SetInt("fxVolume", int.Parse(m_values[2].text));
            PlayerPrefs.SetInt("uiVolume", int.Parse(m_values[3].text));

            m_mixer.SetFloat("masterVol", -80 + PlayerPrefs.GetInt("masterVolume"));
            m_mixer.SetFloat("musicVol", -80 + PlayerPrefs.GetInt("musicVolume"));
            m_mixer.SetFloat("fxVol", -80 + PlayerPrefs.GetInt("fxVolume"));
            m_mixer.SetFloat("uiVol", -80 + PlayerPrefs.GetInt("uiVolume"));

            if (Input.GetButtonDown("XboxA"))
            {
                m_UIAudio.clip = m_sounds[1];
                m_UIAudio.Play();
                switch (m_selectedIndex)
                {
                    case 4:
                        if (m_controlSchemeValue.text == "1")
                        {
                            PlayerPrefs.SetInt("ControlScheme", 1);
                            m_controlSchemeValue.text = "2";
                        }
                        else
                        {
                            PlayerPrefs.SetInt("ControlScheme", 0);
                            m_controlSchemeValue.text = "1";
                        }
                        break;
                    case 5:
                        if (m_invertControlValue.text == "YES")
                        {
                            PlayerPrefs.SetInt("Invert", 1);
                            if (GameManager.Instance != null)
                            {
                                GameManager.Instance.PlayerMove.InvertY = true;
                            }
                            m_invertControlValue.text = "NO";
                        }
                        else
                        {
                            PlayerPrefs.SetInt("Invert", 0);
                            if (GameManager.Instance != null)
                            {
                                GameManager.Instance.PlayerMove.InvertY = false;
                            }
                            m_invertControlValue.text = "YES";
                        }
                        break;
                    case 6:
                        m_mainMenu.SetActive(true);
                        gameObject.SetActive(false);
                        break;
                }
            }
        }
        if (Input.GetButtonDown("XboxB"))
        {
            m_mainMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
    private void AnimateBar(int _item)
    {
        Image _bar = m_menuOptions[_item].GetComponent<Image>();
        _bar.fillAmount += 2f * Time.deltaTime;
    }
    private void CloseBar(int _item)
    {
        Image _bar = m_menuOptions[_item].GetComponent<Image>();
        _bar.fillAmount -= 3f * Time.deltaTime;
    }
    IEnumerator audioCooldown()
    {
        m_audioCooldown = true;
        yield return new WaitForSeconds(0.05f);
        m_audioCooldown = false;
    }
}
