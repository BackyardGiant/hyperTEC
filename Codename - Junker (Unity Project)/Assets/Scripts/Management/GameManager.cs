using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager s_instance;

    [SerializeField, Range(0,1), Tooltip("The speed of objects in the game on a scale of 0-1")]
    private float m_gameSpeed;

    public static GameManager Instance { get => s_instance; set => s_instance = value; }
    public float GameSpeed { get => m_gameSpeed; set => m_gameSpeed = value; }

    private void Awake()
    {
        if(s_instance == null)
        {
            s_instance = this;
        }
        else
        {
            Destroy(s_instance.gameObject);
            s_instance = this;
        }
    }
}
