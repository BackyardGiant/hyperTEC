using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEngine : MonoBehaviour
{

    [SerializeField]
    private GameObject SnapPoint, DefaultEngine, TraderEngine, ExplorerEngine, ConstructionEngine, RightSnap, LeftSnap;

    //public Inventory playerInventory;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerInventoryManager.Instance.EquippedEngine != null)
        {
            GameObject _engine = ModuleManager.Instance.GenerateEngine(PlayerInventoryManager.Instance.EquippedEngine);

            _engine.transform.SetParent(SnapPoint.transform);
            _engine.transform.localPosition = Vector3.zero;

            _engine.transform.GetChild(0).GetComponent<ThrustEffectController>().player = this.gameObject.GetComponent<PlayerMovement>();

            PlayerInventoryManager.Instance.EquippedEngine.Seed = string.Format("{0}", PlayerInventoryManager.Instance.EquippedEngine.EngineId + 1);
        }

        //PlayerInventoryManager.Instance.EquippedEngine = new EngineData();
        
        //int _engineChoice = 1;
        //if(_engineChoice == 1)
        //{
        //    AttachEngine(DefaultEngine);
        //}
        //if (_engineChoice == 2)
        //{
        //    AttachEngine(TraderEngine);
        //}
        //if (_engineChoice == 3)
        //{
        //    AttachEngine(ExplorerEngine);
        //}
        //if (_engineChoice == 4)
        //{
        //    AttachEngine(ConstructionEngine);
        //}

        #region GenerateDefaultGuns

        if (PlayerInventoryManager.Instance.EquippedLeftWeapon != null)
        {
            GameObject _leftGun = ModuleManager.Instance.GenerateWeapon(PlayerInventoryManager.Instance.EquippedLeftWeapon);
            _leftGun.transform.SetParent(LeftSnap.transform);
            _leftGun.transform.localPosition = Vector3.zero;
            _leftGun.transform.localRotation = Quaternion.identity;
            _leftGun.transform.localScale = new Vector3(1, 1, 1);
        }

        if (PlayerInventoryManager.Instance.EquippedRightWeapon != null)
        {
            GameObject _rightGun = ModuleManager.Instance.GenerateWeapon(PlayerInventoryManager.Instance.EquippedRightWeapon);
            _rightGun.transform.SetParent(RightSnap.transform);
            _rightGun.transform.position = Vector3.zero;
            _rightGun.transform.localPosition = Vector3.zero;
            _rightGun.transform.localRotation = Quaternion.identity;
            _rightGun.transform.localScale = new Vector3(1, 1, 1);
        }
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
