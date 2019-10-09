using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUDManager : MonoBehaviour
{
    private static HUDManager s_instance;


    private Canvas m_HUDcanvas;

    public GameObject player;

    [Header ("Enemy Indicator System")]
    public Sprite enemyTarget;
    public Sprite enemyArrowPointer;
    [SerializeField, Tooltip("Colour of the enemy indicators")]
    private Color m_enemyTargetColour;
    [SerializeField, Tooltip("Scale of the enemy indicators")]
    private float m_enemyTargetSize;
    [SerializeField, Tooltip("Scale of the Arrow indicators")]
    private float m_enemyArrowSize;
    [SerializeField, Tooltip("Angle from the player that the enemy has to be for the arrow to dissapear. 29 is the preferred value.")]
    private float m_arrowClampAngle;

    public static HUDManager Instance { get => s_instance; set => s_instance = value; }

    private void Start()
    {
        m_HUDcanvas = GetComponent<Canvas>();
    }
    private void Awake()
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

    // The argument enemy passed in is the same as the enemy calling the function so that the targets stay encapsulated
    public void DrawEnemyTarget(Vector2 _screenPos, EnemyDetection _enemy)
    {
        GameObject _target;
        Image _targetImage;

        if (_enemy.Target != null)
        {
            _target = _enemy.Target;
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
            _enemy.Target = _target;
        }

        //mooveit
        _targetImage.rectTransform.localScale = new Vector3(m_enemyTargetSize, m_enemyTargetSize, m_enemyTargetSize);
        _targetImage.sprite = enemyTarget;
        _targetImage.transform.position = _screenPos;
        _targetImage.transform.localEulerAngles = Vector3.zero;

    }
    // The argument enemy passed in is the same as the enemy calling the function so that the targets stay encapsulated
    public void DrawEnemyArrow(Vector3 _screenPos, EnemyDetection _enemy)
    {
        GameObject _target;
        Image _targetImage;
        //Draw arrow towards enemy if they're off screen and within range.
        if (_enemy.Target != null)
        {
            _target = _enemy.Target;
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
            _enemy.Target = _target;
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


        Vector3 _enemyToPlayer = _enemy.transform.position - player.transform.position;
        float _angleBehind = Vector3.Angle(_enemyToPlayer, -player.transform.forward);

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
        Destroy(_enemy.Target);
    }
}
