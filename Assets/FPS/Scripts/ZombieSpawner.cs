using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> zombies = new List<GameObject>();

    private List<Transform> spawnPoints = new List<Transform>();
    private int wave = 0;
    private int zombiesToSpawn = 4;
    private List<GameObject> players = new List<GameObject>();
    private float cooldown = 0;

    void Start()
    {
        GameObject spawnPointsParent = GameObject.Find("SpawnPoints");

        foreach (Transform t in spawnPointsParent.transform) {
            spawnPoints.Add(t);
        }
    }

    void Update()
    {
        if (players.Count == 0) return;
        if (Time.time < cooldown) return;

        for (int i = 0; i < zombiesToSpawn; i++) {
            int spawnPointIndex = Random.Range(0, spawnPoints.Count);
            int zombieIndex = Random.Range(0, zombies.Count);
            Vector3 randomness = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            GameObject zombie = Instantiate(zombies[zombieIndex], spawnPoints[spawnPointIndex].position + randomness, Quaternion.identity);
            StartCoroutine(zombie.GetComponent<NormalZombieController>().Engage(players[Random.Range(0, players.Count)]));
        }

        cooldown = Time.time + Random.Range(20, 60);
        wave += 1;
        zombiesToSpawn = (wave * wave / 3) + 4;
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
