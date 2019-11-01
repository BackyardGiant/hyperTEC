using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUDManager : MonoBehaviour
{
    private static HUDManager s_instance;
    public GameObject Player;
    public Camera Camera;
    public Sprite TargetSprite;

    #region EnemyIndicators
    [Header ("Enemy Indicator System")]
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
    [Header("Loot Indicator System")]
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
    #endregion

    #region AutoAim
    private GameObject m_closetEnemy;
    private Vector2 m_closetEnemyScreenPos;
    #endregion


    private bool m_displayAnimated;
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
        }
        //If the player lets go of the button while it's below 0.1f fill amount, the player has picked the item up.
        if (Input.GetButtonUp("Interact") && Scanner.fillAmount < 0.1f && m_displayAnimated == true)
        {
            //Make pickup item code here
            //Gameobject _pickupLoot = m_currentLoot;


            Debug.Log("Pickup Loot.");
            Scanner.fillAmount = 0;
        }

        //If player lets go of the button and it's above 0.1f fill amount, clear the progress.
        if (Input.GetButtonUp("Interact") && Scanner.fillAmount > 0.1f && m_displayAnimated == true)
        {
            Scanner.fillAmount = 0;
        }

        //Full scan complete, open and animate display.
        if (Scanner.fillAmount == 1 && m_currentlyScanning == false)
        {
            Scanner.fillAmount = 0;
            LootDisplay.GetComponent<Animator>().Play("DisplayStats");
            m_currentlyScanning = true;
        }
        
        //Full close complete, close and animate display.
        if (Scanner.fillAmount == 1 && m_currentlyScanning == true)
        {
            Scanner.fillAmount = 0;
            LootDisplay.GetComponent<Animator>().Play("HideStats");
            m_currentlyClosingScan = true;
            m_currentlyScanning = false;
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
            ClearLootTarget(m_currentLoot.GetComponent<LootDetection>());
            Destroy(m_currentLoot);
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
        _targetImage.rectTransform.localScale = new Vector3(m_enemyTargetSize, m_enemyTargetSize, m_enemyTargetSize);
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
            if (IsVisibleFrom(_lootObjects[i].GetComponent<Renderer>(), Camera))
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
    #endregion

    private bool IsVisibleFrom(Renderer renderer, Camera camera)
    {
        //Creates planes emitting from selected camera to detect if object is visible. Returns true if it is.
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}
