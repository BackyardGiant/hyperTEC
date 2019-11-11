using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SettingsMenuManagement : MonoBehaviour
{
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


    private bool m_readyForInput;
    private int m_selectedIndex;
    private AudioSource m_UIAudio;
    private bool m_audioCooldown;

    // Start is called before the first frame update
    void Start()
    {
        m_audioCooldown = false;
        m_selectedIndex = 0;
        m_UIAudio = this.GetComponent<AudioSource>();

      
        if (PlayerPrefs.GetInt("FirstTime") == 0)
        {
            PlayerPrefs.SetInt("masterVolume", 80);
            PlayerPrefs.SetInt("musicVolume", 80);
            PlayerPrefs.SetInt("fxVolume", 80);
            PlayerPrefs.SetInt("uiVolume", 80);
            PlayerPrefs.SetInt("FirstTime", 1);
        }
        m_values[0].text = PlayerPrefs.GetInt("masterVolume").ToString();
        m_values[1].text = PlayerPrefs.GetInt("musicVolume").ToString();
        m_values[2].text = PlayerPrefs.GetInt("fxVolume").ToString();
        m_values[3].text = PlayerPrefs.GetInt("uiVolume").ToString();



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


        if (m_readyForInput)
        {
            //If you've added an item to the menu and it's not selecting, it's because it doesn't exist here.
            switch (m_selectedIndex)
            {
                case 0:
                    //Continue
                    AnimateBar(0);
                    CloseBar(1);
                    CloseBar(2);
                    CloseBar(3);
                    CloseBar(4);
                    break;
                case 1:
                    AnimateBar(1);
                    CloseBar(0);
                    CloseBar(2);
                    CloseBar(3);
                    CloseBar(4);
                    //Load Game
                    break;
                case 2:
                    AnimateBar(2);
                    CloseBar(0);
                    CloseBar(1);
                    CloseBar(3);
                    CloseBar(4);
                    //New Game
                    break;
                case 3:
                    AnimateBar(3);
                    CloseBar(0);
                    CloseBar(1);
                    CloseBar(2);
                    CloseBar(4);
                    //Settings
                    break;
                case 4:
                    AnimateBar(4);
                    CloseBar(0);
                    CloseBar(1);
                    CloseBar(2);
                    CloseBar(3);
                    //Quit
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


            if (m_selectedIndex != 4 && m_audioCooldown == false)
            {
                if ((Input.GetAxis("MacroEngine") < 0) || (Input.GetAxis("Horizontal") < -0.3))
                {
                    if (m_values[m_selectedIndex].text == "0")
                    {
                        m_values[m_selectedIndex].text = "0";
                        m_downArrows[m_selectedIndex].SetActive(false);
                    }
                    else
                    {
                        int _value = int.Parse(m_values[m_selectedIndex].text);
                        _value--;
                        m_values[m_selectedIndex].text = _value.ToString();
                        m_downArrows[m_selectedIndex].SetActive(true);
                        m_upArrows[m_selectedIndex].SetActive(true);
                    }
                    m_UIAudio.clip = m_sounds[1];
                    m_UIAudio.Play();
                    StartCoroutine("audioCooldown");
                }
                else if ((Input.GetAxis("MacroEngine") > 0) || (Input.GetAxis("Horizontal") > 0))
                {
                    if (m_values[m_selectedIndex].text == "100")
                    {
                        m_values[m_selectedIndex].text = "100";
                        m_upArrows[m_selectedIndex].SetActive(false);
                    }
                    else
                    {
                        int _value = int.Parse(m_values[m_selectedIndex].text);
                        _value++;
                        m_values[m_selectedIndex].text = _value.ToString();
                        m_upArrows[m_selectedIndex].SetActive(true);
                        m_downArrows[m_selectedIndex].SetActive(true);
                    }
                    m_UIAudio.clip = m_sounds[1];
                    m_UIAudio.Play();
                    StartCoroutine("audioCooldown");
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
                        Debug.Log("Controls");
                        break;
                }
            }
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
