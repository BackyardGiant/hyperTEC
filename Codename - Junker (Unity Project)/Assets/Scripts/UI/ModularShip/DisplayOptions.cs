using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayOptions : MonoBehaviour
{
    public RectTransform contentPanel;
    public RectTransform highlight;

    public VerticalLayoutGroup vertLayout;

    public GameObject goTemplateEngineElement, goTemplateWeaponElement;

    public List<EngineData> availableEngines;
    public List<WeaponData> availableWeapons;

    //public Inventory playerInventory;

    private int m_numItemsOnScreen = 8;

    private List<GameObject> m_modulesList = new List<GameObject>();

    public List<GameObject> ModulesList { get => m_modulesList; set => m_modulesList = value; }
    public int NumItemsOnScreen { get => m_numItemsOnScreen; set => m_numItemsOnScreen = value; }

    private void Awake()
    {
        availableEngines = PlayerInventoryManager.Instance.AvailableEngines;
        availableWeapons = PlayerInventoryManager.Instance.AvailableWeapons;

        for (int i = 0; i < availableEngines.Count; i++)
        {
            GameObject _goTempEngineElement = Instantiate(goTemplateEngineElement, contentPanel);
            _goTempEngineElement.name = "engine" + i.ToString();
            _goTempEngineElement.GetComponent<EngineStatManager>().Data = availableEngines[i];
            _goTempEngineElement.GetComponent<EngineStatManager>().PopulateData();

            RectTransform _tempRect = gameObject.GetComponent<RectTransform>();
            _goTempEngineElement.GetComponent<RectTransform>().sizeDelta = new Vector2(_tempRect.rect.width, Screen.height / m_numItemsOnScreen);

            m_modulesList.Add(_goTempEngineElement);
        }

        for (int i = 0; i < availableWeapons.Count; i++)
        {
            GameObject _goTempWeaponElement = Instantiate(goTemplateWeaponElement, contentPanel);
            _goTempWeaponElement.name = "weapon" + i.ToString();
            _goTempWeaponElement.GetComponent<WeaponStatManager>().Data = availableWeapons[i];
            _goTempWeaponElement.GetComponent<WeaponStatManager>().PopulateData();

            RectTransform _tempRect = gameObject.GetComponent<RectTransform>();
            _goTempWeaponElement.GetComponent<RectTransform>().sizeDelta = new Vector2(_tempRect.rect.width, Screen.height / m_numItemsOnScreen);

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

    public void ScrollUpToSelected(int _diffBetweenTopAndBottom)
    {
        //contentPanel.offsetMin = new Vector2(contentPanel.offsetMin.x, _index * 100); //bottom    
        //contentPanel.offsetMax = new Vector2(contentPanel.offsetMax.x, (_index * -1) * 100);    //top

        RectOffset _tempPadding = new RectOffset(vertLayout.padding.left, vertLayout.padding.right, (_diffBetweenTopAndBottom - NumItemsOnScreen) * -(Screen.height / NumItemsOnScreen), vertLayout.padding.bottom);
        vertLayout.padding = _tempPadding;

        
    }

    public void ScrollDownToSelected(int _index)
    {
        //contentPanel.offsetMin = new Vector2(contentPanel.offsetMin.x, _index * 100); //bottom    
        //contentPanel.offsetMax = new Vector2(contentPanel.offsetMax.x, (_index * -1) * 100);    //top

        RectOffset _tempPadding = new RectOffset(vertLayout.padding.left, vertLayout.padding.right, (_index - NumItemsOnScreen) * -(Screen.height / NumItemsOnScreen), vertLayout.padding.bottom);
        vertLayout.padding = _tempPadding;


    }
}



