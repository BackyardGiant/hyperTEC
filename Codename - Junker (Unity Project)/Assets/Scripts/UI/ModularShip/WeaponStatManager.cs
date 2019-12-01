using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStatManager : MonoBehaviour
{
    public Text name;
    public Image thumbnail;

    [SerializeField]
    private WeaponData data;
    [SerializeField]
    private Sprite m_constructionGun, m_defaultGun, m_traderGun, m_explorerGun;
    public WeaponData Data { get => data; set => data = value; }

    public void PopulateData()
    {
        name.text = data.Name;

        switch (data.CurrentFaction)
        {
            case WeaponData.faction.construction:
                thumbnail.sprite = m_constructionGun;
                break;
            case WeaponData.faction.explorer:
                thumbnail.sprite = m_explorerGun;
                break;
            case WeaponData.faction.initial:
                thumbnail.sprite = m_defaultGun;
                break;
            case WeaponData.faction.trader:
                thumbnail.sprite = m_traderGun;
                break;
        }

        if (thumbnail.sprite == null)
        {
            Color _tempColour = thumbnail.color;
            _tempColour.a = 0f;
            thumbnail.color = _tempColour;
        }
    }
}
