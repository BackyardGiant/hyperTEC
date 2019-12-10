using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    private int m_engineLength;

    private float m_timeDestroyHeld;
    [SerializeField]
    private Image m_destroyFill;

    // Initialise all to "null" (-1)
    public void FillMenu()
    {
        if(PlayerInventoryManager.Instance.EquippedEngineIndex != -1)
        {
            m_equippedEngineIndex = PlayerInventoryManager.Instance.EquippedEngineIndex;
            m_takenIndexes[0] = PlayerInventoryManager.Instance.EquippedEngineIndex;
            //display.UpdateEquippedIndividual(m_takenIndexes[0], "engine");
        }
        else
        {
            m_equippedEngineIndex = -1;
            m_takenIndexes[0] = -1;
            //display.UpdateEquippedIndividual(m_takenIndexes[0], "engine");
        }
            
        if(PlayerInventoryManager.Instance.EquippedLeftIndex != -1)
        {
            m_equippedLeftIndex = PlayerInventoryManager.Instance.EquippedLeftIndex + PlayerInventoryManager.Instance.AvailableEngines.Count;
            m_takenIndexes[1] = PlayerInventoryManager.Instance.EquippedLeftIndex + PlayerInventoryManager.Instance.AvailableEngines.Count;
            //display.UpdateEquippedIndividual(m_takenIndexes[1], "left");
        }
        else
        {
            m_equippedLeftIndex = -1;
            m_takenIndexes[1] = -1;
            //display.UpdateEquippedIndividual(m_takenIndexes[1], "left");
        }
                 
        if(PlayerInventoryManager.Instance.EquippedRightIndex != -1)
        {
            m_equippedRightIndex = PlayerInventoryManager.Instance.EquippedRightIndex + PlayerInventoryManager.Instance.AvailableEngines.Count;
            m_takenIndexes[2] = PlayerInventoryManager.Instance.EquippedRightIndex + PlayerInventoryManager.Instance.AvailableEngines.Count;
            //display.UpdateEquippedIndividual(m_takenIndexes[2], "right");
        }
        else
        {
            m_equippedRightIndex = -1;
            m_takenIndexes[2] = -1;
            //display.UpdateEquippedIndividual(m_takenIndexes[2], "right");
        }
            
        display.UpdateHighlightPosition();
        display.UpdateEquipped(m_takenIndexes);
        DisplayEquipped();

        PreviewSelected(display.ModulesList[m_currentlySelectedIndex]);

        m_bottomIndex = display.NumItemsOnScreen;

        m_filled = true;
    }

    private void ScrollUpInput()
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
            if (Input.GetButtonDown("XboxX"))
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

            if(Input.GetButton("XboxY"))
            {
                m_destroyFill.fillAmount += Time.deltaTime;

                if(m_destroyFill.fillAmount == 1f)
                {
                    m_destroyFill.fillAmount = 0f;
                    //Destroy item
                    bool _equipped = false;

                    for (int i = 0; i < m_takenIndexes.Length; i++)
                    {
                        if(display.ModulesList.Count == 1 || m_currentlySelectedIndex == m_takenIndexes[i])
                        {
                            _equipped = true;
                            break;
                        }
                    }

                    if(!_equipped)
                    {
                        if(display.ModulesList[m_currentlySelectedIndex].TryGetComponent<EngineStatManager>(out EngineStatManager _engineComponent))
                        {
                            PlayerInventoryManager.Instance.RemoveEngine(m_currentlySelectedIndex);
                        }
                        else if (display.ModulesList[m_currentlySelectedIndex].TryGetComponent<WeaponStatManager>(out WeaponStatManager _weaponComponent))
                        {
                            PlayerInventoryManager.Instance.RemoveWeapon(m_currentlySelectedIndex);
                        }

                        Destroy(display.ModulesList[m_currentlySelectedIndex]);
                        display.ModulesList.RemoveAt(m_currentlySelectedIndex);

                        for (int i = 0; i < m_takenIndexes.Length; i++)
                        {
                            if(m_currentlySelectedIndex < m_takenIndexes[i])
                            {
                                m_takenIndexes[i]--;
                            }
                        }

                        PlayerInventoryManager.Instance.FixEquippedIndex(m_takenIndexes);

                        m_equippedEngineIndex = m_takenIndexes[0];
                        m_equippedLeftIndex = m_takenIndexes[1];
                        m_equippedRightIndex = m_takenIndexes[2];


                        //if (m_currentlySelectedIndex == display.ModulesList.Count)
                        //{
                        //    m_currentlySelectedIndex--;
                        //}

                        //RemovePreviousModule();
                        //DisplayEquipped();
                        //PreviewSelected(display.ModulesList[m_currentlySelectedIndex]);

                        ScrollUpInput();

                        FillMenu();
                        display.UpdateEquipped(m_takenIndexes);
                    }
                }
            }
            else
            {
                m_destroyFill.fillAmount = 0f;
            }

            // Equip an option from the inventory
            if (Input.GetButtonDown("Throttle Up"))
            {
                RemoveEngine();
                RemoveLeft();
                RemoveRight();

                GameObject selected = display.ModulesList[m_currentlySelectedIndex];

                if (selected.GetComponent<EngineStatManager>())
                {
                    if (m_equippedEngineIndex != m_currentlySelectedIndex)
                    {

                        m_equippedEngineIndex = m_currentlySelectedIndex;
                        m_takenIndexes[0] = (int)m_equippedEngineIndex;
                        Debug.Log("Equipped engine " + m_equippedEngineIndex);

                        //display.UpdateEquippedIndividual(m_takenIndexes[0], "engine");

                        // Set equipped in player inventory
                        PlayerInventoryManager.Instance.EquippedEngine = PlayerInventoryManager.Instance.AvailableEngines[(int)m_equippedEngineIndex];
                        PlayerInventoryManager.Instance.EquippedEngineIndex = (int)m_equippedEngineIndex;
                        StartCoroutine(SendEngineData(PlayerInventoryManager.Instance.EquippedEngine));
                    }
                    else
                    {
                        m_equippedEngineIndex = -1;
                        m_takenIndexes[0] = -1;

                        //display.UpdateEquippedIndividual(m_takenIndexes[0], "engine");

                        // Set removed in player inventory
                        PlayerInventoryManager.Instance.EquippedEngine = null;
                        PlayerInventoryManager.Instance.EquippedEngineIndex = -1;
                    }
                    try { display.m_statsPanelUpdate.PopulateEngine(PlayerInventoryManager.Instance.EquippedEngine); } catch { display.m_statsPanelUpdate.ClearEngine(); };
                }

                if (selected.GetComponent<WeaponStatManager>())
                {
                    if (display.ModulesList[m_currentlySelectedIndex].GetComponent<ToggleElements>().IsEquipped())
                    {
                        if (m_equippedRightIndex == m_currentlySelectedIndex)
                        {
                            m_equippedRightIndex = -1;
                            m_takenIndexes[2] = -1;

                            //display.UpdateEquippedIndividual(m_takenIndexes[2], "right");

                            // Set removed in player inventory
                            PlayerInventoryManager.Instance.EquippedRightWeapon = null;
                            PlayerInventoryManager.Instance.EquippedRightIndex = -1;

                            RemoveRight();
                        }

                        if (m_equippedLeftIndex == m_currentlySelectedIndex)
                        {
                            m_equippedLeftIndex = -1;
                            m_takenIndexes[1] = -1;

                            //display.UpdateEquippedIndividual(m_takenIndexes[1], "left");

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

                            //display.UpdateEquippedIndividual(m_takenIndexes[1], "left");

                            m_engineLength = PlayerInventoryManager.Instance.AvailableEngines.Count;
                            // Set equipped in player inventory
                            PlayerInventoryManager.Instance.EquippedLeftWeapon = PlayerInventoryManager.Instance.AvailableWeapons[m_equippedLeftIndex - m_engineLength];
                            PlayerInventoryManager.Instance.EquippedLeftIndex = (int)m_equippedLeftIndex - PlayerInventoryManager.Instance.AvailableEngines.Count;
                            StartCoroutine(SendWeaponData(PlayerInventoryManager.Instance.EquippedLeftWeapon));
                        }

                        if (m_equippedRightIndex != m_currentlySelectedIndex && !m_leftSideSelected)
                        {
                            m_equippedRightIndex = m_currentlySelectedIndex;
                            m_takenIndexes[2] = m_equippedRightIndex;
                            Debug.Log("Equipped right gun " + m_equippedRightIndex);

                            //display.UpdateEquippedIndividual(m_takenIndexes[2], "right");

                            m_engineLength = PlayerInventoryManager.Instance.AvailableEngines.Count;

                            // Set equipped in player inventory
                            PlayerInventoryManager.Instance.EquippedRightWeapon = PlayerInventoryManager.Instance.AvailableWeapons[m_equippedRightIndex - m_engineLength];
                            PlayerInventoryManager.Instance.EquippedRightIndex = (int)m_equippedRightIndex - PlayerInventoryManager.Instance.AvailableEngines.Count;
                            StartCoroutine(SendWeaponData(PlayerInventoryManager.Instance.EquippedRightWeapon));
                        }
                    }
                    m_engineLength = PlayerInventoryManager.Instance.AvailableEngines.Count;
                    try { display.m_statsPanelUpdate.PopulateWeapon(PlayerInventoryManager.Instance.AvailableWeapons[m_currentlySelectedIndex - m_engineLength]); } catch { }
                }

                DisplayEquipped();

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


    IEnumerator SendWeaponData(WeaponData _statblock)
    {
        //Faction
        //Damage
        //FireRate
        //Accuracy
        //DPS

        //OI AIDEN PUT DPS HERE PLS

        float _damageMin = 5;
        float _damageMax = 30;

        float _fireRateMin = 0.1f;
        float _fireRateMax = 1.3f;

        float _accuracyMin = 0;
        float _accuracyMax = 1;

        float _damageValue, _shotsPerSecond, _accuracyValue;

        _damageValue = _damageMin + ((_damageMax - _damageMin) * (_statblock.Damage / 100f));
        _shotsPerSecond = 1/(_fireRateMin + ((_fireRateMax - _fireRateMin) * ((100 - _statblock.FireRate) / 100f)));
        _accuracyValue = _accuracyMin + ((_accuracyMax - _accuracyMin) * (_statblock.Accuracy / 100f));

        float DPS = _accuracyValue * _damageValue * _shotsPerSecond;

        string BASE_URL = "https://docs.google.com/forms/u/1/d/e/1FAIpQLSc73TkV2ctEU0mOeKFUf-l1y2ZlP_QX0qE2KkUZmPG03j9_5A/formResponse";
        WWWForm form = new WWWForm();


        form.AddField("entry.1977573003", _statblock.CurrentFaction.ToString());
        form.AddField("entry.1564794063", _statblock.Damage.ToString());
        form.AddField("entry.1413229124", _statblock.FireRate.ToString());
        form.AddField("entry.476948232", _statblock.Accuracy.ToString());
        form.AddField("entry.906396447", DPS.ToString());


        byte[] rawData = form.data;
        WWW www = new WWW(BASE_URL, rawData);
        yield return www;

    }

    IEnumerator SendEngineData(EngineData _statblock)
    {
        //Faction
        //Speed
        //Acceleration
        //Boost Power
        //Handling


        string BASE_URL = "https://docs.google.com/forms/u/1/d/e/1FAIpQLSfroDYTkAwKmMXGbmHctdYf0tuFEcYyeb15J5wRRtsq1GdWLw/formResponse";
        WWWForm form = new WWWForm();


        form.AddField("entry.1853573029", _statblock.CurrentFaction.ToString());
        form.AddField("entry.1516976851", _statblock.TopSpeed.ToString());
        form.AddField("entry.737210474", _statblock.Acceleration.ToString());
        form.AddField("entry.974680751", _statblock.BoostPower.ToString());
        form.AddField("entry.1903627046", _statblock.Handling.ToString());


        byte[] rawData = form.data;
        WWW www = new WWW(BASE_URL, rawData);
        yield return www;

    }
}
