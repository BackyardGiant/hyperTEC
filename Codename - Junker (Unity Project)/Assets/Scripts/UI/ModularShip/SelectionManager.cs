using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public DisplayOptions display;

    private int currentlySelectedIndex = 0;

    private bool readyForInput = true;

    public GameObject shipEngineSnap, shipLeftSnap, shipRightSnap;

    private float equippedEngineIndex, equippedLeftIndex, equippedRightIndex;

    private int[] takenIndex = new int[] { -1, -1, -1 };

    private bool leftSideSelected = true;

    // Start is called before the first frame update
    void Start()
    {
        equippedEngineIndex = -1;
        equippedLeftIndex = -1;
        equippedRightIndex = -1;
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
                if (currentlySelectedIndex == display.ModulesList.Count - 1)
                {
                    currentlySelectedIndex = 0;
                }
                else
                {
                    currentlySelectedIndex++;
                }

                if(!CheckIfAlreadyEquipped())
                {
                    RemovePreviousModule();
                    DisplayEquipped();
                    PreviewSelected(display.ModulesList[currentlySelectedIndex]);
                }

                display.UpdateHighlightPosition(currentlySelectedIndex);              
                readyForInput = false;
            }

            if ((Input.GetAxis("MacroEngine") > 0) || (Input.GetAxis("Vertical") > 0))
            {
                if (currentlySelectedIndex == 0)
                {
                    currentlySelectedIndex = display.ModulesList.Count - 1;
                }
                else
                {
                    currentlySelectedIndex--;
                }

                if (!CheckIfAlreadyEquipped())
                {
                    RemovePreviousModule();
                    DisplayEquipped();
                    PreviewSelected(display.ModulesList[currentlySelectedIndex]);
                }

                display.UpdateHighlightPosition(currentlySelectedIndex);
                readyForInput = false;
            }
        }

        if(Input.GetButtonDown("XboxY"))
        {
            leftSideSelected = !leftSideSelected;
            Debug.Log("Left side selected? : " + leftSideSelected);

            if(!CheckIfAlreadyEquipped())
            {
                RemovePreviousModule();
                DisplayEquipped();
                PreviewSelected(display.ModulesList[currentlySelectedIndex]);
            }          
        }

        if (Input.GetButtonDown("Throttle Up"))
        {
            GameObject selected = display.ModulesList[currentlySelectedIndex];

            if (selected.GetComponent<EngineStatManager>() && equippedEngineIndex != currentlySelectedIndex)
            {
                equippedEngineIndex = currentlySelectedIndex;
                takenIndex[0] = (int)equippedEngineIndex;
                Debug.Log("Equipped engine " + equippedEngineIndex);
            }
            else if (selected.GetComponent<WeaponStatManager>())
            {
                if (leftSideSelected && equippedLeftIndex != currentlySelectedIndex)
                {
                    equippedLeftIndex = currentlySelectedIndex;
                    takenIndex[1] = (int)equippedLeftIndex;
                    Debug.Log("Equipped left gun " + equippedLeftIndex);
                }
                else if(!leftSideSelected && equippedRightIndex != currentlySelectedIndex)
                {
                    equippedRightIndex = currentlySelectedIndex;
                    takenIndex[2] = (int)equippedRightIndex;
                    Debug.Log("Equipped right gun " + equippedRightIndex);
                }
            }           
        }
    }

    private bool CheckIfAlreadyEquipped()
    {
        bool isAlreadyEquipped = false;

        foreach (int index in takenIndex)
        {
            if (index == currentlySelectedIndex)
            {
                isAlreadyEquipped = true;
            }
        }

        return isAlreadyEquipped;
    }

    private void DisplayEquipped()
    {
        if(equippedEngineIndex != -1)
        {
            EngineData statBlock = display.ModulesList[(int)equippedEngineIndex].GetComponent<EngineStatManager>().Data;
            GameObject tempEngine = Instantiate(statBlock.EngineModel);

            tempEngine.transform.SetParent(shipEngineSnap.transform);
            tempEngine.transform.position = shipEngineSnap.transform.position;
            tempEngine.transform.rotation = shipEngineSnap.transform.rotation;
        }

        if(equippedLeftIndex != -1)
        {
            WeaponData statBlock = display.ModulesList[(int)equippedLeftIndex].GetComponent<WeaponStatManager>().Data;
            GameObject tempWeapon = Instantiate(statBlock.WeaponModel);

            tempWeapon.transform.SetParent(shipLeftSnap.transform);
            tempWeapon.transform.position = shipLeftSnap.transform.position;
            tempWeapon.transform.rotation = shipLeftSnap.transform.rotation;
            tempWeapon.transform.localScale = new Vector3(0.5f, 0.5f, -0.5f);
        }

        if(equippedRightIndex != -1)
        {
            WeaponData statBlock = display.ModulesList[(int)equippedRightIndex].GetComponent<WeaponStatManager>().Data;
            GameObject tempWeapon = Instantiate(statBlock.WeaponModel);

            tempWeapon.transform.SetParent(shipRightSnap.transform);
            tempWeapon.transform.position = shipRightSnap.transform.position;
            tempWeapon.transform.rotation = shipRightSnap.transform.rotation;
            tempWeapon.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

    private void PreviewSelected(GameObject selectedObject)
    {
        if (selectedObject.GetComponent<EngineStatManager>())
        {
            EngineData statBlock = selectedObject.GetComponent<EngineStatManager>().Data;
            GameObject tempEngine = Instantiate(statBlock.EngineModel);

            tempEngine.transform.SetParent(shipEngineSnap.transform);
            tempEngine.transform.position = shipEngineSnap.transform.position;
            tempEngine.transform.rotation = shipEngineSnap.transform.rotation;
        }
        else if (selectedObject.GetComponent<WeaponStatManager>())
        {
            WeaponData statBlock = selectedObject.GetComponent<WeaponStatManager>().Data;
            GameObject tempWeapon = Instantiate(statBlock.WeaponModel);

            if (leftSideSelected)
            {
                tempWeapon.transform.SetParent(shipLeftSnap.transform);
                tempWeapon.transform.position = shipLeftSnap.transform.position;
                tempWeapon.transform.rotation = shipLeftSnap.transform.rotation;
                tempWeapon.transform.localScale = new Vector3(0.5f, 0.5f, -0.5f);
            }
            else
            {
                tempWeapon.transform.SetParent(shipRightSnap.transform);
                tempWeapon.transform.position = shipRightSnap.transform.position;
                tempWeapon.transform.rotation = shipRightSnap.transform.rotation;
                tempWeapon.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
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

        tempChildrenCount = shipLeftSnap.transform.childCount;

        if (tempChildrenCount != 0)
        {
            for (int i = 0; i < tempChildrenCount; i++)
            {
                GameObject.Destroy(shipLeftSnap.transform.GetChild(i).gameObject);
            }
        }

        tempChildrenCount = shipRightSnap.transform.childCount;

        if (tempChildrenCount != 0)
        {
            for (int i = 0; i < tempChildrenCount; i++)
            {
                GameObject.Destroy(shipRightSnap.transform.GetChild(i).gameObject);
            }
        }
    }
}
