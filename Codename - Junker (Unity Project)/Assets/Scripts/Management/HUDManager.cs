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
    [SerializeField, Tooltip("How quickly the scan and destroy occurs. Higher is faster."), Range(0.2f,1f)]
    private float m_fillSpeed;
    #endregion


    private Canvas m_HUDcanvas;
    private bool m_displayAnimated;
    private bool m_currentlyClosingScan = false;
    private bool m_currentlyScanning = false;
    private bool m_buttonHolding = false;
    private GameObject m_displayDismissed;
    private GameObject m_currentLoot;

    #region Accessors
    public static HUDManager Instance { get => s_instance; set => s_instance = value; }
    public int ViewDistance { get => m_viewDistance; }
    public int LootViewDistance { get => m_lootViewDistance; }
    #endregion

    void Start()
    {
        m_HUDcanvas = GetComponent<Canvas>();
        m_displayAnimated = false;
        m_arrowClampAngle = Mathf.Asin((Screen.height) / Mathf.Sqrt((m_viewDistance * m_viewDistance) + (Screen.height * Screen.height)));
        m_arrowClampAngle = m_arrowClampAngle * Mathf.Rad2Deg;
    }
    void Awake()
    {

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
        if (Input.GetButton("Interact") && LootDisplay.activeInHierarchy == true)
        {
            m_buttonHolding = true;
            Scanner.fillAmount += m_fillSpeed * Time.deltaTime;
        }
        if (Input.GetButtonUp("Interact") && Scanner.fillAmount < 0.1f && LootDisplay.activeInHierarchy == true)
        {
            //Make pickup item code here
            //Gameobject _pickupLoot = m_currentLoot;


            Debug.Log("Yay STUFF");
            Scanner.fillAmount = 0;
        }
        if (Input.GetButtonUp("Interact") && Scanner.fillAmount > 0.1f && LootDisplay.activeInHierarchy == true)
        {
            Scanner.fillAmount = 0;
        }
        if (Scanner.fillAmount == 1 && m_currentlyScanning == false)
        {
            m_currentlyScanning = true;
            LootDisplay.GetComponent<Animator>().Play("DisplayStats");
            Scanner.fillAmount = 0;
        }
        if (Scanner.fillAmount == 1 && m_currentlyScanning == true)
        {
            LootDisplay.GetComponent<Animator>().Play("HideStats");
            m_currentlyClosingScan = true;
            m_currentlyScanning = false;
            Scanner.fillAmount = 0;
        }
        if (Input.GetButton("Dismiss") && LootDisplay.activeInHierarchy == true)
        {
            Destroyer.fillAmount += m_fillSpeed * Time.deltaTime;
        }
        if (Input.GetButtonUp("Dismiss") && LootDisplay.activeInHierarchy == true)
        {
            Destroyer.fillAmount = 0;
        }
        if (Destroyer.fillAmount == 1 && LootDisplay.activeInHierarchy == true)
        {

            //Make it explode in here
            ClearLootTarget(m_currentLoot.GetComponent<LootDetection>());

            Destroy(m_currentLoot);
            if (m_currentlyScanning == true)
            {
                LootDisplay.GetComponent<Animator>().Play("CloseFromScanned");
            }
            else
            {
                LootDisplay.GetComponent<Animator>().Play("CloseFromUnscanned");
            }
            m_displayAnimated = false;
            m_currentlyScanning = false;
            Invoke("DisableLootDisplay", 1.15f);
            Destroyer.fillAmount = 0;
        }


    }


    #region Enemy Detection Methods
    // The argument enemy passed in is the same as the enemy calling the function so that the targets stay encapsulated
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

    }
    // The argument enemy passed in is the same as the enemy calling the function so that the targets stay encapsulated
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
    public void ClearEnemyDetection(EnemyDetection _enemy)
    {
        Destroy(_enemy.EnemyTarget);
    }
    #endregion
    #region Loot Detection Methods
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

        if(m_currentLoot == null)
        {
            m_currentLoot = _loot.gameObject;
        }
        //moveit
        _targetImage.rectTransform.localScale = new Vector3(m_lootTargetSize, m_lootTargetSize, m_lootTargetSize);
        _targetImage.sprite = TargetSprite;
        _targetImage.transform.position = _screenPos;
        _targetImage.transform.localEulerAngles = Vector3.zero;

        _screenPos = Camera.WorldToScreenPoint(m_currentLoot.transform.position);
        DrawLootDisplay(_screenPos,_loot);
    }
    public void ClearLootTarget(LootDetection _loot)
    {
        Destroy(_loot.LootTarget);
        m_displayAnimated = false;
        m_currentlyClosingScan = false;
        Transform[] children = this.GetComponentsInChildren<Transform>();
        foreach (Transform transform in children){
            if (transform.name == "LootTarget")
            {
                return;
            }
        }
        if (m_currentlyScanning == true)
        {
            LootDisplay.GetComponent<Animator>().Play("CloseFromScanned");
        }
        else
        {
            LootDisplay.GetComponent<Animator>().Play("CloseFromUnscanned");
        }
        m_currentlyScanning = false;
    }
    public void DrawLootDisplay(Vector2 _targetPos, LootDetection _loot)
    {
        if (!m_displayAnimated)
        {
            LootDisplay.GetComponent<Animator>().Play("ShowLoot");
            m_displayAnimated = true;
        }
        Vector3 _displayTargetPos;
        if (m_currentlyScanning)
        {
            _displayTargetPos = new Vector3(_targetPos.x, _targetPos.y + m_displayOffset * Screen.height * 2.5f);
            LootDisplay.GetComponent<RectTransform>().position = Vector3.Lerp(LootDisplay.GetComponent<RectTransform>().position, _displayTargetPos, 0.2f);

        }
        else if (m_currentlyClosingScan)
        {
            _displayTargetPos = new Vector3(_targetPos.x, _targetPos.y + m_displayOffset * Screen.height);
            LootDisplay.GetComponent<RectTransform>().position = Vector3.MoveTowards(LootDisplay.GetComponent<RectTransform>().position, _displayTargetPos, 0.000001f * Mathf.Pow(Vector3.Distance(LootDisplay.GetComponent<RectTransform>().position, _displayTargetPos), 3));
            if (Vector3.Distance(LootDisplay.GetComponent<RectTransform>().position, _displayTargetPos) < 20f)
            {
                m_currentlyClosingScan = false;
            }
        }
        else
        {
            _displayTargetPos = new Vector3(_targetPos.x, _targetPos.y + m_displayOffset * Screen.height);
            LootDisplay.GetComponent<RectTransform>().position = Vector3.Lerp(LootDisplay.GetComponent<RectTransform>().position, _displayTargetPos, 0.25f);
        }
    }
    #endregion

}
