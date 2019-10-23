using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponData : ScriptableObject
{
    [SerializeField]
    private string name, description;

    private enum moduleType
    {
        weapon, engine
    }

    private moduleType currentModuleType = moduleType.engine;

    [SerializeField]
    private Sprite thumbnail;

    [SerializeField]
    private float value, damage, reloadTime, fireRate, accuracy;

    [SerializeField]
    private GameObject weaponModel;

    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public Sprite Thumbnail { get => thumbnail; set => thumbnail = value; }
    public float Value { get => value; set => this.value = value; }
    public float Damage { get => damage; set => damage = value; }
    public float ReloadTime { get => reloadTime; set => reloadTime = value; }
    public float FireRate { get => fireRate; set => fireRate = value; }
    public float Accuracy { get => accuracy; set => accuracy = value; }
    private moduleType CurrentModuleType { get => currentModuleType; }
    public GameObject WeaponModel { get => weaponModel; set => weaponModel = value; }
}
