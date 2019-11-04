using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEngine : MonoBehaviour
{

    [SerializeField]
    private GameObject SnapPoint, DefaultEngine, TraderEngine, ExplorerEngine, ConstructionEngine;

    // Start is called before the first frame update
    void Start()
    {

        int _engineChoice = PlayerPrefs.GetInt("ChosenEngine");
        if(_engineChoice == 1)
        {
            AttatchEngine(DefaultEngine);
        }
        if (_engineChoice == 2)
        {
            AttatchEngine(TraderEngine);
        }
        if (_engineChoice == 3)
        {
            AttatchEngine(ExplorerEngine);
        }
        if (_engineChoice == 4)
        {
            AttatchEngine(ConstructionEngine);
        }
    }

    // Update is called once per frame
    void AttatchEngine(GameObject _engine)
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
