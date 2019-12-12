using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TerrainGenerator : MonoBehaviour
{
    public int usableEnvironmentSize;

    [Header ("Environment Variables"), Tooltip("The range at which clusters will generate.")]
    [SerializeField] private int m_environmentSize;
    [SerializeField] private int m_ambientAsteroids;

    [Header ("Asteroid Cluster Variables"),Space(20)]
    [SerializeField] private int m_clusterCount;
    [SerializeField] private int m_asteroidsPerCluster;
    [SerializeField, Tooltip("The range at which asteroids will spawn within the cluster.")] private int m_clusterSize;

    [SerializeField,Header ("Asteroid Types"),Space(20),Tooltip("All of the different types of asteroid.")]
    private asteroidClass[] m_asteroidTypes;

    private List<GameObject> asteroids = new List<GameObject>();


    public void generate()
    {
        clear();

        Vector3 _clusterLocation;
        Vector3 _randomLocation;

        for (int i = 0; i < m_ambientAsteroids; i++)
        {
            _randomLocation = (Random.insideUnitSphere * m_environmentSize);
            spawnAsteroid(_randomLocation);
        }
   
        for (int i = 0; i < m_clusterCount; i++)
        {
            //Spawn a Cluster at a randomPosition
            _clusterLocation = Random.insideUnitSphere * m_environmentSize;
            m_asteroidsPerCluster = (int)Random.Range(m_asteroidsPerCluster * 0.8f, m_asteroidsPerCluster * 1.2f);
            for (int j = 0; j < m_asteroidsPerCluster; j++)
            {
                _randomLocation = (Random.insideUnitSphere * m_clusterSize) + _clusterLocation;
                spawnAsteroid(_randomLocation);
            }
        }
    }

    public void clear()
    {
        for(int i=0; i < this.transform.childCount; i++)
        {
            GameObject obj = this.transform.GetChild(i).gameObject;
            DestroyImmediate(obj);
        }
        foreach(GameObject obj in asteroids)
        {
            DestroyImmediate(obj);
        }
        asteroids = new List<GameObject>();

    }
    private void spawnAsteroid(Vector3 _position)
    {
        while (true)
        {
            //Choose a random asteroid to spawn.
            asteroidClass asteroid = m_asteroidTypes[Random.Range(0, m_asteroidTypes.Length)];
            GameObject asteroidObj = asteroid.asteroidObject;

            if (chanceRoll(asteroid.rarity))
            {
                float _randomX = Random.Range(0f, 360f);
                float _randomY = Random.Range(0f, 360f);
                float _randomZ = Random.Range(0f, 360f);

                GameObject obj = Instantiate(asteroidObj);
                obj.transform.position = _position;
                obj.transform.rotation = new Quaternion(_randomX, _randomY, _randomZ, 1);
                obj.transform.parent = this.transform;
                asteroids.Add(obj);
                break;
            }
        }
    }

    #region Helpful Methods

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.zero, usableEnvironmentSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, m_environmentSize);
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
    #endregion
}
