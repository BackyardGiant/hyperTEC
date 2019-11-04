using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayOptions : MonoBehaviour
{
    public RectTransform contentPanel;
    public RectTransform highlight;

    public GameObject goTemplateEngineElement, goTemplateWeaponElement;

    public List<EngineData> availableEngines;
    public List<WeaponData> availableWeapons;

    public Inventory playerInventory;

    private List<GameObject> m_modulesList = new List<GameObject>();

    public List<GameObject> ModulesList { get => m_modulesList; set => m_modulesList = value; }

    private void Awake()
    {
        availableEngines = playerInventory.AvailableEngines;
        availableWeapons = playerInventory.AvailableWeapons;

        for (int i = 0; i < availableEngines.Count; i++)
        {
            GameObject _goTempEngineElement = Instantiate(goTemplateEngineElement, contentPanel);
            _goTempEngineElement.name = "engine" + i.ToString();
            _goTempEngineElement.GetComponent<EngineStatManager>().Data = availableEngines[i];
            _goTempEngineElement.GetComponent<EngineStatManager>().PopulateData();
            m_modulesList.Add(_goTempEngineElement);
        }

        for (int i = 0; i < availableWeapons.Count; i++)
        {
            GameObject _goTempWeaponElement = Instantiate(goTemplateWeaponElement, contentPanel);
            _goTempWeaponElement.name = "weapon" + i.ToString();
            _goTempWeaponElement.GetComponent<WeaponStatManager>().Data = availableWeapons[i];
            _goTempWeaponElement.GetComponent<WeaponStatManager>().PopulateData();
            m_modulesList.Add(_goTempWeaponElement);
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
