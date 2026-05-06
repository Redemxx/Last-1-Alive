using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> zombies = new List<GameObject>();
    [SerializeField] private int[] weights;
    [SerializeField] private int initialZombiesToSpawn = 4;
    [SerializeField] private float waveCooldownMin = 15f;
    [SerializeField] private float waveCooldownMax = 45f;
    [SerializeField] private float spawnCooldownMin = 3f;
    [SerializeField] private float spawnCooldownMax = 20f;
    [SerializeField] private bool ignoreRay = false;
    [SerializeField] private LayerMask playerMask;

    private List<Transform> spawnPoints = new List<Transform>();
    private int wave = 0;
    private int zombiesToSpawn;
    private List<GameObject> players = new List<GameObject>();
    private float waveCooldown = 0;
    private float spawnCooldown = 0;
    private int totalWeight = 0;

    void Start()
    {
        foreach (int weight in weights)
        {
            totalWeight += weight;
        }

        Transform spawnPointsParent = transform.Find("SpawnPoints");
        zombiesToSpawn = initialZombiesToSpawn;

        foreach (Transform t in spawnPointsParent) {
            spawnPoints.Add(t);
        }

        Transform displayObj = transform.Find("Display");
        if (displayObj != null) {
            Destroy(displayObj.gameObject);
        }
    }

    void Update()
    {
        if (players.Count == 0) return;
        if (Time.time < waveCooldown) return;
        if (Time.time < spawnCooldown) return;

        int spawnPointIndex = Random.Range(0, spawnPoints.Count);
        int zombieIndex = GetWeightedRandomIndex();

        if (!ignoreRay && CheckLineOfSight(spawnPointIndex)) return;

        GameObject zombie = Instantiate(zombies[zombieIndex], spawnPoints[spawnPointIndex].position, Quaternion.identity);
        StartCoroutine(zombie.GetComponent<NormalZombieController>().Engage(players[Random.Range(0, players.Count)]));
        zombiesToSpawn -= 1;
        spawnCooldown = Time.time + Random.Range(spawnCooldownMin, spawnCooldownMax);

        if (zombiesToSpawn <= 0) {
            StartNewWave();
        }
    }

    void StartNewWave()
    {
        waveCooldown = Time.time + Random.Range(waveCooldownMin, waveCooldownMax);
        wave += 1;
        zombiesToSpawn = (wave * wave / 2) + 4;
    }

    bool CheckLineOfSight(int spawnPointIndex)
    {
        foreach (GameObject player in players) {
            Vector3 diff = player.transform.position - spawnPoints[spawnPointIndex].position + Vector3.up * 0.5f;
            Ray ray = new Ray(spawnPoints[spawnPointIndex].position, diff);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, playerMask)) {
                if (hit.collider.gameObject == player) {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player entered spawner range");
        players.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Player left spawner range");
        players.Remove(other.gameObject);
    }

    private int GetWeightedRandomIndex()
    {
        int randomValue = Random.Range(0, totalWeight);
        for (int i = 0; i < weights.Length; i++)
        {
            if (randomValue < weights[i])
            {
                return i;
            }
            randomValue -= weights[i];
        }

        return zombies.Count - 1;
    }
}
