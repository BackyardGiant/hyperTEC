using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleElements : MonoBehaviour
{
    public GameObject GoHighlight;
    public GameObject GoEquipped;

    private void Start()
    {
        GoHighlight.SetActive(false);
    }

    public void HighlightOn()
    {
        GoHighlight.SetActive(true);
    }

    public void HighlightOff()
    {
        GoHighlight.SetActive(false);
    }
    public void EquippedOn()
    {
        GoEquipped.SetActive(true);
    }

    public void EquippedOff()
    {
        GoEquipped.SetActive(false);
    }

    public bool IsEquipped()
    {
        return GoEquipped.activeInHierarchy;
    }
}
