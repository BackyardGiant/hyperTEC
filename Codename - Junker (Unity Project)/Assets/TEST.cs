using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour
{
    public WeaponData weapon;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            ModuleManager.Instance.GenerateWeapon(weapon);
        }
    }
}
