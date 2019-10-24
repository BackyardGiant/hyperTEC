using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayOptions : MonoBehaviour
{
    public RectTransform contentPanel;
    public RectTransform highlight;

    [SerializeField]
    private List<GameObject> m_modulesList;

    public List<GameObject> ModulesList { get => m_modulesList; set => m_modulesList = value; }

    private void Awake()
    {
        for (int i = 0; i < m_modulesList.Count; i++)
        {
            GameObject _goNewModule = GameObject.Instantiate(m_modulesList[i]);
            _goNewModule.transform.SetParent(contentPanel);
            m_modulesList[i] = _goNewModule;
        }
    }

    public void UpdateHighlightPosition(int index)
    {
        m_modulesList[index].GetComponent<ToggleElements>().HighlightOn();

        for (int i = 0; i < ModulesList.Count; i++)
        {
            if(i != index)
            {
                ModulesList[i].GetComponent<ToggleElements>().HighlightOff();
            }
        }
    }

    public void UpdateEquipped(int[] equippedIndexes)
    {
        for (int i = 0; i < m_modulesList.Count; i++)
        {
            for (int j = 0; j < equippedIndexes.Length; j++)
            {
                if(i == equippedIndexes[j])
                {
                    ModulesList[i].GetComponent<ToggleElements>().EquippedOn();
                    break;
                }
                else
                {
                    ModulesList[i].GetComponent<ToggleElements>().EquippedOff();
                }
            }
        }
    }


}
