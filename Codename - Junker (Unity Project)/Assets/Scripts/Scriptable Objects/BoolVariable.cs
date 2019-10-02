using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoolVariable : ScriptableObject {

    [SerializeField]
    private bool value;

    public bool Value
    {
        get
        {
            return value;
        }
        set
        {
            this.value = value;
        }
    }
}
