using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public DisplayOptions display;
    public GameObject goShipEngineSnap, goShipLeftSnap, goShipRightSnap;

    private int m_currentlySelectedIndex = 0;
    private bool m_readyForInput = true;
    private float m_equippedEngineIndex, m_equippedLeftIndex, m_equippedRightIndex;
    private int[] m_takenIndexes = new int[] { -1, -1, -1 };
    private bool m_leftSideSelected = true;

    // Initialise all to "null" (-1)
    void Start()
    {
        m_equippedEngineIndex = -1;
        m_equippedLeftIndex = -1;
        m_equippedRightIndex = -1;
    }

    private void Update()
    {
        // Reset input "cooldown"
        if((Input.GetAxis("MacroEngine") == 0) && (Input.GetAxis("Vertical") == 0))
        {
            m_readyForInput = true;
        }

        if(m_readyForInput)
        {
            if ((Input.GetAxis("MacroEngine") < 0) || (Input.GetAxis("Vertical") < 0))
            {
                if (m_currentlySelectedIndex == display.ModulesList.Count - 1)
                {
                    m_currentlySelectedIndex = 0;
                }
                else
                {
                    m_currentlySelectedIndex++;
                }

                // Only allow a new item to be previewed if it is not already equipped
                if(!CheckIfAlreadyEquipped())
                {
                    RemovePreviousModule();
                    DisplayEquipped();
                    PreviewSelected(display.ModulesList[m_currentlySelectedIndex]);
                }

                display.UpdateHighlightPosition(m_currentlySelectedIndex);
                m_readyForInput = false;
            }

            if ((Input.GetAxis("MacroEngine") > 0) || (Input.GetAxis("Vertical") > 0))
            {
                if (m_currentlySelectedIndex == 0)
                {
                    m_currentlySelectedIndex = display.ModulesList.Count - 1;
                }
                else
                {
                    m_currentlySelectedIndex--;
                }

                if (!CheckIfAlreadyEquipped())
                {
                    RemovePreviousModule();
                    DisplayEquipped();
                    PreviewSelected(display.ModulesList[m_currentlySelectedIndex]);
                }

                display.UpdateHighlightPosition(m_currentlySelectedIndex);
                m_readyForInput = false;
            }
        }

        // Swap from left to right side of the ship
        if(Input.GetButtonDown("XboxY"))
        {
            m_leftSideSelected = !m_leftSideSelected;
            Debug.Log("Left side selected? : " + m_leftSideSelected);

            if(!CheckIfAlreadyEquipped())
            {
                RemovePreviousModule();
                DisplayEquipped();
                PreviewSelected(display.ModulesList[m_currentlySelectedIndex]);
            }          
        }

        // Equip an option from the inventory
        if (Input.GetButtonDown("Throttle Up"))
        {
            GameObject selected = display.ModulesList[m_currentlySelectedIndex];

            if (selected.GetComponent<EngineStatManager>() && m_equippedEngineIndex != m_currentlySelectedIndex)
            {
                m_equippedEngineIndex = m_currentlySelectedIndex;
                m_takenIndexes[0] = (int)m_equippedEngineIndex;
                Debug.Log("Equipped engine " + m_equippedEngineIndex);
            }
            else if (selected.GetComponent<WeaponStatManager>())
            {
                if (m_leftSideSelected && m_equippedLeftIndex != m_currentlySelectedIndex)
                {
                    m_equippedLeftIndex = m_currentlySelectedIndex;
                    m_takenIndexes[1] = (int)m_equippedLeftIndex;
                    Debug.Log("Equipped left gun " + m_equippedLeftIndex);
                }
                else if(!m_leftSideSelected && m_equippedRightIndex != m_currentlySelectedIndex)
                {
                    m_equippedRightIndex = m_currentlySelectedIndex;
                    m_takenIndexes[2] = (int)m_equippedRightIndex;
                    Debug.Log("Equipped right gun " + m_equippedRightIndex);
                }
            }           
        }
    }

    private bool CheckIfAlreadyEquipped()
    {
        bool _isAlreadyEquipped = false;

        foreach (int _index in m_takenIndexes)
        {
            if (_index == m_currentlySelectedIndex)
            {
                _isAlreadyEquipped = true;
            }
        }

        return _isAlreadyEquipped;
    }

    /// <summary>
    /// Default view, display currently selected items in the event that nothing is being previews
    /// </summary>
    private void DisplayEquipped()
    {
        if(m_equippedEngineIndex != -1)
        {
            EngineData statBlock = display.ModulesList[(int)m_equippedEngineIndex].GetComponent<EngineStatManager>().Data;
            GameObject tempEngine = Instantiate(statBlock.EngineModel);

            tempEngine.transform.SetParent(goShipEngineSnap.transform);
            tempEngine.transform.position = goShipEngineSnap.transform.position;
            tempEngine.transform.rotation = goShipEngineSnap.transform.rotation;
        }

        if(m_equippedLeftIndex != -1)
        {
            WeaponData statBlock = display.ModulesList[(int)m_equippedLeftIndex].GetComponent<WeaponStatManager>().Data;
            GameObject tempWeapon = Instantiate(statBlock.WeaponModel);

            tempWeapon.transform.SetParent(goShipLeftSnap.transform);
            tempWeapon.transform.position = goShipLeftSnap.transform.position;
            tempWeapon.transform.rotation = goShipLeftSnap.transform.rotation;
            tempWeapon.transform.localScale = new Vector3(0.5f, 0.5f, -0.5f);
        }

        if(m_equippedRightIndex != -1)
        {
            WeaponData statBlock = display.ModulesList[(int)m_equippedRightIndex].GetComponent<WeaponStatManager>().Data;
            GameObject tempWeapon = Instantiate(statBlock.WeaponModel);

            tempWeapon.transform.SetParent(goShipRightSnap.transform);
            tempWeapon.transform.position = goShipRightSnap.transform.position;
            tempWeapon.transform.rotation = goShipRightSnap.transform.rotation;
            tempWeapon.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

    /// <summary>
    /// Set a component as a preview, before equip
    /// </summary>
    /// <param name="selectedObject"> The object which is being previewed </param>
    private void PreviewSelected(GameObject selectedObject)
    {
        if (selectedObject.GetComponent<EngineStatManager>())
        {
            EngineData _statBlock = selectedObject.GetComponent<EngineStatManager>().Data;
            GameObject _tempEngine = Instantiate(_statBlock.EngineModel);

            _tempEngine.transform.SetParent(goShipEngineSnap.transform);
            _tempEngine.transform.position = goShipEngineSnap.transform.position;
            _tempEngine.transform.rotation = goShipEngineSnap.transform.rotation;
        }
        else if (selectedObject.GetComponent<WeaponStatManager>())
        {
            WeaponData _statBlock = selectedObject.GetComponent<WeaponStatManager>().Data;
            GameObject _tempWeapon = Instantiate(_statBlock.WeaponModel);

            if (m_leftSideSelected)
            {
                _tempWeapon.transform.SetParent(goShipLeftSnap.transform);
                _tempWeapon.transform.position = goShipLeftSnap.transform.position;
                _tempWeapon.transform.rotation = goShipLeftSnap.transform.rotation;
                _tempWeapon.transform.localScale = new Vector3(0.5f, 0.5f, -0.5f);
            }
            else
            {
                _tempWeapon.transform.SetParent(goShipRightSnap.transform);
                _tempWeapon.transform.position = goShipRightSnap.transform.position;
                _tempWeapon.transform.rotation = goShipRightSnap.transform.rotation;
                _tempWeapon.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }

        }
    }

    /// <summary>
    /// Remove all current children from the ship body
    /// </summary>
    private void RemovePreviousModule()
    {
        int _tempChildrenCount;

        _tempChildrenCount = goShipEngineSnap.transform.childCount;

        if (_tempChildrenCount != 0)
        {
            for (int i = 0; i < _tempChildrenCount; i++)
            {
                GameObject.Destroy(goShipEngineSnap.transform.GetChild(i).gameObject);
            }
        }

        _tempChildrenCount = goShipLeftSnap.transform.childCount;

        if (_tempChildrenCount != 0)
        {
            for (int i = 0; i < _tempChildrenCount; i++)
            {
                GameObject.Destroy(goShipLeftSnap.transform.GetChild(i).gameObject);
            }
        }

        _tempChildrenCount = goShipRightSnap.transform.childCount;

        if (_tempChildrenCount != 0)
        {
            for (int i = 0; i < _tempChildrenCount; i++)
            {
                GameObject.Destroy(goShipRightSnap.transform.GetChild(i).gameObject);
            }
        }
    }
}
