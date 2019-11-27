 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    private static HUDManager s_instance;
    public GameObject Player;
    public Camera Camera;
    public Sprite TargetSprite;
    public GameObject Explosion;
    public Transform WaypointsAndMarkers;
    public Image Healthbar;

    //public Inventory playerInv;

    #region EnemyIndicators
    [Header ("Enemy Indicator System"), Space(20)]
    public Sprite enemyArrowPointer;
    [SerializeField, Tooltip("Colour of the enemy indicators")]
    private Color m_enemyTargetColour;
    [SerializeField, Tooltip("Scale of the enemy indicators")]
    private float m_enemyTargetSize;
    [SerializeField, Tooltip("Scale of the Arrow indicators")]
    private float m_enemyArrowSize;
    [SerializeField, Tooltip("Distance at which enemies should be detected")]
    private int m_viewDistance;
    private float m_arrowClampAngle;
    #endregion
    #region LootIndicators
    [Header("Loot Indicator System"),Space(20)]
    public GameObject LootDisplay;
    public Image Scanner;
    public Image Destroyer;
    [SerializeField, Tooltip("Colour of the loot indicators")]
    private Color m_lootTargetColour;
    [SerializeField, Tooltip("Scale of the loot indicators")]
    private float m_lootTargetSize;
    [SerializeField, Tooltip("Distance at which loot should be detected")]
    private int m_lootViewDistance;
    [SerializeField, Tooltip("How far away from the target that the loot display appears"),Range(0f,0.2f)]
    private float m_displayOffset;
    [SerializeField, Tooltip("How quickly the scan occurs. Higher is faster."), Range(0.2f,1f)]
    private float m_scanningFillSpeed;
    [SerializeField, Tooltip("How quickly the destroy occurs. Higher is faster."), Range(0.2f, 1f)]
    private float m_destroyFillSpeed;
    [SerializeField, Header("Loot Display Elements"), Tooltip("Display elements of the Loot Display. This will show the stats of the visible loot item."), Space(20)]
    private Image m_lootItemIcon;
    [SerializeField]
    private Sprite m_weapon, m_engine, m_shield;
    [SerializeField]
    private TextMeshProUGUI m_lootDisplayTitle, m_lootTitle;
    [SerializeField, Tooltip("Each stat item. Needs the value first then the arrow second.")]
    private GameObject[] m_stat1, m_stat2, m_stat3, m_stat4;
    #endregion
    #region QuestIndicators
    [Header("Quest Indicator System"), Space(20)]
    public GameObject QuestDisplay;
    public Image QuestScanner;
    public Image QuestDestroyer;
    [SerializeField, Tooltip("Colour of the loot indicators")]
    private Color m_questTargetColour;
    [SerializeField, Tooltip("Scale of the loot indicators")]
    private float m_questTargetSize;
    [SerializeField, Tooltip("Distance at which loot should be detected")]
    private int m_questViewDistance;
    [SerializeField, Tooltip("How far away from the target that the loot display appears"), Range(0f, 0.2f)]
    private float m_questDisplayOffset;
    [SerializeField, Tooltip("How quickly the scan occurs. Higher is faster."), Range(0.2f, 1f)]
    private float m_questScanningFillSpeed;
    [SerializeField, Tooltip("How quickly the destroy occurs. Higher is faster."), Range(0.2f, 1f)]
    private float m_questDestroyFillSpeed;
    [SerializeField, Header("Quest Display Elements"), Tooltip("Display elements of the Loot Display. This will show the stats of the visible loot item."), Space(20)]
    private Image m_questItemIcon;
    [SerializeField]
    private Sprite m_kill, m_collect, m_target, m_recon, m_control;
    [SerializeField]
    private TextMeshProUGUI m_questDisplayTitle, m_questDisplayObjectTitle, questDescription, questReward;
    #endregion
    #region AutoAim
    private GameObject m_closestEnemy;
    private Vector2 m_closestEnemyScreenPos;
    #endregion
    #region QuestDisplay
    [SerializeField, Header("Quest Display Text Elements")]
    private TextMeshProUGUI m_questTitle;
    [SerializeField]
    private TextMeshProUGUI m_questDescription,m_questProgress;
    [SerializeField]
    private TextMeshProUGUI m_completedQuestTitle, m_completedQuestDescription;
    [SerializeField]
    private Animator m_completedQuestAnimator;

    #endregion

    [SerializeField, Header("On Screen Objects"), Space(20)]
    private GameObject m_warningObject;
    [SerializeField]
    private GameObject m_hitmarkerObject;

    private bool m_displayAnimated;
    private bool m_enablePickup;
    private bool m_currentlyClosingScan = false;
    private bool m_currentlyScanning = false;
    private bool m_displayQuestAnimated;
    private bool m_enableQuestPickup;
    private bool m_currentlyClosingQuestScan = false;
    private bool m_currentlyQuestScanning = false;
    private GameObject m_displayDismissed;
    private GameObject m_currentTarget;
    private GameObject m_prevTarget;
    private Vector2 m_crosshairPosition;
    private float m_buttonHoldTime = 0;
    private int m_buttonBeingHeld = -1;

    //private GameObject m_currentBeacon;
    //private GameObject m_prevBeacon;

    #region Accessors
    public static HUDManager Instance { get => s_instance; set => s_instance = value; }
    public int ViewDistance { get => m_viewDistance; }
    public int LootViewDistance { get => m_lootViewDistance; }
    public GameObject ClosetEnemy { get => m_closestEnemy; set => m_closestEnemy = value; }
    public Vector2 ClosestEnemyScreenPos { get => m_closestEnemyScreenPos; set => m_closestEnemyScreenPos = value; }
    #endregion

    void Start()
    {
        HideWarning();
        m_enablePickup = true;
        //Calculate Clamp angle of arrow. This is equal to the angle at which the enemy is to the behind of the player. Working out this value prevents arrows floating around the screen.
        m_displayAnimated = false;
        m_arrowClampAngle = Mathf.Asin((Screen.height) / Mathf.Sqrt((m_viewDistance * m_viewDistance) + (Screen.height * Screen.height)));
        m_arrowClampAngle = m_arrowClampAngle * Mathf.Rad2Deg;
        m_crosshairPosition = new Vector2(Screen.width / 2, Screen.height / 2);
        Destroyer.fillAmount = 0;
        Scanner.fillAmount = 0;
        // 18/11/19 - TEMPORARILY MAKE SURE YOU'RE TRACKING THE RIGHT QUEST.
        QuestManager.Instance.TrackingQuestIndex = 0;
    }
    void Awake()
    {
        //Singleton Implementation
        if (s_instance == null)
        {
            s_instance = this;
        }
        else
        {
            Destroy(s_instance.gameObject);
            s_instance = this;
        }
    }
    void Update()
    {
        #region LootInteraction

        if (m_displayAnimated == false)
        {
            m_buttonBeingHeld = -1;
        }

        if(Input.GetButton("Interact") && m_displayAnimated == true)
        {
            m_buttonBeingHeld = 0;
        }
        else if(Input.GetButton("Dismiss") && m_displayAnimated == true)
        {
            m_buttonBeingHeld = 1;
        }

        if (m_buttonBeingHeld != -1 && m_displayAnimated == true)
        {
            m_buttonHoldTime += Time.deltaTime * m_scanningFillSpeed;
        }

        if(m_currentTarget != null)
        {
            if (m_currentTarget.tag == "Component")
            {
                if (QuestDisplay.transform.localScale.x >= 0.17)
                {
                    ClearBeaconDisplay();
                }
                if (Input.GetButtonUp("Interact") && m_buttonHoldTime < 0.3f && m_displayAnimated == true && m_enablePickup == true)
                {
                    //Make pickup item code here
                    GameObject _pickupLoot = m_currentTarget;

                    if (m_currentTarget.tag == "Component")
                    {
                        if (m_currentTarget.GetComponent<LootDetection>().LootType == LootDetection.m_lootTypes.Weapon)
                        {
                            PlayerInventoryManager.Instance.AvailableWeapons.Add(m_currentTarget.transform.GetChild(0).GetComponent<WeaponGenerator>().statBlock);
                        }
                        else if (m_currentTarget.GetComponent<LootDetection>().LootType == LootDetection.m_lootTypes.Engine)
                        {
                            PlayerInventoryManager.Instance.AvailableEngines.Add(m_currentTarget.transform.GetChild(0).GetComponent<EngineGenerator>().engineStatBlock);
                        }
                    }

                    IncrementPlayerPref("WeaponsCollected");
                    Debug.Log("Pickup Loot.");

                    Destroy(m_currentTarget);
                    ClearLootDisplay();
                    ClearLootTarget(m_currentTarget.GetComponent<LootDetection>());

                    Scanner.fillAmount = 0;
                    m_buttonBeingHeld = -1;
                    m_buttonHoldTime = 0;
                }

                if (m_buttonBeingHeld == 0 && m_buttonHoldTime >= 0.3f)
                {
                    Scanner.fillAmount = m_buttonHoldTime - 0.3f;
                    DisplayLootStats(m_currentTarget);
                }

                if (Input.GetButtonUp("Interact"))
                {
                    Scanner.fillAmount = 0;
                    m_buttonBeingHeld = -1;
                    m_buttonHoldTime = 0;
                }

                //Full scan complete, open and animate display.
                if (Scanner.fillAmount == 1 && m_currentlyScanning == false)
                {
                    m_enablePickup = false;
                    m_currentlyScanning = true;
                    IncrementPlayerPref("WeaponsScanned");
                    Scanner.fillAmount = 0;
                    m_buttonBeingHeld = -1;
                    m_buttonHoldTime = 0;
                    LootDisplay.GetComponent<Animator>().Play("DisplayStats");

                    Invoke("togglePickup", .2f);
                }

                //Full close complete, close and animate display.
                if (Scanner.fillAmount == 1 && m_currentlyScanning == true)
                {
                    m_enablePickup = false;
                    m_currentlyScanning = false;
                    Scanner.fillAmount = 0;
                    m_buttonBeingHeld = -1;
                    m_buttonHoldTime = 0;
                    LootDisplay.GetComponent<Animator>().Play("HideStats");
                    m_currentlyClosingScan = true;

                    Invoke("togglePickup", .2f);
                }

                //Dismiss being held down fills in the button.
                if (m_buttonBeingHeld == 1 && m_displayAnimated == true)
                {
                    Destroyer.fillAmount = m_buttonHoldTime;
                }

                //Letting go of the Dismiss button clears the progress
                if (Input.GetButtonUp("Dismiss"))
                {
                    Destroyer.fillAmount = 0;
                    m_buttonBeingHeld = -1;
                    m_buttonHoldTime = 0;
                }

                //Destroy the target object.
                if (Destroyer.fillAmount == 1 && m_displayAnimated == true)
                {
                    //Make it explode in here
                    Destroyer.fillAmount = 0;
                    m_buttonBeingHeld = -1;
                    m_buttonHoldTime = 0;
                    GameObject explosion = Instantiate(Explosion, m_currentTarget.transform.position, m_currentTarget.transform.rotation);
                    explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    AudioManager.Instance.PlayWorld("ExplosionShort3", m_currentTarget.gameObject, true, false);
                    if (m_currentTarget.tag == "Component")
                    {
                        ClearLootTarget(m_currentTarget.GetComponent<LootDetection>());
                    }
                    Destroy(m_currentTarget);
                    ClearLootDisplay();

                }
            }
            else if (m_currentTarget.tag == "QuestBeacon")
            {
                if (LootDisplay.transform.localScale.x >= 0.17)
                {
                    ClearLootDisplay();
                }
                if (Input.GetButtonUp("Interact") && m_buttonHoldTime < 0.3f && m_displayAnimated == true && m_enableQuestPickup == true)
                {
                    //Make pickup item code here
                    GameObject _questAccepted = m_currentTarget;

                    Quest _quest = m_currentTarget.GetComponent<QuestBeconDetection>().Quest;

                    switch (m_currentTarget.GetComponent<QuestBeconDetection>().QuestType)
                    {
                        case QuestType.kill:
                            QuestManager.Instance.CreateKillQuest(_quest.Size, _quest.Name, _quest.Description);
                            break;
                        case QuestType.collect:
                            //QuestManager.Instance.CreateCollectQuest(_quest.)
                            break;
                        case QuestType.control:
                            break;
                        case QuestType.recon:
                            break;
                        case QuestType.targets:
                            break;
                    }


                    if (m_currentTarget.GetComponent<QuestBeconDetection>().QuestType == QuestType.kill)
                    {
                        QuestManager.Instance.CreateKillQuest(_quest.Size, _quest.Name, _quest.Description);
                    }

                    Debug.Log("Quest Collected");

                    Destroy(m_currentTarget);
                    ClearBeaconDisplay();
                    ClearBeaconTarget(m_currentTarget.GetComponent<QuestBeconDetection>());

                    QuestScanner.fillAmount = 0;
                    m_buttonBeingHeld = -1;
                    m_buttonHoldTime = 0;
                }

                if (m_buttonBeingHeld == 0 && m_buttonHoldTime >= 0.3f)
                {
                    QuestScanner.fillAmount = m_buttonHoldTime - 0.3f;
                    //DisplayLootStats(m_currentTarget);
                }

                if (Input.GetButtonUp("Interact"))
                {
                    QuestScanner.fillAmount = 0;
                    m_buttonBeingHeld = -1;
                    m_buttonHoldTime = 0;
                }

                //Full scan complete, open and animate display.
                if (QuestScanner.fillAmount == 1 && m_currentlyQuestScanning == false)
                {
                    m_enableQuestPickup = false;
                    m_currentlyQuestScanning = true;
                    QuestScanner.fillAmount = 0;
                    m_buttonBeingHeld = -1;
                    m_buttonHoldTime = 0;
                    QuestDisplay.GetComponent<Animator>().Play("QuestScan");

                    Invoke("toggleQuestPickup", .2f);
                }

                //Full close complete, close and animate display.
                if (QuestScanner.fillAmount == 1 && m_currentlyQuestScanning == true)
                {
                    m_enableQuestPickup = false;
                    m_currentlyQuestScanning = false;
                    QuestScanner.fillAmount = 0;
                    m_buttonBeingHeld = -1;
                    m_buttonHoldTime = 0;
                    QuestDisplay.GetComponent<Animator>().Play("QuestCloseScan");
                    m_currentlyClosingQuestScan = true;

                    Invoke("toggleQuestPickup", .2f);
                }

                //Dismiss being held down fills in the button.
                if (m_buttonBeingHeld == 1 && m_displayQuestAnimated == true)
                {
                    QuestDestroyer.fillAmount = m_buttonHoldTime;
                }

                //Letting go of the Dismiss button clears the progress
                if (Input.GetButtonUp("Dismiss"))
                {
                    QuestDestroyer.fillAmount = 0;
                    m_buttonBeingHeld = -1;
                    m_buttonHoldTime = 0;
                }

                //Destroy the target object.
                if (QuestDestroyer.fillAmount == 1 && m_displayQuestAnimated == true)
                {
                    //Make it explode in here
                    QuestDestroyer.fillAmount = 0;
                    m_buttonBeingHeld = -1;
                    m_buttonHoldTime = 0;
                    GameObject explosion = Instantiate(Explosion, m_currentTarget.transform.position, m_currentTarget.transform.rotation);
                    explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    AudioManager.Instance.PlayWorld("ExplosionShort3", m_currentTarget.gameObject, true, false);
                    ClearBeaconTarget(m_currentTarget.GetComponent<QuestBeconDetection>());
                    Destroy(m_currentTarget);
                    ClearBeaconDisplay();

                }
            }
        }

        if (m_buttonBeingHeld == -1)
        {
            Destroyer.fillAmount = 0;
            Scanner.fillAmount = 0;
            m_buttonHoldTime = 0;
        }

        if(m_currentTarget == null && m_displayAnimated == true)
        {
            ClearLootDisplay();
            ClearBeaconDisplay();
        }
        
        #endregion
        DisplayActiveQuest();
    }

    #region Enemy Detection Methods
    // The argument enemy passed in is the same as the enemy calling the function so that the targets stay encapsulated.
    public void DrawEnemyTarget(Vector2 _screenPos, EnemyDetection _enemy)
    {
        GameObject _target;
        Image _targetImage;

        if (_enemy.EnemyTarget != null)
        {
            _target = _enemy.EnemyTarget;
            _targetImage = _target.GetComponent<Image>();
        }
        else
        {
            _target = new GameObject();
            _target.name = "EnemyTarget";
            _target.transform.parent = WaypointsAndMarkers;

            //Setting the sprite
            _targetImage = _target.AddComponent<Image>();
            _targetImage.color = m_enemyTargetColour;
            _enemy.EnemyTarget = _target;
            _target.tag = "TargetUI";
        }

        //moveit
        float _distancePercentage =  1 - Vector3.Distance(Player.transform.position, _enemy.GetComponent<Transform>().position) / m_viewDistance;
        float _finalSize = 0.05f + _distancePercentage * m_enemyTargetSize ;





        _targetImage.rectTransform.localScale = new Vector3(_finalSize,_finalSize,_finalSize);




        _targetImage.sprite =TargetSprite;
        _targetImage.transform.position = _screenPos;
        _targetImage.transform.localEulerAngles = Vector3.zero;


        if (m_closestEnemy == null || Vector2.Distance(_screenPos, m_crosshairPosition) < Vector2.Distance(m_closestEnemyScreenPos, m_crosshairPosition))
        {
            m_closestEnemy = _enemy.gameObject;
            if (Vector3.Distance(m_closestEnemy.transform.position, Player.transform.position) < (0.7 * m_viewDistance))
            {
                m_closestEnemyScreenPos = _screenPos;
            }
            else
            {
                m_closestEnemyScreenPos = new Vector2(10, 10);
            }
        }        

        if (m_closestEnemy == _enemy.gameObject)
        {
            if (Vector3.Distance(m_closestEnemy.transform.position, Player.transform.position) < (0.7 * m_viewDistance))
            {
                m_closestEnemyScreenPos = _screenPos;
            }
            else
            {
                m_closestEnemyScreenPos = new Vector2(10, 10);
            }
        }


    }
    // The argument enemy passed in is the same as the enemy calling the function so that the targets stay encapsulated.
    public void DrawEnemyArrow(Vector3 _screenPos, EnemyDetection _enemy)
    {
        GameObject _target;
        Image _targetImage;
        //Draw arrow towards enemy if they're off screen and within range.
        if (_enemy.EnemyTarget != null)
        {
            _target = _enemy.EnemyTarget;
            _targetImage = _target.GetComponent<Image>();
        }
        else
        {
            _target = new GameObject();
            _target.name = "EnemyTarget";
            _target.transform.parent = WaypointsAndMarkers;

            //Setting the sprite
            _targetImage = _target.AddComponent<Image>();
            _targetImage.color = m_enemyTargetColour;
            _enemy.EnemyTarget = _target;
            _target.tag = "TargetUI";
        }




        _targetImage.rectTransform.localScale = new Vector3(m_enemyArrowSize, m_enemyArrowSize, m_enemyArrowSize);




        _targetImage.sprite = enemyArrowPointer;

        //Rotate Arrow towards enemy.
        Vector3 _difference = _screenPos - _targetImage.rectTransform.position;
        _difference.Normalize();
        float _rotZ = Mathf.Atan2(_difference.y, _difference.x) * Mathf.Rad2Deg;
        Quaternion _newRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, _rotZ + 270f));
        _targetImage.rectTransform.rotation = _newRotation;

        //Clamp arrow position to edge of the screen.
        _screenPos.y = Mathf.Clamp(_screenPos.y, 20, Screen.height - 20);
        _screenPos.x = Mathf.Clamp(_screenPos.x, 20, Screen.width - 20);

        //Clamp arrow position to a circle around the centre. IT BROKEN
        //_screenPos = Vector2.ClampMagnitude(_screenPos, Screen.height / 5);
        //_screenPos.x += Screen.width / 2;
        //_screenPos.y += Screen.height / 2;




        Vector3 _enemyToPlayer = _enemy.transform.position - Player.transform.position;
        float _angleBehind = Vector3.Angle(_enemyToPlayer, -Player.transform.forward);


        if (_angleBehind > m_arrowClampAngle)
        {
            _targetImage.rectTransform.position = _screenPos;
        }
        else
        {
            ClearEnemyDetection(_enemy);
        }
    }
    //Clears the target of the enemy.
    public void ClearEnemyDetection(EnemyDetection _enemy)
    {
        Destroy(_enemy.EnemyTarget);
    }
    #endregion

    #region Loot Detection Methods

    //Draws the diamond on the screenspace position of the loot.
    public void DrawLootTarget(Vector2 _screenPos, LootDetection _loot)
    {
        GameObject _lootObject;
        Image _targetImage;

        if (_loot.LootTarget != null)
        {
            _lootObject = _loot.LootTarget;
            _targetImage = _lootObject.GetComponent<Image>();
        }
        else
        {
            _lootObject = new GameObject();
            _lootObject.name = "LootTarget";
            _lootObject.transform.parent = WaypointsAndMarkers;

            //Setting the sprite
            _targetImage = _lootObject.AddComponent<Image>();
            _targetImage.color = m_lootTargetColour;
            _loot.LootTarget = _lootObject;
        }
        //moveit
        _targetImage.rectTransform.localScale = new Vector3(m_lootTargetSize, m_lootTargetSize, m_lootTargetSize);
        _targetImage.sprite = TargetSprite;
        _targetImage.transform.position = _screenPos;
        _targetImage.transform.localEulerAngles = Vector3.zero;


        //Find all "component" tagged game objects
        GameObject[] _lootObjects = GameObject.FindGameObjectsWithTag("Component");
        GameObject[] _questObjects = GameObject.FindGameObjectsWithTag("QuestBeacon");

        GameObject[] _objects = new GameObject[_lootObjects.Length + _questObjects.Length];

        _lootObjects.CopyTo(_objects, 0);
        _questObjects.CopyTo(_objects, _lootObjects.Length);

        m_currentTarget = ReturnTarget(_objects);
        //If targeted loot has changed, reset LootDisplay.
        if (m_prevTarget != m_currentTarget)
        {
            // Swapping over the LootDisplays to a different object
            m_displayAnimated = false;
            m_currentlyScanning = false;
            Destroyer.fillAmount = 0;
            Scanner.fillAmount = 0;
        }
        m_prevTarget = m_currentTarget;

        if (m_currentTarget == null)
        {
            Debug.Log("<color=red> Current Loot is null. </color> Do all loot objects have the component tag?");
        }

        if (m_currentTarget.tag == "Component")
        {
            //Screenpos of loot, and LootDetection script. This will aid in automatically entering values from a piece of loot.
            _screenPos = Camera.WorldToScreenPoint(m_currentTarget.transform.position);
            _loot = m_currentTarget.GetComponent<LootDetection>();


            DrawDisplay(_screenPos);
        }

    }
    //Clear the target for a loot. Also close/hide the Loot Display.
    public void ClearLootTarget(LootDetection _loot)
    {
        Destroy(_loot.LootTarget);
        //Need to make it clear the loot Display if there isn't any loot left on screen.
        if (countTargets(0) == 0 && m_displayAnimated == true)
        {
            ClearLootDisplay();
        }
    }
    //Clears the Loot Display
    private void ClearLootDisplay()
    {
        Scanner.fillAmount = 0;
        Destroyer.fillAmount = 0;
        m_displayAnimated = false;
        m_currentlyClosingScan = false;
        m_currentlyScanning = false;
        if (m_currentlyScanning == true)
        {
            LootDisplay.GetComponent<Animator>().Play("CloseFromScanned");
        }
        else if (m_currentlyScanning == false)
        {
            LootDisplay.GetComponent<Animator>().Play("CloseFromUnscanned");
        }


    }
    //Returns the loot which is closest to the crosshair. This acts as automatic targetting of loot.
    private GameObject ReturnTarget(GameObject[] _visibleObjects)
    {
        GameObject _target = null;
        float _currentDistance = 0f;
        float _minimumDistance = Mathf.Infinity;
        Vector2 _crosshairPosition = new Vector2(Screen.width / 2,Screen.height / 2);

        foreach (GameObject _objTarget in _visibleObjects)
        {
            _currentDistance = Vector2.Distance(_crosshairPosition, Camera.WorldToScreenPoint(_objTarget.transform.position));
            if (_currentDistance <_minimumDistance && Vector3.Distance(_objTarget.transform.position,Player.transform.position) < m_lootViewDistance)
            {
                _target = _objTarget;
                _minimumDistance = _currentDistance;
            }
        }

        return _target;
    }
    private int countTargets(int _type)
    {
        if(_type == 0)
        {
            int _targetCount = 0;
            GameObject[] _lootObjects = GameObject.FindGameObjectsWithTag("Component");

            List<GameObject> _visibleObjects = new List<GameObject>();

            for (int i = 0; i < _lootObjects.Length; i++)
            {
                float _distance = Vector3.Distance(Player.transform.position, _lootObjects[i].transform.position);
                if (IsVisibleFrom(_lootObjects[i].GetComponent<Renderer>(), Camera) && _distance < m_lootViewDistance)
                {
                    _visibleObjects.Add(_lootObjects[i]);
                }
            }
            _targetCount = _visibleObjects.Count;
            return _targetCount;
        }
        else if(_type == 1)
        {
            int _targetCount = 0;
            GameObject[] _questObjects = GameObject.FindGameObjectsWithTag("QuestBeacon");

            List<GameObject> _visibleObjects = new List<GameObject>();

            for (int i = 0; i < _questObjects.Length; i++)
            {
                float _distance = Vector3.Distance(Player.transform.position, _questObjects[i].transform.position);
                if (IsVisibleFrom(_questObjects[i].GetComponent<Renderer>(), Camera) && _distance < m_lootViewDistance)
                {
                    _visibleObjects.Add(_questObjects[i]);
                }
            }
            _targetCount = _visibleObjects.Count;
            return _targetCount;

        }
        else
        {
            return 0;
        }
    }
    private int countTargets()
    {
        int _targetCount = 0;
        GameObject[] _lootObjects = GameObject.FindGameObjectsWithTag("Component");
        GameObject[] _questObjects = GameObject.FindGameObjectsWithTag("QuestBeacon");

        GameObject[] _objects = new GameObject[_lootObjects.Length + _questObjects.Length];

        _lootObjects.CopyTo(_objects, 0);
        _questObjects.CopyTo(_objects, _lootObjects.Length);

        List<GameObject> _visibleObjects = new List<GameObject>();

        for (int i = 0; i < _objects.Length; i++)
        {
            float _distance = Vector3.Distance(Player.transform.position, _objects[i].transform.position);
            if (IsVisibleFrom(_objects[i].GetComponent<Renderer>(), Camera) && _distance < m_lootViewDistance)
            {
                _visibleObjects.Add(_objects[i]);
            }
        }
        _targetCount = _visibleObjects.Count;
        return _targetCount;
    }
    //Draws the lootdisplay with appropriate offset based on the current function.
    private void DrawDisplay(Vector2 _targetPos)
    {
        //Check player is going slow enough to look at it.
        Vector3 _displayTargetPos;

        if (m_currentTarget.tag == "Component")
        {
            DisplayLootTitle(m_currentTarget);

            if (QuestDisplay.transform.localScale.x >= 0.17)
            {
                if (m_currentlyScanning == true)
                {
                    QuestDisplay.GetComponent<Animator>().Play("CloseFromScanned");
                }
                else if (m_currentlyScanning == false)
                {
                    QuestDisplay.GetComponent<Animator>().Play("CloseFromUnscanned");
                }
            }

            if (Player.GetComponent<PlayerMovement>().CurrentSpeed / Player.GetComponent<PlayerMovement>().MaxAcceleration < 0.5)
            {
                if (!m_displayAnimated)
                {
                    LootDisplay.GetComponent<Animator>().Play("ShowLoot");
                    m_displayAnimated = true;
                }
            }
            if (m_currentlyScanning)
            {
                _displayTargetPos = new Vector3(_targetPos.x, _targetPos.y + (m_displayOffset * Screen.height * 3f));
                LootDisplay.GetComponent<RectTransform>().position = Vector3.Lerp(LootDisplay.GetComponent<RectTransform>().position, _displayTargetPos, 0.08f);

            }
            else if (m_currentlyClosingScan)
            {
                _displayTargetPos = new Vector3(_targetPos.x, _targetPos.y + m_displayOffset * Screen.height);
                LootDisplay.GetComponent<RectTransform>().position = Vector3.MoveTowards(LootDisplay.GetComponent<RectTransform>().position, _displayTargetPos, 0.000006f * Mathf.Pow(Vector3.Distance(LootDisplay.GetComponent<RectTransform>().position, _displayTargetPos), 3));
                if (Vector3.Distance(LootDisplay.GetComponent<RectTransform>().position, _displayTargetPos) < 10)
                {
                    m_currentlyClosingScan = false;
                }
            }
            else
            {
                _displayTargetPos = new Vector3(_targetPos.x, _targetPos.y + m_displayOffset * Screen.height);
                LootDisplay.GetComponent<RectTransform>().position = _displayTargetPos;
            }
        }
        else if(m_currentTarget.tag == "QuestBeacon")
        {
            DisplayBeconTitle(m_currentTarget);

            if (QuestDisplay.transform.localScale.x >= 0.17)
            {
                if (m_currentlyQuestScanning == true)
                {
                    QuestDisplay.GetComponent<Animator>().Play("QuestCloseFromScanned");
                }
                else if (m_currentlyQuestScanning == false)
                {
                    QuestDisplay.GetComponent<Animator>().Play("QuestCloseFromUnscanned");
                }
            }

            if (Player.GetComponent<PlayerMovement>().CurrentSpeed / Player.GetComponent<PlayerMovement>().MaxAcceleration < 0.5)
            {
                if (!m_displayQuestAnimated)
                {
                    QuestDisplay.GetComponent<Animator>().Play("OpenQuestDisplay");
                    m_displayAnimated = true;   
                }
            }
            if (m_currentlyQuestScanning)
            {
                _displayTargetPos = new Vector3(_targetPos.x, _targetPos.y + (m_displayOffset * Screen.height * 3f));
                QuestDisplay.GetComponent<RectTransform>().position = Vector3.Lerp(QuestDisplay.GetComponent<RectTransform>().position, _displayTargetPos, 0.08f);

            }
            else if (m_currentlyClosingQuestScan)
            {
                _displayTargetPos = new Vector3(_targetPos.x, _targetPos.y + m_displayOffset * Screen.height);
                QuestDisplay.GetComponent<RectTransform>().position = Vector3.MoveTowards(QuestDisplay.GetComponent<RectTransform>().position, _displayTargetPos, 0.000006f * Mathf.Pow(Vector3.Distance(QuestDisplay.GetComponent<RectTransform>().position, _displayTargetPos), 3));
                if (Vector3.Distance(QuestDisplay.GetComponent<RectTransform>().position, _displayTargetPos) < 10)
                {
                    m_currentlyClosingQuestScan = false;
                }
            }
            else
            {
                _displayTargetPos = new Vector3(_targetPos.x, _targetPos.y + m_displayOffset * Screen.height);
                QuestDisplay.GetComponent<RectTransform>().position = _displayTargetPos;
            }
        }
    }
    private void DisplayLootStats(GameObject _currentLoot)
    {
        if (_currentLoot != null)
        {
            try { 
                if(_currentLoot.GetComponent<LootDetection>().LootType == LootDetection.m_lootTypes.Weapon)
                {

                    WeaponData _lootData = _currentLoot.transform.GetChild(0).GetComponent<WeaponGenerator>().statBlock;
                    float _currentDamage = 0;
                    float _currentFireRate = 0;
                    float _currentReloadTime = 0;
                    float _currentAccuracy = 0;

                    try
                    {
                        if (PlayerInventoryManager.Instance.EquippedLeftWeapon.Damage < PlayerInventoryManager.Instance.EquippedRightWeapon.Damage) { _currentDamage = PlayerInventoryManager.Instance.EquippedLeftWeapon.Damage; }
                        else { _currentDamage = PlayerInventoryManager.Instance.EquippedRightWeapon.Damage; }

                        if (PlayerInventoryManager.Instance.EquippedLeftWeapon.FireRate < PlayerInventoryManager.Instance.EquippedRightWeapon.FireRate) { _currentFireRate = PlayerInventoryManager.Instance.EquippedLeftWeapon.FireRate; }
                        else { _currentFireRate = PlayerInventoryManager.Instance.EquippedRightWeapon.FireRate; }

                        if (PlayerInventoryManager.Instance.EquippedLeftWeapon.ReloadTime < PlayerInventoryManager.Instance.EquippedRightWeapon.ReloadTime) { _currentReloadTime = PlayerInventoryManager.Instance.EquippedLeftWeapon.ReloadTime; }
                        else { _currentReloadTime = PlayerInventoryManager.Instance.EquippedRightWeapon.ReloadTime; }

                        if (PlayerInventoryManager.Instance.EquippedLeftWeapon.Accuracy < PlayerInventoryManager.Instance.EquippedRightWeapon.Accuracy) { _currentAccuracy = PlayerInventoryManager.Instance.EquippedLeftWeapon.Accuracy; }
                        else { _currentAccuracy = PlayerInventoryManager.Instance.EquippedRightWeapon.Accuracy; }
                    }
                    catch { };
                        
                    m_lootTitle.text = _lootData.Name;

                    Image _damageArrow = m_stat1[2].GetComponent<Image>();
                    Image _fireRateArrow = m_stat2[2].GetComponent<Image>();
                    Image _reloadArrow = m_stat3[2].GetComponent<Image>();
                    Image _accuracyArrow = m_stat4[2].GetComponent<Image>();

                    ///////////////////////////////////////////////
                    m_stat1[0].GetComponent<TextMeshProUGUI>().text = "Damage";
                    m_stat1[1].GetComponent<TextMeshProUGUI>().text = DisplayNiceStats(_lootData.Damage);// _lootData.Damage.ToString();
                    if (_lootData.Damage > _currentDamage)
                    {
                        //Higher - Green Arrow
                        _damageArrow.enabled = true;
                        _damageArrow.color = Color.green;
                        _damageArrow.rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
                    }
                    else if (_lootData.Damage < _currentDamage)
                    {
                        //Lower - Red Arrow
                        _damageArrow.enabled = true;
                        _damageArrow.color = Color.red;
                        _damageArrow.rectTransform.localRotation = Quaternion.Euler(0, 0, 180);
                    }
                    else
                    {
                        // Equal - Hide Arrow
                        _damageArrow.enabled = false;
                    }

                    //////////////////////////////////////////////
                    m_stat2[0].GetComponent<TextMeshProUGUI>().text = "Fire Rate";
                    m_stat2[1].GetComponent<TextMeshProUGUI>().text = DisplayNiceStats(_lootData.FireRate);// _lootData.FireRate.ToString();
                    if (_lootData.FireRate > _currentFireRate)
                    {
                        //Higher - Green Arrow
                        _fireRateArrow.enabled = true;
                        _fireRateArrow.color = Color.green;
                        _fireRateArrow.rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
                    }
                    else if (_lootData.FireRate < _currentFireRate)
                    {
                        //Lower - Red Arrow
                        _fireRateArrow.enabled = true;
                        _fireRateArrow.color = Color.red;
                        _fireRateArrow.rectTransform.localRotation = Quaternion.Euler(0, 0, 180);
                    }
                    else
                    {
                        // Equal - Hide Arrow
                        _fireRateArrow.enabled = false;
                    }

                    /////////////////////////////////////////////////
                    m_stat3[0].GetComponent<TextMeshProUGUI>().text = "Reload Time";
                    m_stat3[1].GetComponent<TextMeshProUGUI>().text = DisplayNiceStats(_lootData.ReloadTime);// _lootData.ReloadTime.ToString();
                    if (_lootData.ReloadTime > _currentReloadTime)
                    {
                        //Higher - Green Arrow
                        _reloadArrow.enabled = true;
                        _reloadArrow.color = Color.green;
                        _reloadArrow.rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
                    }
                    else if (_lootData.ReloadTime < _currentReloadTime)
                    {
                        //Lower - Red Arrow
                        _reloadArrow.enabled = true;
                        _reloadArrow.color = Color.red;
                        _reloadArrow.rectTransform.localRotation = Quaternion.Euler(0, 0, 180);
                    }
                    else
                    {
                        // Equal - Hide Arrow
                        _reloadArrow.enabled = false;
                    }



                    ////////////////////////////////////////////////
                    m_stat4[0].GetComponent<TextMeshProUGUI>().text = "Accuracy";
                    m_stat4[1].GetComponent<TextMeshProUGUI>().text = DisplayNiceStats(_lootData.Accuracy); //_lootData.Accuracy.ToString();
                    if (_lootData.Accuracy > _currentAccuracy)
                    {
                        //Higher - Green Arrow
                        _accuracyArrow.enabled = true;
                        _accuracyArrow.color = Color.green;
                        _accuracyArrow.rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
                    }
                    else if (_lootData.Accuracy < _currentAccuracy)
                    {
                        //Lower - Red Arrow
                        _accuracyArrow.enabled = true;
                        _accuracyArrow.color = Color.red;
                        _accuracyArrow.rectTransform.localRotation = Quaternion.Euler(0, 0, 180);
                    }
                    else
                    {
                        // Equal - Hide Arrow
                        _accuracyArrow.enabled = false;
                    }
                }
                if (_currentLoot.GetComponent<LootDetection>().LootType == LootDetection.m_lootTypes.Engine)
                {
                    float _currentTopSpeed = 0;
                    float _currentAcceleration = 0;
                    float _currentBoostPower = 0;
                    float _currentHandling = 0;

                    EngineData _lootData = _currentLoot.transform.GetChild(0).GetComponent<EngineGenerator>().engineStatBlock;
                    try
                    {
                        _currentTopSpeed = PlayerInventoryManager.Instance.EquippedEngine.TopSpeed;
                        _currentAcceleration = PlayerInventoryManager.Instance.EquippedEngine.Acceleration;
                        _currentBoostPower = PlayerInventoryManager.Instance.EquippedEngine.BoostPower;
                        _currentHandling = PlayerInventoryManager.Instance.EquippedEngine.Handling;
                    }
                    catch
                    { };
                    

                    m_stat1[0].GetComponent<TextMeshProUGUI>().text = "Top Speed";
                    m_stat2[0].GetComponent<TextMeshProUGUI>().text = "Acceleration";
                    m_stat3[0].GetComponent<TextMeshProUGUI>().text = "Boost Power";
                    m_stat4[0].GetComponent<TextMeshProUGUI>().text = "Handling";

                    m_stat1[1].GetComponent<TextMeshProUGUI>().text = _lootData.TopSpeed.ToString();
                    m_stat2[1].GetComponent<TextMeshProUGUI>().text = _lootData.Acceleration.ToString();
                    m_stat3[1].GetComponent<TextMeshProUGUI>().text = _lootData.BoostPower.ToString();
                    m_stat4[1].GetComponent<TextMeshProUGUI>().text = _lootData.Handling.ToString();

                    Image _topSpeedArrow = m_stat1[2].GetComponent<Image>();
                    Image _accelerationArrow = m_stat2[2].GetComponent<Image>();
                    Image _boostPowerArrow = m_stat3[2].GetComponent<Image>();
                    Image _handlingArrow = m_stat4[2].GetComponent<Image>();

                    m_lootTitle.text = _lootData.Name;

                    ////////////////TOP SPEED ARROW
                    if (_lootData.TopSpeed > _currentTopSpeed)
                    {
                        //Higher - Green Arrow
                        _topSpeedArrow.enabled = true;
                        _topSpeedArrow.color = Color.green;
                        _topSpeedArrow.rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
                    }
                    else if (_lootData.TopSpeed < _currentTopSpeed)
                    {
                        //Lower - Red Arrow
                        _topSpeedArrow.enabled = true;
                        _topSpeedArrow.color = Color.red;
                        _topSpeedArrow.rectTransform.localRotation = Quaternion.Euler(0, 0, 180);
                    }
                    else
                    {
                        // Equal - Hide Arrow
                        _topSpeedArrow.enabled = false;
                    }

                    ////////////////ACCELERATION ARROW
                    if (_lootData.Acceleration > _currentAcceleration)
                    {
                        //Higher - Green Arrow
                        _accelerationArrow.enabled = true;
                        _accelerationArrow.color = Color.green;
                        _accelerationArrow.rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
                    }
                    else if (_lootData.Acceleration < _currentAcceleration)
                    {
                        //Lower - Red Arrow
                        _accelerationArrow.enabled = true;
                        _accelerationArrow.color = Color.red;
                        _accelerationArrow.rectTransform.localRotation = Quaternion.Euler(0, 0, 180);
                    }
                    else
                    {
                        // Equal - Hide Arrow
                        _accelerationArrow.enabled = false;
                    }


                    ////////////////BOOST POWER
                    if (_lootData.BoostPower > _currentBoostPower)
                    {
                        //Higher - Green Arrow
                        _boostPowerArrow.enabled = true;
                        _boostPowerArrow.color = Color.green;
                        _boostPowerArrow.rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
                    }
                    else if (_lootData.BoostPower < _currentBoostPower)
                    {
                        //Lower - Red Arrow
                        _boostPowerArrow.enabled = true;
                        _boostPowerArrow.color = Color.red;
                        _boostPowerArrow.rectTransform.localRotation = Quaternion.Euler(0, 0, 180);
                    }
                    else
                    {
                        // Equal - Hide Arrow
                        _boostPowerArrow.enabled = false;
                    }

                    ////////////////HANDLING
                    if (_lootData.Handling > _currentHandling)
                    {
                        //Higher - Green Arrow
                        _handlingArrow.enabled = true;
                        _handlingArrow.color = Color.green;
                        _handlingArrow.rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
                    }
                    else if (_lootData.Handling < _currentHandling)
                    {
                        //Lower - Red Arrow
                        _handlingArrow.enabled = true;
                        _handlingArrow.color = Color.red;
                        _handlingArrow.rectTransform.localRotation = Quaternion.Euler(0, 0, 180);
                    }
                    else
                    {
                        // Equal - Hide Arrow
                        _handlingArrow.enabled = false;
                    }
                }
            }
            catch { }
        }
    }
    private void DisplayLootTitle(GameObject _currentLoot)
    {
        if (_currentLoot != null)
        {
            if (_currentLoot.GetComponent<LootDetection>().LootType == LootDetection.m_lootTypes.Weapon)
            {
                m_lootItemIcon.sprite = m_weapon;
                m_lootDisplayTitle.text = "WEAPON";
            }
            if (_currentLoot.GetComponent<LootDetection>().LootType == LootDetection.m_lootTypes.Engine)
            {
                m_lootItemIcon.sprite = m_engine;
                m_lootDisplayTitle.text = "ENGINE";
            }
            if (_currentLoot.GetComponent<LootDetection>().LootType == LootDetection.m_lootTypes.Shield)
            {
                m_lootItemIcon.sprite = m_shield;
                m_lootDisplayTitle.text = "SHIELD";
            }
        }
    }      
    private string DisplayNiceStats(float _value)
    {
        string _returnVal;
        float _percentage = _value / 100f;

        float _newValue = 100 + (899 * _percentage);
        _returnVal = ((int)_newValue).ToString();
        return _returnVal;
    }
    private void togglePickup()
    {
        m_enablePickup = true;
    }
    private void toggleQuestPickup()
    {
        m_enableQuestPickup = true;
    }
    #endregion

    #region Quest Becon Detection Methods
    public void DrawBeaconTarget(Vector2 _screenPos, QuestBeconDetection _beacon)
    {
        GameObject _beaconObject;
        Image _targetImage;

        if (_beacon.QuestTarget != null)
        {
            _beaconObject = _beacon.QuestTarget;
            _targetImage = _beaconObject.GetComponent<Image>();
        }
        else
        {
            _beaconObject = new GameObject();
            _beaconObject.name = "BeconTarget";
            _beaconObject.transform.parent = WaypointsAndMarkers;

            //Setting the sprite
            _targetImage = _beaconObject.AddComponent<Image>();
            _targetImage.color = m_questTargetColour;
            _beacon.QuestTarget = _beaconObject;
        }
        //moveit
        _targetImage.rectTransform.localScale = new Vector3(m_questTargetSize, m_questTargetSize, m_questTargetSize);
        _targetImage.sprite = TargetSprite;
        _targetImage.transform.position = _screenPos;
        _targetImage.transform.localEulerAngles = Vector3.zero;

        //If targeted loot has changed, reset LootDisplay.
        if (m_prevTarget != m_currentTarget)
        {
            // Swapping over the LootDisplays to a different object
            m_displayQuestAnimated = false;
            m_currentlyQuestScanning = false;
            QuestDestroyer.fillAmount = 0;
            QuestScanner.fillAmount = 0;
        }
        m_prevTarget = m_currentTarget;

        if (m_currentTarget == null)
        {
            Debug.Log("<color=red> Current beacon is null. </color> Do all loot objects have the component tag?");
        }
        else if (m_currentTarget.tag == "QuestBeacon")
        {
            //Screenpos of loot, and LootDetection script. This will aid in automatically entering values from a piece of loot.
            _screenPos = Camera.WorldToScreenPoint(m_currentTarget.transform.position);
            _beacon = m_currentTarget.GetComponent<QuestBeconDetection>();


            DrawDisplay(_screenPos);
        }

    }

    public void ClearBeaconTarget(QuestBeconDetection _beacon)
    {
        Destroy(_beacon.QuestTarget);
        //Need to make it clear the loot Display if there isn't any loot left on screen.
        if (countTargets(1) == 0 && m_displayQuestAnimated == true)
        {
            ClearBeaconDisplay();
        }
    }

    //Clears the Loot Display
    private void ClearBeaconDisplay()
    {
        Scanner.fillAmount = 0;
        Destroyer.fillAmount = 0;
        m_displayQuestAnimated = false;
        m_currentlyClosingQuestScan = false;
        m_currentlyQuestScanning = false;
        if (m_currentlyQuestScanning == true)
        {
            QuestDisplay.GetComponent<Animator>().Play("QuestCloseFromScanned");
        }
        else if (m_currentlyQuestScanning == false)
        {
            QuestDisplay.GetComponent<Animator>().Play("QuestClosedFromUnscanned");
        }


    }

    private void DisplayBeconTitle(GameObject _currentBeacon)
    {
        if (_currentBeacon != null)
        {
            if (_currentBeacon.GetComponent<QuestBeconDetection>().QuestType == QuestType.kill)
            {
                m_lootItemIcon.sprite = m_weapon;
                m_questDisplayTitle.text = _currentBeacon.GetComponent<QuestBeconDetection>().Quest.Name;
            }
            if (_currentBeacon.GetComponent<QuestBeconDetection>().QuestType == QuestType.control)
            {
                m_lootItemIcon.sprite = m_engine;
                m_questDisplayTitle.text = _currentBeacon.GetComponent<QuestBeconDetection>().Quest.Name;
            }
            if (_currentBeacon.GetComponent<QuestBeconDetection>().QuestType == QuestType.collect)
            {
                m_lootItemIcon.sprite = m_shield;
                m_questDisplayTitle.text = _currentBeacon.GetComponent<QuestBeconDetection>().Quest.Name;
            }
            if (_currentBeacon.GetComponent<QuestBeconDetection>().QuestType == QuestType.recon)
            {
                m_lootItemIcon.sprite = m_shield;
                m_questDisplayTitle.text = _currentBeacon.GetComponent<QuestBeconDetection>().Quest.Name;
            }
            if (_currentBeacon.GetComponent<QuestBeconDetection>().QuestType == QuestType.targets)
            {
                m_lootItemIcon.sprite = m_shield;
                m_questDisplayTitle.text = _currentBeacon.GetComponent<QuestBeconDetection>().Quest.Name;
            }
        }
    }

    private GameObject ReturnTargetBeacon(GameObject[] _visibleBeacons)
    {
        GameObject _target = null;
        float _currentDistance = 0f;
        float _minimumDistance = Mathf.Infinity;
        Vector2 _crosshairPosition = new Vector2(Screen.width / 2, Screen.height / 2);

        foreach (GameObject _objTarget in _visibleBeacons)
        {
            _currentDistance = Vector2.Distance(_crosshairPosition, Camera.WorldToScreenPoint(_objTarget.transform.position));
            if (_currentDistance < _minimumDistance && Vector3.Distance(_objTarget.transform.position, Player.transform.position) < m_lootViewDistance)
            {
                _target = _objTarget;
                _minimumDistance = _currentDistance;
            }
        }

        return _target;
    }

    private void DisplayQuestData(Quest _currentQuest)
    {
        if(_currentQuest != null)
        {
            m_questDescription.text = _currentQuest.Description;
            questReward.text = _currentQuest.RewardName;
        }
    }
    #endregion
    #region QuestDisplay
    private void DisplayActiveQuest()
    {
        try {
            if (QuestManager.Instance.CurrentQuests[QuestManager.Instance.TrackingQuestIndex].Complete == false)
            {
                    m_questTitle.text = QuestManager.Instance.CurrentQuests[QuestManager.Instance.TrackingQuestIndex].Name;
                    m_questDescription.text = QuestManager.Instance.CurrentQuests[QuestManager.Instance.TrackingQuestIndex].Description;
                    m_questProgress.text = QuestManager.Instance.CurrentQuests[QuestManager.Instance.TrackingQuestIndex].CurrentAmountCompleted.ToString() + "/" + QuestManager.Instance.CurrentQuests[QuestManager.Instance.TrackingQuestIndex].Size.ToString();
            }
            else
            {
                m_completedQuestAnimator.Play("QuestCompletedAnim");
                m_questTitle.text = "";
                m_questDescription.text = "";
                m_questProgress.text = "";
                m_completedQuestTitle.text = QuestManager.Instance.CurrentQuests[QuestManager.Instance.TrackingQuestIndex].Name;
                m_completedQuestDescription.text = QuestManager.Instance.CurrentQuests[QuestManager.Instance.TrackingQuestIndex].Description;
                QuestManager.Instance.TrackingQuestIndex += 1;
            }
        } catch { }
    }



    #endregion
    #region DisplayWarningMethods
    public void DisplayWarning(string _warningText)
    {
        if (m_warningObject.activeSelf == true)
        {
            Debug.LogWarning("Warning is already active.");
        }
        else
        {
            m_warningObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _warningText;
            m_warningObject.SetActive(true);
        }
    }

    public void HideWarning()
    {
        m_warningObject.SetActive(false);
    }

    #endregion

    public void playHitmarker()
    {
        m_hitmarkerObject.GetComponent<Animator>().Play("HitmarkerFlash");
    }
    #region Universal Methods
    private bool IsVisibleFrom(Renderer renderer, Camera camera)
    {
        //Creates planes emitting from selected camera to detect if object is visible. Returns true if it is.
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
    private void IncrementPlayerPref(string _name)
    {
        int _value = PlayerPrefs.GetInt(_name);
        PlayerPrefs.SetInt(_name, _value + 1);
    }

    public void ClearAllDisplays()
    {
        int _waypointCount = WaypointsAndMarkers.transform.childCount;

        for(int i=0; i<_waypointCount; i++)
        {
            Destroy(WaypointsAndMarkers.GetChild(i).gameObject);
        }

        if (PlayerInventoryManager.Instance.EquippedEngine == null)
        {
            DisplayWarning("No Engine");
        }
    }
    #endregion
}
