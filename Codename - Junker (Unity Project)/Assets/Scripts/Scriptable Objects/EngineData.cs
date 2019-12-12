using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EngineData : ScriptableObject
{
    [SerializeField]
    private string engineName, description;

    private enum moduleType
    {
        weapon, engine
    }
    public enum faction
    {
        explorer, initial, trader, construction
    }

    private moduleType currentModuleType = moduleType.engine;

    [SerializeField]
    private faction currentFaction;

    [SerializeField]
    private Sprite thumbnail;

    [SerializeField]
    private float value, topSpeed, acceleration, boostPower, handling;

    [SerializeField]
    private int engineId;

    [SerializeField]
    private string seed;

    [ContextMenu("Generate Seed")]
    private void generateSeed()
    {
        string _maxSpeedString;
        string _accelerationString;
        string _boostPowerString;
        string _handlingString;
        string _valueString;

        #region Creating corecctly formated string
        if (topSpeed < 10)
        {
            _maxSpeedString = "00" + topSpeed;
        }
        else if (topSpeed < 100)
        {
            _maxSpeedString = "0" + topSpeed;
        }
        else
        {
            _maxSpeedString = topSpeed.ToString();
        }

        if (acceleration < 10)
        {
            _accelerationString = "00" + acceleration;
        }
        else if (acceleration < 100)
        {
            _accelerationString = "0" + acceleration;
        }
        else
        {
            _accelerationString = acceleration.ToString();
        }

        if (boostPower < 10)
        {
            _boostPowerString = "00" + boostPower;
        }
        else if (boostPower < 100)
        {
            _boostPowerString = "0" + boostPower;
        }
        else
        {
            _boostPowerString = boostPower.ToString();
        }

        if (handling < 10)
        {
            _handlingString = "00" + handling;
        }
        else if (handling < 100)
        {
            _handlingString = "0" + handling;
        }
        else
        {
            _handlingString = handling.ToString();
        }

        if (value < 10)
        {
            _valueString = "00" + value;
        }
        else if (value < 100)
        {
            _valueString = "0" + value;
        }
        else
        {
            _valueString = value.ToString();
        }
        #endregion

        seed = (int)currentFaction + _maxSpeedString + _accelerationString + _boostPowerString + _handlingString + _valueString + engineId;
    }


    public string Name { get => engineName; set => engineName = value; }
    public string Description { get => description; set => description = value; }
    public Sprite Thumbnail { get => thumbnail; set => thumbnail = value; }
    public float Value { get => value; set => this.value = value; }
    public float TopSpeed { get => topSpeed; set => topSpeed = value; }
    public float Acceleration { get => acceleration; set => acceleration = value; }
    public float Handling { get => handling; set => handling = value; }
    private moduleType CurrentModuleType { get => currentModuleType; }
    public int EngineId { get => engineId; set => engineId = value; }
    public faction CurrentFaction { get => currentFaction; set => currentFaction = value; }
    public string Seed { get => seed; set => seed = value; }
    public float BoostPower { get => boostPower; set => boostPower = value; }
}
