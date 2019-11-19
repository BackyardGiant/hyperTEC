using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySavingObject
{
    public string[] availableWeapons;
    public string[] availableEngines;

    public string equippedEngineIndex;
    public string equippedLeftIndex;
    public string equippedRightIndex;

    public InventorySavingObject(List<string> _weapons, List<string> _engines, string _equippedEngineIndex, string _equippedLeftIndex, string _equippedRightIndex)
    {
        availableWeapons = _weapons.ToArray();
        availableEngines = _engines.ToArray();

        equippedEngineIndex = _equippedEngineIndex;
        equippedLeftIndex = _equippedLeftIndex;
        equippedRightIndex = _equippedRightIndex;
    }
}
