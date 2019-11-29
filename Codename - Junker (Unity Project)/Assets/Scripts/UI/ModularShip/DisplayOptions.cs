using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayOptions : MonoBehaviour
{
    public RectTransform contentPanel;
    public RectTransform highlight;
    public RectTransform statsPanel;
    public PopulateStatDisplay m_statsPanelUpdate;
    public SelectionManager selectionManager;

    public VerticalLayoutGroup vertLayout;

    public GameObject goTemplateEngineElement, goTemplateWeaponElement;

    public List<EngineData> availableEngines;
    public List<WeaponData> availableWeapons;

    //public Inventory playerInventory;

    private int m_numItemsOnScreen = 8;
    [HideInInspector] private int index;

    private List<GameObject> m_modulesList = new List<GameObject>();

    public List<GameObject> ModulesList { get => m_modulesList; set => m_modulesList = value; }
    public int NumItemsOnScreen { get => m_numItemsOnScreen; set => m_numItemsOnScreen = value; }
    public int Index { get => index; set => index = value; }

    private bool m_finishedLoad = false;
    
    public void FillInventory()
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
            _goTempWeaponElement.GetComponent<RectTransform>().sizeDelta = new Vector2(_tempRect.rect.width * 0.6f, Screen.height / m_numItemsOnScreen);

            m_modulesList.Add(_goTempWeaponElement);
        }

        RectTransform _targetBlock = m_modulesList[0].GetComponent<RectTransform>();
        statsPanel.sizeDelta = new Vector2(_targetBlock.sizeDelta.x /3,_targetBlock.sizeDelta.y * 1.7f);

        index = 0;
        UpdateHighlightPosition();
        m_statsPanelUpdate = statsPanel.GetComponent<PopulateStatDisplay>();

        m_finishedLoad = true;

        Invoke("WaitForLoad", 0.05f);
    }

    private void WaitForLoad()
    {
        selectionManager.FillMenu();
    }

    private void Update()
    {
        if (m_finishedLoad)
        {
            RectTransform _targetBlock = m_modulesList[index].GetComponent<RectTransform>();
            Vector2 _statsDisplayPosition = statsPanel.position;
            Vector2 _statsDisplayRequiredPosition;

            if (index != 0)
            {
                _statsDisplayRequiredPosition = new Vector2(_targetBlock.position.x + _targetBlock.sizeDelta.x * 0.9f, _targetBlock.position.y);
                _statsDisplayPosition.y = Mathf.Clamp(_statsDisplayPosition.y, statsPanel.sizeDelta.y, Screen.height - statsPanel.sizeDelta.y);
                if (_statsDisplayPosition != _statsDisplayRequiredPosition)
                {
                    statsPanel.position = Vector2.Lerp(_statsDisplayPosition, _statsDisplayRequiredPosition, 0.1f);
                }
            }
            else
            {
                _statsDisplayRequiredPosition = new Vector2(_targetBlock.position.x + _targetBlock.sizeDelta.x * 0.9f, +_targetBlock.position.y - _targetBlock.sizeDelta.y * 0.4f);
                if (_statsDisplayPosition != _statsDisplayRequiredPosition)
                {
                    statsPanel.position = Vector2.Lerp(_statsDisplayPosition, _statsDisplayRequiredPosition, 0.1f);
                }
            }
        }
    }

    public void UpdateHighlightPosition()
    {
        m_modulesList[index].GetComponent<ToggleElements>().HighlightOn();
        for (int i = 0; i < m_modulesList.Count; i++)
        {
            if(i != index)
            {
                ModulesList[i].GetComponent<ToggleElements>().HighlightOff();
            }
        }

        //Displays stats correctly, if shields don't work then we need to add them to here.
        if(m_modulesList[index].name.Contains("weapon"))
        {
            m_statsPanelUpdate.PopulateWeapon(m_modulesList[index].GetComponent<WeaponStatManager>().Data);
        }
        else if (m_modulesList[index].name.Contains("engine"))
        {
            m_statsPanelUpdate.PopulateEngine(m_modulesList[index].GetComponent<EngineStatManager>().Data);
        }



        Vector2 _statDisplayPosition = statsPanel.position;
        Vector2 _statsDisplayRequiredPosition = new Vector2(_statDisplayPosition.x,m_modulesList[index].GetComponent<RectTransform>().position.y);
        statsPanel.position = Vector2.Lerp(_statDisplayPosition, _statsDisplayRequiredPosition, 0.5f);
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
        //contentPanel.offsetMin = new Vector2(contentPanel.offsetMin.x, index * 100); //bottom    
        //contentPanel.offsetMax = new Vector2(contentPanel.offsetMax.x, (index * -1) * 100);    //top

        RectOffset _tempPadding = new RectOffset(vertLayout.padding.left, vertLayout.padding.right, (_diffBetweenTopAndBottom - NumItemsOnScreen) * -(Screen.height / NumItemsOnScreen), vertLayout.padding.bottom);
        vertLayout.padding = _tempPadding;

        
    }

    public void ScrollDownToSelected()
    {
        //contentPanel.offsetMin = new Vector2(contentPanel.offsetMin.x, index * 100); //bottom    
        //contentPanel.offsetMax = new Vector2(contentPanel.offsetMax.x, (index * -1) * 100);    //top

        RectOffset _tempPadding = new RectOffset(vertLayout.padding.left, vertLayout.padding.right, (index - NumItemsOnScreen) * -(Screen.height / NumItemsOnScreen), vertLayout.padding.bottom);
        vertLayout.padding = _tempPadding;


    }
}



