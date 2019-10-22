using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStatManager : MonoBehaviour
{
    [SerializeField]
    private WeaponData data;
    public WeaponData Data { get => data; set => data = value; }
}
