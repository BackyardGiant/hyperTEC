using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField]
    private int m_environmentSize;

    [SerializeField]
    private int m_asteroidCount;

    [SerializeField,Space(20)]
    private asteroidClass[] m_asteroidTypes;

    private List<GameObject> asteroids = new List<GameObject>();


    public void generate()
    {
        clear();
        Vector3 randomPosition;
        Vector3 randomRotation;
        GameObject asteroidObj;
        for (int i = 0; i < m_asteroidCount; i++)
        {
            //Choose a random asteroid to spawn.
            asteroidClass asteroid = m_asteroidTypes[Random.Range(0, m_asteroidTypes.Length)];
            asteroidObj = asteroid.asteroidObject;

            //Chance roll on the random asteroid, if false, try again.
            if (chanceRoll(asteroid.rarity))
            {
                randomPosition = Random.insideUnitSphere * m_environmentSize;
                float _randomX = Random.Range(0f, 360f);
                float _randomY = Random.Range(0f, 360f);
                float _randomZ = Random.Range(0f, 360f);

                GameObject obj = Instantiate(asteroidObj);
                obj.transform.position = randomPosition;
                obj.transform.rotation = new Quaternion(_randomX, _randomY, _randomZ, 1);
                obj.transform.parent = this.transform;
                asteroids.Add(obj);
            }
            else
            {
                i = i - 1;
            }
        }
    }

    public void clear()
    {
        for(int i=0; i < transform.childCount; i++)
        {
            GameObject obj = transform.GetChild(i).gameObject;
            DestroyImmediate(obj);
        }
        asteroids = new List<GameObject>();

    }


    private bool chanceRoll(float _chance)
    {
        float _random = Random.Range(0.0f, 1.0f);
        if (_random < _chance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
