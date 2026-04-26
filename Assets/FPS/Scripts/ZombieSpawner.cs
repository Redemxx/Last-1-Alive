using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> zombies = new List<GameObject>();
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

    void Start()
    {
        Transform spawnPointsParent = transform.Find("SpawnPoints");
        zombiesToSpawn = initialZombiesToSpawn;

        foreach (Transform t in spawnPointsParent) {
            spawnPoints.Add(t);
        }
    }

    void Update()
    {
        if (players.Count == 0) return;
        if (Time.time < waveCooldown) return;
        if (Time.time < spawnCooldown) return;

        int spawnPointIndex = Random.Range(0, spawnPoints.Count);
        int zombieIndex = Random.Range(0, zombies.Count);

        if (!ignoreRay && CheckLineOfSight(spawnPointIndex)) return;

        Vector3 randomness = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        GameObject zombie = Instantiate(zombies[zombieIndex], spawnPoints[spawnPointIndex].position + randomness, Quaternion.identity);
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
}
