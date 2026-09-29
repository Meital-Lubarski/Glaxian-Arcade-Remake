using UnityEngine;
using System.Collections;

public class StarSpawner : MonoBehaviour
{
    public static StarSpawner Instance { get; private set; }
    
    
    [SerializeField] private GameObject _starPrefab;
    [SerializeField] private float spawnInterval = 4f;
    [SerializeField] private float spawnY = -1;
    [SerializeField] private float spawnRangeX = 8f;
    
    [SerializeField] private GameObject _bonusStarPrefab;
    [SerializeField] private float bonusSpawnChance = 0.2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(0.1f);

        while (true)
        {
            if (SessionManager.Instance == null || SessionManager.Instance.GetCurrentState() != GameState.Playing)
            {
                yield return new WaitForSeconds(0.5f); 
                continue;
            }

            yield return new WaitForSeconds(spawnInterval);

            if (SessionManager.Instance != null && SessionManager.Instance.GetCurrentState() == GameState.Playing)
            {
                SpawnStar();
            }
        }
    }

    public void SpawnStar()
    {
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        Vector3 spawnPos = new Vector3(randomX, spawnY, 0);
        GameObject prefabToSpawn = (Random.value < bonusSpawnChance) ? _bonusStarPrefab : _starPrefab;
        Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
    }
}
