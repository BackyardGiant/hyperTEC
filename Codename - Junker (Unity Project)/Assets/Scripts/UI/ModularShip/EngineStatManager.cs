using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineStatManager : MonoBehaviour
{
    [SerializeField]
    private EngineData data;
    public EngineData Data { get => data; set => data = value; }
}
