using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public void EquippedOn(string _text)
    {
        GoEquipped.GetComponent<Text>().text = _text;
        GoEquipped.SetActive(true);
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
