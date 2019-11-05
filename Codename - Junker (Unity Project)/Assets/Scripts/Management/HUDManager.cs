﻿using System.Collections;
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

    public Inventory playerInv;

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
    [SerializeField, Tooltip("How far away from the target that the loot display appears"),Range(0f,1f)]
    private float m_displayOffset = 0.1f;
    [SerializeField, Tooltip("How quickly the scan occurs. Higher is faster."), Range(0.2f,1f)]
    private float m_scanningFillSpeed;
    [SerializeField, Tooltip("How quickly the destroy occurs. Higher is faster."), Range(0.2f, 1f)]
    private float m_destroyFillSpeed;
    [SerializeField,Header("Loot Display Elements"),Tooltip("Display elements of the Loot Display. This will show the stats of the visible loot item."),Space(20)]
    private TextMeshProUGUI m_weaponTitle;
    [SerializeField, Tooltip("Each stat item. Needs the value first then the arrow second.")]
    private GameObject[] m_damage, m_fireRate, m_reloadTime, m_accuracy;
    #endregion

    #region AutoAim
    private GameObject m_closetEnemy;
    private Vector2 m_closetEnemyScreenPos;
    #endregion


    private bool m_displayAnimated;
    private bool m_enablePickup;
    private bool m_currentlyClosingScan = false;
    private bool m_currentlyScanning = false;
    private GameObject m_displayDismissed;
    private GameObject m_currentLoot;
    private GameObject m_prevLoot;
    private Vector2 m_crosshairPosition;

    #region Accessors
    public static HUDManager Instance { get => s_instance; set => s_instance = value; }
    public int ViewDistance { get => m_viewDistance; }
    public int LootViewDistance { get => m_lootViewDistance; }
    public GameObject ClosetEnemy { get => m_closetEnemy; set => m_closetEnemy = value; }
    public Vector2 ClosetEnemyScreenPos { get => m_closetEnemyScreenPos; set => m_closetEnemyScreenPos = value; }
    #endregion

    void Start()
    {
        m_enablePickup = true;
        //Calculate Clamp angle of arrow. This is equal to the angle at which the enemy is to the behind of the player. Working out this value prevents arrows floating around the screen.
        m_displayAnimated = false;
        m_arrowClampAngle = Mathf.Asin((Screen.height) / Mathf.Sqrt((m_viewDistance * m_viewDistance) + (Screen.height * Screen.height)));
        m_arrowClampAngle = m_arrowClampAngle * Mathf.Rad2Deg;
        m_crosshairPosition = new Vector2(Screen.width / 2, Screen.height / 2);
        Destroyer.fillAmount = 0;
        Scanner.fillAmount = 0;
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
        //If player presses button, and Loot Display is active, start filling in the scan button.
        if (Input.GetButton("Interact") && LootDisplay.activeInHierarchy == true)
        {
            Scanner.fillAmount += m_scanningFillSpeed * Time.deltaTime;
            DisplayLootStats(m_currentLoot);
        }
        //If the player lets go of the button while it's below 0.1f fill amount, the player has picked the item up.
        if (Input.GetButtonUp("Interact") && Scanner.fillAmount < 0.1f && m_displayAnimated == true && m_enablePickup == true)
        {
            //Make pickup item code here
            GameObject _pickupLoot = m_currentLoot;
            
            if(m_currentLoot.GetComponent<LootDetection>().LootType == LootDetection.m_lootTypes.Weapon)
            {
                playerInv.AvailableWeapons.Add(m_currentLoot.transform.GetChild(0).GetComponent<WeaponGenerator>().statBlock);
            }

            IncrementPlayerPref("WeaponsCollected");
            Debug.Log("Pickup Loot.");
            Scanner.fillAmount = 0;

            Destroy(m_currentLoot);
            ClearLootDisplay();
            ClearLootTarget(m_currentLoot.GetComponent<LootDetection>());
        }

        //If player lets go of the button and it's above 0.1f fill amount, clear the progress.
        if (Input.GetButtonUp("Interact") && Scanner.fillAmount > 0.1f && m_displayAnimated == true)
        {
            Scanner.fillAmount = 0;
        }

        //Full scan complete, open and animate display.
        if (Scanner.fillAmount == 1 && m_currentlyScanning == false)
        {
            m_enablePickup = false;

            IncrementPlayerPref("WeaponsScanned");
            Scanner.fillAmount = 0;
            LootDisplay.GetComponent<Animator>().Play("DisplayStats");
            m_currentlyScanning = true;

            Invoke("togglePickup", .2f);
        }
        
        //Full close complete, close and animate display.
        if (Scanner.fillAmount == 1 && m_currentlyScanning == true)
        {
            m_enablePickup = false;

            Scanner.fillAmount = 0;
            LootDisplay.GetComponent<Animator>().Play("HideStats");
            m_currentlyClosingScan = true;
            m_currentlyScanning = false;

            Invoke("togglePickup", .2f);
        }

        //Dismiss being held down fills in the button.
        if (Input.GetButton("Dismiss") && LootDisplay.activeInHierarchy == true)
        {
            Destroyer.fillAmount += m_destroyFillSpeed * Time.deltaTime;
        }

        //Letting go of the Dismiss button clears the progress
        if (Input.GetButtonUp("Dismiss") && LootDisplay.activeInHierarchy == true)
        {
            Destroyer.fillAmount = 0;
        }

        //Destroy the target object.
        if (Destroyer.fillAmount == 1 && LootDisplay.activeInHierarchy == true)
        {
            //Make it explode in here
            Destroyer.fillAmount = 0;
            GameObject explosion = Instantiate(Explosion,m_currentLoot.transform.position,m_currentLoot.transform.rotation);
            explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            AudioManager.Instance.PlayWorld("ExplosionShort3", m_currentLoot.gameObject, true, false);
            ClearLootTarget(m_currentLoot.GetComponent<LootDetection>());
            Destroy(m_currentLoot);
            IncrementPlayerPref("WeaponsDestroyed");
            ClearLootDisplay();

        }
        #endregion
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
            _target.transform.parent = this.transform;

            //Setting the sprite
            _targetImage = _target.AddComponent<Image>();
            _targetImage.color = m_enemyTargetColour;
            _enemy.EnemyTarget = _target;
        }

        //moveit
        float _distancePercentage =  1 - Vector3.Distance(Player.transform.position, _enemy.GetComponent<Transform>().position) / m_viewDistance;
        float _finalSize = 0.05f + _distancePercentage * m_enemyTargetSize ;





        _targetImage.rectTransform.localScale = new Vector3(_finalSize,_finalSize,_finalSize);




        _targetImage.sprite =TargetSprite;
        _targetImage.transform.position = _screenPos;
        _targetImage.transform.localEulerAngles = Vector3.zero;

        if(m_closetEnemy == null || Vector2.Distance(_screenPos, m_crosshairPosition) < Vector2.Distance(m_closetEnemyScreenPos, m_crosshairPosition))
        {
            m_closetEnemy = _enemy.gameObject;
            m_closetEnemyScreenPos = _screenPos;
        }

        if (m_closetEnemy == _enemy.gameObject)
        {
            m_closetEnemyScreenPos = _screenPos;
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
            _target.transform.parent = this.transform;

            //Setting the sprite
            _targetImage = _target.AddComponent<Image>();
            _targetImage.color = m_enemyTargetColour;
            _enemy.EnemyTarget = _target;
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
        m_closetEnemy = null;
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
            _lootObject.transform.parent = this.transform;

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
        m_currentLoot = ReturnTargetLoot(_lootObjects);

        //If targeted loot has changed, reset LootDisplay.
        if(m_prevLoot != m_currentLoot)
        {
            // Swapping over the LootDisplays to a different object
            m_displayAnimated = false;
            m_currentlyScanning = false;
            Destroyer.fillAmount = 0;
            Scanner.fillAmount = 0;
        }
        m_prevLoot = m_currentLoot;

        if (m_currentLoot == null)
        {
            Debug.Log("<color=red> Current Loot is null. </color> Do all loot objects have the component tag?");
        }

        //Screenpos of loot, and LootDetection script. This will aid in automatically entering values from a piece of loot.
        _screenPos = Camera.WorldToScreenPoint(m_currentLoot.transform.position);
        _loot = m_currentLoot.GetComponent<LootDetection>();

        DrawLootDisplay(_screenPos,_loot);

    }
    //Clear the target for a loot. Also close/hide the Loot Display.
    public void ClearLootTarget(LootDetection _loot)
    {
        Destroy(_loot.LootTarget);
        //Need to make it clear the loot Display if there isn't any loot left on screen.
        if (countTargets() == 0)
        {
            ClearLootDisplay();
        }
    }
    //Clears the Loot Display
    private void ClearLootDisplay()
    {
        Scanner.fillAmount = 0;
        Destroyer.fillAmount = 0;
        if (m_currentlyScanning == true)
        {
            LootDisplay.GetComponent<Animator>().Play("CloseFromScanned");
            m_displayAnimated = false;
            m_currentlyClosingScan = false;
            m_currentlyScanning = false;
        }
        else
        {
            LootDisplay.GetComponent<Animator>().Play("CloseFromUnscanned");
            m_displayAnimated = false;
            m_currentlyClosingScan = false;
            m_currentlyScanning = false;
        }
    }
    //Returns the loot which is closest to the crosshair. This acts as automatic targetting of loot.
    private GameObject ReturnTargetLoot(GameObject[] _visibleLoot)
    {
        GameObject _target = null;
        float _currentDistance = 0f;
        float _minimumDistance = Mathf.Infinity;
        Vector2 _crosshairPosition = new Vector2(Screen.width / 2,Screen.height / 2);

        foreach (GameObject _objTarget in _visibleLoot)
        {
            _currentDistance = Vector2.Distance(_crosshairPosition, Camera.WorldToScreenPoint(_objTarget.transform.position));
            if (_currentDistance <_minimumDistance)
            {
                _target = _objTarget;
                _minimumDistance = _currentDistance;
            }
        }

        return _target;
    }
    private int countTargets()
    {
        int _targetCount = 0;
        GameObject[] _lootObjects = GameObject.FindGameObjectsWithTag("Component");
        List<GameObject> _visibleLootObjects = new List<GameObject>();

        for(int i = 0; i < _lootObjects.Length; i++)
        {
            float _distance = Vector3.Distance(Player.transform.position, _lootObjects[i].transform.position);
            if (IsVisibleFrom(_lootObjects[i].GetComponent<Renderer>(), Camera) && _distance < m_lootViewDistance)
            {
                _visibleLootObjects.Add(_lootObjects[i]);
            }
        }
        _targetCount = _visibleLootObjects.Count;
        return _targetCount;
    }
    //Draws the lootdisplay with appropriate offset based on the current function.
    private void DrawLootDisplay(Vector2 _targetPos, LootDetection _loot)
    {
        //Check player is going slow enough to look at it.
        Vector3 _displayTargetPos;
        if (Player.GetComponent<PlayerMovement>().CurrentSpeed/Player.GetComponent<PlayerMovement>().MaxAcceleration < 0.5)
        {
            if (!m_displayAnimated)
            {
                LootDisplay.GetComponent<Animator>().Play("ShowLoot");
                m_displayAnimated = true;
            }
            if (m_currentlyScanning)
            {
                _displayTargetPos = new Vector3(_targetPos.x, _targetPos.y + m_displayOffset * Screen.height * 2.5f);
                LootDisplay.GetComponent<RectTransform>().position = Vector3.Lerp(LootDisplay.GetComponent<RectTransform>().position, _displayTargetPos, 0.2f);

            }
        }
        if (m_currentlyClosingScan)
        {
            _displayTargetPos = new Vector3(_targetPos.x, _targetPos.y + m_displayOffset * Screen.height);
            LootDisplay.GetComponent<RectTransform>().position = Vector3.MoveTowards(LootDisplay.GetComponent<RectTransform>().position, _displayTargetPos, 0.00001f * Mathf.Pow(Vector3.Distance(LootDisplay.GetComponent<RectTransform>().position, _displayTargetPos), 3));
            if (Vector3.Distance(LootDisplay.GetComponent<RectTransform>().position, _displayTargetPos) < 40)
            {
                m_currentlyClosingScan = false;
            }
        }
        else
        {
            _displayTargetPos = new Vector3(_targetPos.x, _targetPos.y + m_displayOffset * Screen.height);
            LootDisplay.GetComponent<RectTransform>().position = Vector3.Lerp(LootDisplay.GetComponent<RectTransform>().position, _displayTargetPos, 0.2f);
        }
    }
    private void DisplayLootStats(GameObject _currentLoot)
    {
        try
        {
            WeaponData _lootData = _currentLoot.transform.GetChild(0).GetComponent<WeaponGenerator>().statBlock;
            float _currentDamage;
            float _currentFireRate;
            float _currentReloadTime;
            float _currentAccuracy;

            if (playerInv.EquippedLeftWeapon.Damage < playerInv.EquippedRightWeapon.Damage){_currentDamage = playerInv.EquippedLeftWeapon.Damage;}
            else {_currentDamage = playerInv.EquippedRightWeapon.Damage;}

            if (playerInv.EquippedLeftWeapon.FireRate < playerInv.EquippedRightWeapon.FireRate) { _currentFireRate = playerInv.EquippedLeftWeapon.FireRate; }
            else { _currentFireRate = playerInv.EquippedRightWeapon.FireRate; }

            if (playerInv.EquippedLeftWeapon.ReloadTime < playerInv.EquippedRightWeapon.ReloadTime) { _currentReloadTime = playerInv.EquippedLeftWeapon.ReloadTime; }
            else { _currentReloadTime = playerInv.EquippedRightWeapon.ReloadTime; }

            if (playerInv.EquippedLeftWeapon.Accuracy < playerInv.EquippedRightWeapon.Accuracy) { _currentAccuracy = playerInv.EquippedLeftWeapon.Accuracy; }
            else { _currentAccuracy = playerInv.EquippedRightWeapon.Accuracy; }


            m_weaponTitle.text = _lootData.Name;


            Image _damageArrow = m_damage[1].GetComponent<Image>();
            Image _fireRateArrow = m_fireRate[1].GetComponent<Image>();
            Image _reloadArrow = m_reloadTime[1].GetComponent<Image>();
            Image _accuracyArrow = m_accuracy[1].GetComponent<Image>();

            ///////////////////////////////////////////////
            m_damage[0].GetComponent<TextMeshProUGUI>().text = _lootData.Damage.ToString();
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
                _damageArrow.rectTransform.localRotation = Quaternion.Euler(0,0,180);
            }
            else
            {
                // Equal - Hide Arrow
                _damageArrow.enabled = false;
            }

            //////////////////////////////////////////////
            m_fireRate[0].GetComponent<TextMeshProUGUI>().text = _lootData.FireRate.ToString();
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
            m_reloadTime[0].GetComponent<TextMeshProUGUI>().text = _lootData.ReloadTime.ToString();
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
            m_accuracy[0].GetComponent<TextMeshProUGUI>().text = _lootData.Accuracy.ToString();
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
        catch { Debug.LogError("Error with displaying loot stats."); }
    }
    private void togglePickup()
    {
        m_enablePickup = true;
    }
    #endregion


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
    #endregion
}
