using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEngine : MonoBehaviour
{

    [SerializeField]
    private GameObject SnapPoint, DefaultEngine, TraderEngine, ExplorerEngine, ConstructionEngine, RightSnap, LeftSnap;

    // Start is called before the first frame update
    void Start()
    {

        int _engineChoice = PlayerPrefs.GetInt("ChosenEngine");
        if(_engineChoice == 1)
        {
            AttachEngine(DefaultEngine);
        }
        if (_engineChoice == 2)
        {
            AttachEngine(TraderEngine);
        }
        if (_engineChoice == 3)
        {
            AttachEngine(ExplorerEngine);
        }
        if (_engineChoice == 4)
        {
            AttachEngine(ConstructionEngine);
        }

        #region GenerateDefaultGuns
        RightSnap.transform.GetChild(0).GetComponent<WeaponGenerator>().GenerateGun();
        LeftSnap.transform.GetChild(0).GetComponent<WeaponGenerator>().GenerateGun();
        #endregion
    }

    // Update is called once per frame
    void AttachEngine(GameObject _engine)
    {
        try
        {
            GameObject _obj = Instantiate(_engine, SnapPoint.transform);
            _obj.transform.localPosition = Vector3.zero;

            _obj.transform.GetChild(0).GetComponent<ThrustEffectController>().player = this.gameObject.GetComponent<PlayerMovement>();
        }
        catch {}
    }
}
