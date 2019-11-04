using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Inventory : ScriptableObject
{
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
}
