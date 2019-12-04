using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarFormatting : MonoBehaviour
{
    [SerializeField]private Image m_secondHealthBar;
    [SerializeField] private Color m_idleColour;
    [SerializeField] private Color m_normalColour;
    [SerializeField] private Color m_criticalColour;


    // Update is called once per frame
    void Update()
    {
        float currentHealthPercentage = this.GetComponent<Image>().fillAmount;
        m_secondHealthBar.fillAmount = currentHealthPercentage;
        if (currentHealthPercentage == 1.0f)
        {
            transform.GetComponent<Image>().color = Color.Lerp(transform.GetComponent<Image>().color, m_idleColour, 0.1f);
            m_secondHealthBar.color = Color.Lerp(transform.GetComponent<Image>().color, m_idleColour, 0.1f);
        }
        if (currentHealthPercentage < 1.0f && currentHealthPercentage > 0.3f)
        {
            transform.GetComponent<Image>().color = Color.Lerp(transform.GetComponent<Image>().color,m_normalColour,0.1f);
            m_secondHealthBar.color = Color.Lerp(transform.GetComponent<Image>().color, m_normalColour, 0.1f);
        }
        if (currentHealthPercentage < 0.3f)
        {
            transform.GetComponent<Image>().color = Color.Lerp(transform.GetComponent<Image>().color, m_criticalColour, 0.1f);
            m_secondHealthBar.color = Color.Lerp(transform.GetComponent<Image>().color, m_criticalColour, 0.1f);
        }

    }
}
