using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public RectTransform highlight;

    private GameObject[] currentObjects;

    private int currentlySelectedIndex = 0;

    private bool readyForInput = true;

    public GameObject shipEngineSnap, shipLeftSnap, shipRightSnap;
    private GameObject previousEngine, previousLeft, previousRight;
    private GameObject currentEngine, currentLeft, currentRight;

    private bool leftSideSelected = true;

    // Start is called before the first frame update
    void Start()
    {
        currentObjects = GameObject.FindGameObjectsWithTag("Module");
    }

    private void Update()
    {
        if((Input.GetAxis("MacroEngine") == 0) && (Input.GetAxis("Vertical") == 0))
        {
            readyForInput = true;
        }

        if(readyForInput)
        {
            if ((Input.GetAxis("MacroEngine") < 0) || (Input.GetAxis("Vertical") < 0))
            {
                RemovePreviousModule();

                if (currentlySelectedIndex == currentObjects.Length - 1)
                {
                    currentlySelectedIndex = 0;
                }
                else
                {
                    currentlySelectedIndex++;
                }
                UpdateHighlight();
                readyForInput = false;
            }

            if ((Input.GetAxis("MacroEngine") > 0) || (Input.GetAxis("Vertical") > 0))
            {
                RemovePreviousModule();

                if (currentlySelectedIndex == 0)
                {
                    currentlySelectedIndex = currentObjects.Length - 1;
                }
                else
                {
                    currentlySelectedIndex--;
                }
                UpdateHighlight();
                readyForInput = false;
            }
        }

        if(Input.GetButtonDown("XboxY"))
        {
            leftSideSelected = !leftSideSelected;
            Debug.Log("Left side selected? : " + leftSideSelected);

            RemovePreviousModule();
            UpdateHighlight();
        }

        if (Input.GetButtonDown("Throttle Up"))
        {
            previousEngine = currentEngine;
            previousLeft = currentLeft;
            previousRight = currentRight;
        }
    }

    private void UpdateHighlight()
    {
        highlight.position = currentObjects[currentlySelectedIndex].GetComponent<RectTransform>().position;
        SelectModule(currentObjects[currentlySelectedIndex]);
    }

    private void SelectModule(GameObject selectedObject)
    {
        if(selectedObject.GetComponent<EngineStatManager>())
        {
            EngineData statBlock = selectedObject.GetComponent<EngineStatManager>().Data;
            GameObject tempEngine = Instantiate(statBlock.EngineModel);

            tempEngine.transform.SetParent(shipEngineSnap.transform);
            tempEngine.transform.position = shipEngineSnap.transform.position;
            tempEngine.transform.rotation = shipEngineSnap.transform.rotation;

            currentEngine = tempEngine;
        }
        else if(selectedObject.GetComponent<WeaponStatManager>())
        {
            WeaponData statBlock = selectedObject.GetComponent<WeaponStatManager>().Data;
            GameObject tempWeapon = Instantiate(statBlock.WeaponModel);

            if (leftSideSelected)
            {
                tempWeapon.transform.SetParent(shipLeftSnap.transform);
                tempWeapon.transform.position = shipLeftSnap.transform.position;
                tempWeapon.transform.rotation = shipLeftSnap.transform.rotation;
                tempWeapon.transform.localScale = new Vector3(0.5f, 0.5f, -0.5f);

                currentLeft = tempWeapon;
            }
            else
            {
                tempWeapon.transform.SetParent(shipRightSnap.transform);
                tempWeapon.transform.position = shipRightSnap.transform.position;
                tempWeapon.transform.rotation = shipRightSnap.transform.rotation;
                tempWeapon.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                currentRight = tempWeapon;
            }

        }

        
    }

    private void RemovePreviousModule()
    {
        int tempChildrenCount;

        tempChildrenCount = shipEngineSnap.transform.childCount;

        if (tempChildrenCount != 0)
        {
            for (int i = 0; i < tempChildrenCount; i++)
            {
                GameObject.Destroy(shipEngineSnap.transform.GetChild(i).gameObject);
            }
        }

        if(previousEngine != null)
        {
            SelectModule(previousEngine);
        }

        tempChildrenCount = shipLeftSnap.transform.childCount;

        if (tempChildrenCount != 0)
        {
            for (int i = 0; i < tempChildrenCount; i++)
            {
                GameObject.Destroy(shipLeftSnap.transform.GetChild(i).gameObject);
            }
        }

        if (previousLeft != null)
        {
            SelectModule(previousLeft);
        }

        tempChildrenCount = shipRightSnap.transform.childCount;

        if (tempChildrenCount != 0)
        {
            for (int i = 0; i < tempChildrenCount; i++)
            {
                GameObject.Destroy(shipRightSnap.transform.GetChild(i).gameObject);
            }
        }

        if (previousRight != null)
        {
            SelectModule(previousRight);
        }
    }
}
