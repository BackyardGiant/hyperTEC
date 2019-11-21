using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : MonoBehaviour
{
    private static PlayerInventoryManager s_instance;
    public static PlayerInventoryManager Instance { get => s_instance; set => s_instance = value; }

    [SerializeField]
    private List<WeaponData> m_availableWeapons;

    [SerializeField]
    private List<EngineData> m_availableEngines;

    [SerializeField]
    private EngineData m_equippedEngine;

    [SerializeField]
    private WeaponData m_equippedLeftWeapon, m_equippedRightWeapon;

    [SerializeField]
    private int m_equippedEngineIndex, m_equippedLeftIndex, m_equippedRightIndex;

    public List<WeaponData> AvailableWeapons { get => m_availableWeapons; set => m_availableWeapons = value; }
    public List<EngineData> AvailableEngines { get => m_availableEngines; set => m_availableEngines = value; }
    public EngineData EquippedEngine { get => m_equippedEngine; set => m_equippedEngine = value; }
    public WeaponData EquippedLeftWeapon { get => m_equippedLeftWeapon; set => m_equippedLeftWeapon = value; }
    public WeaponData EquippedRightWeapon { get => m_equippedRightWeapon; set => m_equippedRightWeapon = value; }
    public int EquippedEngineIndex { get => m_equippedEngineIndex; set => m_equippedEngineIndex = value; }
    public int EquippedLeftIndex { get => m_equippedLeftIndex; set => m_equippedLeftIndex = value; }
    public int EquippedRightIndex { get => m_equippedRightIndex; set => m_equippedRightIndex = value; }

    private void Awake()
    {

        if (s_instance == null)
        {
            s_instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void WipeInventory()
    {
        WeaponData DefaultLeft = m_availableWeapons[0];
        WeaponData DefaultRight = m_availableWeapons[1];

        m_availableEngines = new List<EngineData>();
        m_availableWeapons = new List<WeaponData>();
        m_availableWeapons.Add(DefaultLeft);
        m_availableWeapons.Add(DefaultRight);

        m_equippedEngineIndex = new int();
        m_equippedLeftIndex = new int();
        m_equippedRightIndex = new int();
    }
}
