using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponData : ScriptableObject
{
    [SerializeField]
    private string weaponName, description;

    private enum moduleType
    {
        weapon, engine
    }
    public enum faction
    {
        explorer, initial, trader, construction
    }

    public enum fireRateType
    {
        slow, medium, fast
    }
    [SerializeField]
    private fireRateType currentFireRateType;

    private moduleType currentModuleType = moduleType.weapon;
    [SerializeField]
    private int fireRateIndex;

    [SerializeField]
    private faction currentFaction;

    [SerializeField]
    private Sprite thumbnail;

    [SerializeField]
    private float value, damage, reloadTime, fireRate, accuracy;

    [SerializeField]
    private int barrelId, magazineId, batteryId, targetId;

    [SerializeField]
    private string seed;

    [ContextMenu("Generate Seed")]
    private void generateSeed()
    {
        string accuracyString;
        string fireRateString;
        string reloadTimeString;
        string damageString;
        string valueString;
        string fireSoundIndexString;

        if (fireRate < 10)
        {
            fireRateString = "00" + fireRate;
        }
        else if (fireRate < 100)
        {
            fireRateString = "0" + fireRate;
        }
        else
        {
            fireRateString = fireRate.ToString();
        }

        if (accuracy < 10)
        {
            accuracyString = "00" + accuracy;
        }
        else if (accuracy < 100)
        {
            accuracyString = "0" + accuracy;
        }
        else
        {
            accuracyString = accuracy.ToString();
        }

        if (damage < 10)
        {
            damageString = "00" + damage;
        }
        else if (damage < 100)
        {
            damageString = "0" + damage;
        }
        else
        {
            damageString = damage.ToString();
        }

        if (reloadTime < 10)
        {
            reloadTimeString = "00" + reloadTime;
        }
        else if (reloadTime < 100)
        {
            reloadTimeString = "0" + reloadTime;
        }
        else
        {
            reloadTimeString = reloadTime.ToString();
        }

        if (value < 10)
        {
            valueString = "00" + value;
        }
        else if (value < 100)
        {
            valueString = "0" + value;
        }
        else
        {
            valueString = value.ToString();
        }

        if (fireRateIndex < 10)
        {
            fireSoundIndexString = "00" + fireRateIndex;
        }
        else if (fireRateIndex < 100)
        {
            fireSoundIndexString = "0" + fireRateIndex;
        }
        else
        {
            fireSoundIndexString = fireRateIndex.ToString();
        }

        seed = ((int)currentFaction).ToString() + fireRateString + accuracyString + damageString + reloadTimeString + valueString + barrelId.ToString() + batteryId.ToString() + magazineId.ToString() + targetId.ToString() + fireSoundIndexString + ((int)currentFireRateType).ToString();
    }

    public string Name { get => weaponName; set => weaponName = value; }
    public string Description { get => description; set => description = value; }
    public Sprite Thumbnail { get => thumbnail; set => thumbnail = value; }
    public float Value { get => value; set => this.value = value; }
    public float Damage { get => damage; set => damage = value; }
    public float ReloadTime { get => reloadTime; set => reloadTime = value; }
    public float FireRate { get => fireRate; set => fireRate = value; }
    public float Accuracy { get => accuracy; set => accuracy = value; }
    private moduleType CurrentModuleType { get => currentModuleType; }
    public faction CurrentFaction { get => currentFaction; set => currentFaction = value; }
    public int BarrelId { get => barrelId; set => barrelId = value; }
    public int MagazineId { get => magazineId; set => magazineId = value; }
    public int BatteryId { get => batteryId; set => batteryId = value; }
    public int TargetId { get => targetId; set => targetId = value; }
    public string Seed { get => seed; set => seed = value; }
    public fireRateType CurrentFireRateType { get => currentFireRateType; set => currentFireRateType = value; }
    public int FireRateIndex { get => fireRateIndex; set => fireRateIndex = value; }
}
