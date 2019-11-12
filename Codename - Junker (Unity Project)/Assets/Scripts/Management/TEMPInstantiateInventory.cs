using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMPInstantiateInventory : MonoBehaviour
{
    [SerializeField]
    private GameObject InventoryManager;
    // Start is called before the first frame update
    void Awake()
    {
        if (GameObject.FindGameObjectWithTag("InventoryManager") == null)
        {
            Instantiate(InventoryManager);
        }
    }
}
