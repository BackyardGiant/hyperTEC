using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectionManager : MonoBehaviour
{
    public DisplayOptions display;
    public GameObject goShipEngineSnap, goShipLeftSnap, goShipRightSnap;

    public TextMeshProUGUI currentPreviewtext;

    public Material previewMaterial;

    [SerializeField]
    private int m_currentlySelectedIndex = 0;
    private bool m_readyForInput = true;
    private int m_equippedEngineIndex, m_equippedLeftIndex, m_equippedRightIndex;
    private int[] m_takenIndexes = new int[] { -1, -1, -1 };
    private bool m_leftSideSelected = true;

    [SerializeField]
    private int m_bottomIndex;

    private bool m_filled = false;
    // Initialise all to "null" (-1)
    public void FillMenu()
    {
        m_equippedEngineIndex = PlayerInventoryManager.Instance.EquippedEngineIndex;
        m_equippedLeftIndex = PlayerInventoryManager.Instance.EquippedLeftIndex + PlayerInventoryManager.Instance.AvailableEngines.Count;
        m_equippedRightIndex = PlayerInventoryManager.Instance.EquippedRightIndex + PlayerInventoryManager.Instance.AvailableEngines.Count;

        m_takenIndexes[0] = PlayerInventoryManager.Instance.EquippedEngineIndex;
        m_takenIndexes[1] = PlayerInventoryManager.Instance.EquippedLeftIndex + PlayerInventoryManager.Instance.AvailableEngines.Count;
        m_takenIndexes[2] = PlayerInventoryManager.Instance.EquippedRightIndex + PlayerInventoryManager.Instance.AvailableEngines.Count;

        display.UpdateHighlightPosition();
        display.UpdateEquipped(m_takenIndexes);
        DisplayEquipped();

        PreviewSelected(display.ModulesList[m_currentlySelectedIndex]);

        m_bottomIndex = display.NumItemsOnScreen;

        m_filled = true;
    }

    private void Update()
    {
        if (m_filled == true)
        {
            // Reset input "cooldown"
            if ((Input.GetAxis("MacroEngine") == 0) && (Input.GetAxis("Vertical") == 0))
            {
                m_readyForInput = true;
            }

            if (m_readyForInput)
            {
                if ((Input.GetAxis("MacroEngine") < 0) || (Input.GetAxis("Vertical") < 0))
                {
                    if (m_currentlySelectedIndex >= display.ModulesList.Count - 1)
                    {
                        m_currentlySelectedIndex = display.ModulesList.Count - 1;
                    }
                    else
                    {
                        m_currentlySelectedIndex++;
                    }
                    Debug.Log("Index is " + m_currentlySelectedIndex);

                    if (m_currentlySelectedIndex >= display.NumItemsOnScreen)
                    {
                        if (m_bottomIndex != display.ModulesList.Count)
                        {
                            display.Index = m_currentlySelectedIndex + 1;
                            display.ScrollDownToSelected();
                            m_bottomIndex++;
                        }

                    }

                    RemovePreviousModule();
                    DisplayEquipped();
                    PreviewSelected(display.ModulesList[m_currentlySelectedIndex]);

                    display.Index = m_currentlySelectedIndex;
                    display.UpdateHighlightPosition();
                    m_readyForInput = false;
                }

                if ((Input.GetAxis("MacroEngine") > 0) || (Input.GetAxis("Vertical") > 0))
                {
                    if (m_currentlySelectedIndex == 0)
                    {
                        m_currentlySelectedIndex = 0;
                    }
                    else
                    {
                        m_currentlySelectedIndex--;
                    }

                    if (m_currentlySelectedIndex < m_bottomIndex - display.NumItemsOnScreen)
                    {
                        m_bottomIndex--;
                        display.Index = m_bottomIndex;
                        display.ScrollDownToSelected();
                    }

                    RemovePreviousModule();
                    DisplayEquipped();
                    PreviewSelected(display.ModulesList[m_currentlySelectedIndex]);
                    display.Index = m_currentlySelectedIndex;
                    display.UpdateHighlightPosition();
                    m_readyForInput = false;
                }
            }

            // Swap from left to right side of the ship
            if (Input.GetButtonDown("XboxY"))
            {
                m_leftSideSelected = !m_leftSideSelected;
                Debug.Log("Left side selected? : " + m_leftSideSelected);

                if (m_leftSideSelected)
                {
                    currentPreviewtext.text = "Currently Previewing: Left Side";
                }
                else
                {
                    currentPreviewtext.text = "Currently Previewing: Right Side";
                }

                if (!CheckIfAlreadyEquipped())
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

                if (selected.GetComponent<EngineStatManager>())
                {
                    if (m_equippedEngineIndex != m_currentlySelectedIndex)
                    {
                        m_equippedEngineIndex = m_currentlySelectedIndex;
                        m_takenIndexes[0] = (int)m_equippedEngineIndex;
                        Debug.Log("Equipped engine " + m_equippedEngineIndex);

                        // Set equipped in player inventory
                        PlayerInventoryManager.Instance.EquippedEngine = PlayerInventoryManager.Instance.AvailableEngines[(int)m_equippedEngineIndex];
                        PlayerInventoryManager.Instance.EquippedEngineIndex = (int)m_equippedEngineIndex;
                    }
                    else
                    {
                        m_equippedEngineIndex = -1;
                        m_takenIndexes[0] = -1;

                        // Set removed in player inventory
                        PlayerInventoryManager.Instance.EquippedEngine = null;
                        PlayerInventoryManager.Instance.EquippedEngineIndex = -1;
                    }
                }

                if (selected.GetComponent<WeaponStatManager>())
                {
                    if (display.ModulesList[m_currentlySelectedIndex].GetComponent<ToggleElements>().IsEquipped())
                    {
                        if (m_equippedRightIndex == m_currentlySelectedIndex)
                        {
                            m_equippedRightIndex = -1;
                            m_takenIndexes[2] = -1;

                            // Set removed in player inventory
                            PlayerInventoryManager.Instance.EquippedRightWeapon = null;
                            PlayerInventoryManager.Instance.EquippedRightIndex = -1;

                            RemoveRight();
                        }

                        if (m_equippedLeftIndex == m_currentlySelectedIndex)
                        {
                            m_equippedLeftIndex = -1;
                            m_takenIndexes[1] = -1;

                            // Set removed in player inventory
                            PlayerInventoryManager.Instance.EquippedLeftWeapon = null;
                            PlayerInventoryManager.Instance.EquippedLeftIndex = -1;

                            RemoveLeft();
                        }
                    }
                    else
                    {
                        if (m_equippedLeftIndex != m_currentlySelectedIndex && m_leftSideSelected)
                        {
                            m_equippedLeftIndex = m_currentlySelectedIndex;
                            m_takenIndexes[1] = m_equippedLeftIndex;
                            Debug.Log("Equipped left gun " + m_equippedLeftIndex);
                            int _engineLength = PlayerInventoryManager.Instance.AvailableEngines.Count;

                            // Set equipped in player inventory
                            PlayerInventoryManager.Instance.EquippedLeftWeapon = PlayerInventoryManager.Instance.AvailableWeapons[m_equippedLeftIndex - _engineLength];
                            PlayerInventoryManager.Instance.EquippedLeftIndex = (int)m_equippedLeftIndex - PlayerInventoryManager.Instance.AvailableEngines.Count;
                        }

                        if (m_equippedRightIndex != m_currentlySelectedIndex && !m_leftSideSelected)
                        {
                            m_equippedRightIndex = m_currentlySelectedIndex;
                            m_takenIndexes[2] = m_equippedRightIndex;
                            Debug.Log("Equipped right gun " + m_equippedRightIndex);
                            int _engineLength = PlayerInventoryManager.Instance.AvailableEngines.Count;

                            // Set equipped in player inventory
                            PlayerInventoryManager.Instance.EquippedRightWeapon = PlayerInventoryManager.Instance.AvailableWeapons[m_equippedRightIndex - _engineLength];
                            PlayerInventoryManager.Instance.EquippedRightIndex = (int)m_equippedRightIndex - PlayerInventoryManager.Instance.AvailableEngines.Count;
                        }
                    }



                }

                PreviewSelected(display.ModulesList[m_currentlySelectedIndex]);              
                display.UpdateEquipped(m_takenIndexes);
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
            GameObject tempEngine = ModuleManager.Instance.GenerateEngine(statBlock);

            tempEngine.transform.GetChild(0).GetComponent<ThrustEffectController>().enabled = false;

            tempEngine.transform.SetParent(goShipEngineSnap.transform);
            tempEngine.transform.position = goShipEngineSnap.transform.position;
            tempEngine.transform.rotation = goShipEngineSnap.transform.rotation;
        }
        else
        {
            RemoveEngine();
        }

        if(m_equippedLeftIndex != -1)
        {
            WeaponData statBlock = display.ModulesList[(int)m_equippedLeftIndex].GetComponent<WeaponStatManager>().Data;
            GameObject tempWeapon = ModuleManager.Instance.GenerateWeapon(statBlock);

            tempWeapon.transform.SetParent(goShipLeftSnap.transform);
            tempWeapon.transform.position = goShipLeftSnap.transform.position;
            tempWeapon.transform.rotation = goShipLeftSnap.transform.rotation;
            tempWeapon.transform.localScale = new Vector3(1, 1, -1);

            Debug.Log("Instantiated from equipped :" + statBlock.Name);
        }
        else
        {
            RemoveLeft();
        }

        if(m_equippedRightIndex != -1)
        {
            WeaponData statBlock = display.ModulesList[(int)m_equippedRightIndex].GetComponent<WeaponStatManager>().Data;
            GameObject tempWeapon = ModuleManager.Instance.GenerateWeapon(statBlock);


            tempWeapon.transform.SetParent(goShipRightSnap.transform);
            tempWeapon.transform.position = goShipRightSnap.transform.position;
            tempWeapon.transform.rotation = goShipRightSnap.transform.rotation;
            tempWeapon.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            RemoveRight();
        }
    }

    /// <summary>
    /// Set a component as a preview, before equip
    /// </summary>
    /// <param name="selectedObject"> The object which is being previewed </param>
    private void PreviewSelected(GameObject selectedObject)
    {
        if(!CheckIfAlreadyEquipped())
        {
            if (selectedObject.GetComponent<EngineStatManager>())
            {
                RemoveEngine();

                EngineData _statBlock = selectedObject.GetComponent<EngineStatManager>().Data;
                GameObject _tempEngine = ModuleManager.Instance.GenerateEngine(_statBlock);

                _tempEngine.transform.GetChild(0).GetComponent<MeshRenderer>().material = previewMaterial;

                _tempEngine.transform.GetChild(0).GetComponent<ThrustEffectController>().enabled = false;

                _tempEngine.transform.SetParent(goShipEngineSnap.transform);
                _tempEngine.transform.position = goShipEngineSnap.transform.position;
                _tempEngine.transform.rotation = goShipEngineSnap.transform.rotation;
            }
            else if (selectedObject.GetComponent<WeaponStatManager>())
            {
                if(m_leftSideSelected)
                {
                    RemoveLeft();
                }
                else
                {
                    RemoveRight();
                }

                WeaponData _statBlock = selectedObject.GetComponent<WeaponStatManager>().Data;
                GameObject _tempWeapon = ModuleManager.Instance.GenerateWeapon(_statBlock);

                Debug.Log("Instantiated gun" + _statBlock.Name);

                if (m_leftSideSelected)
                {
          

                    _tempWeapon.transform.GetChild(0).GetComponent<MeshRenderer>().material = previewMaterial;
                    _tempWeapon.transform.GetChild(0).Find("BarrelSnap").GetComponentInChildren<MeshRenderer>().material = previewMaterial;
                    _tempWeapon.transform.GetChild(0).Find("MagSnap").GetComponentInChildren<MeshRenderer>().material = previewMaterial;
                    _tempWeapon.transform.GetChild(0).Find("TargetSnap").GetComponentInChildren<MeshRenderer>().material = previewMaterial;
                    _tempWeapon.transform.GetChild(0).Find("BatterySnap").GetComponentInChildren<MeshRenderer>().material = previewMaterial;

                    _tempWeapon.transform.SetParent(goShipLeftSnap.transform);
                    _tempWeapon.transform.position = goShipLeftSnap.transform.position;
                    _tempWeapon.transform.rotation = goShipLeftSnap.transform.rotation;
                    _tempWeapon.transform.localScale = new Vector3(1, 1, -1);

                }
                else
                {
                 
                    _tempWeapon.transform.GetChild(0).GetComponent<MeshRenderer>().material = previewMaterial;
                    _tempWeapon.transform.GetChild(0).Find("BarrelSnap").GetComponentInChildren<MeshRenderer>().material = previewMaterial;
                    _tempWeapon.transform.GetChild(0).Find("MagSnap").GetComponentInChildren<MeshRenderer>().material = previewMaterial;
                    _tempWeapon.transform.GetChild(0).Find("TargetSnap").GetComponentInChildren<MeshRenderer>().material = previewMaterial;
                    _tempWeapon.transform.GetChild(0).Find("BatterySnap").GetComponentInChildren<MeshRenderer>().material = previewMaterial;

                    _tempWeapon.transform.SetParent(goShipRightSnap.transform);
                    _tempWeapon.transform.position = goShipRightSnap.transform.position;
                    _tempWeapon.transform.rotation = goShipRightSnap.transform.rotation;
                    _tempWeapon.transform.localScale = new Vector3(1, 1, 1);
                }

            }
        }     
    }

    /// <summary>
    /// Remove all current children from the ship body
    /// </summary>
    private void RemovePreviousModule()
    {
        RemoveEngine();
        RemoveLeft();
        RemoveRight();
    }

    #region specific removal
    private void RemoveEngine()
    {
        int _tempChildrenCount = goShipEngineSnap.transform.childCount;

        if (_tempChildrenCount != 0)
        {
            for (int i = 0; i < _tempChildrenCount; i++)
            {
                Destroy(goShipEngineSnap.transform.GetChild(i).gameObject);
            }
        }
    }

    private void RemoveLeft()
    {
        int _tempChildrenCount = goShipLeftSnap.transform.childCount;

        if (_tempChildrenCount != 0)
        {
            for (int i = 0; i < _tempChildrenCount; i++)
            {
                Destroy(goShipLeftSnap.transform.GetChild(i).gameObject);
                Debug.Log("removing left");
            }
        }
    }

    private void RemoveRight()
    {
        int _tempChildrenCount = goShipRightSnap.transform.childCount;

        if (_tempChildrenCount != 0)
        {
            for (int i = 0; i < _tempChildrenCount; i++)
            {
                Destroy(goShipRightSnap.transform.GetChild(i).gameObject);
            }
        }
    }
    #endregion
}
