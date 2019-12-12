using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PromptSystemManagement : MonoBehaviour
{
    private static PromptSystemManagement s_instance;
    public static PromptSystemManagement Instance { get => s_instance; set => s_instance = value; }

    public enum PromptType { QuestPickup, ItemPickup };


    [SerializeField]
    private GameObject m_promptObject;
    [SerializeField]
    private Animator m_promptAnimator;


    private bool m_promptActive;
    private int m_currentPromptCount;
    private PromptType m_currentType;

    void Start()
    {
        m_currentPromptCount = 0;

    }
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

    public void DisplayPrompt(PromptType _type)
    {
        m_currentPromptCount += 1;
        if (m_promptActive == false)
        {
                m_promptActive = true;
                m_currentType = _type;
                if (m_currentType == PromptType.QuestPickup)
                {
                    
                    m_promptObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "   Quest Collected";
                }
                else if(m_currentType == PromptType.ItemPickup)
                {
                    m_promptObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "   Item Collected";
                }

                m_promptObject.SetActive(true);
                Invoke("HidePrompt", 2.5f);
        }
        else if(m_promptActive == true)
        {
            if (m_currentType == PromptType.QuestPickup)
            {
                m_promptObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "   " + m_currentPromptCount + "x Quest Collected";
                CancelInvoke();
                m_promptAnimator.Play("ContinuePrompt");
                Invoke("HidePrompt", 2.5f);
            }
            else if (m_currentType == PromptType.ItemPickup)
            {
                m_promptObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "   " + m_currentPromptCount + "x Item Collected";
                CancelInvoke();
                m_promptAnimator.Play("ContinuePrompt");
                Invoke("HidePrompt", 2.5f);
            }
        }
    }

    private void HidePrompt()
    {
        m_promptObject.SetActive(false);
        m_promptActive = false;
        m_currentPromptCount = 0;
    }
}
