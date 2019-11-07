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

    private string seed;

    public string Name { get => engineName; set => engineName = value; }
    public string Description { get => description; set => description = value; }
    public Sprite Thumbnail { get => thumbnail; set => thumbnail = value; }
    public float Value { get => value; set => this.value = value; }
    public float TopSpeed { get => topSpeed; set => topSpeed = value; }
    public float Acceleration { get => acceleration; set => acceleration = value; }
    public float Acceleration1 { get => acceleration; set => acceleration = value; }
    public float Handling { get => handling; set => handling = value; }
    private moduleType CurrentModuleType { get => currentModuleType; }
    public int EngineId { get => engineId; set => engineId = value; }
    public faction CurrentFaction { get => currentFaction; set => currentFaction = value; }
    public string Seed { get => seed; set => seed = value; }
}
