using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySavingObject
{
    public string[] availableWeapons;
    public string[] availableEngines;



    public InventorySavingObject(List<string> _weapons, List<string> _engines)
    {
        availableWeapons = _weapons.ToArray();
        availableEngines = _engines.ToArray();
    }
}
