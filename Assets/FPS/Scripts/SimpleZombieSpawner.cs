using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SimpleZombieSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] zombies;
    [SerializeField] private int[] weights;
    [SerializeField] private LayerMask spawnSurface;
    [SerializeField] private LayerMask avoidMask;
    [SerializeField] private int minSpawn;
    [SerializeField] private int maxSpawn;
    private float radius;
    private int totalWeight;

    void Start()
    {
        foreach (int weight in weights)
        {
            totalWeight += weight;
        }

        SphereCollider sc = gameObject.GetComponent<SphereCollider>();
        radius = sc.radius;
        int spawnCount = Random.Range(minSpawn, maxSpawn + 1);

        int tryCount = 0;
        for (float i = 0; i < spawnCount; i++) {
            tryCount++;
            if (tryCount > spawnCount * 100) {
                break;
            }

            int zombieIndex = GetWeightedRandomIndex();
            Vector3 randomPos = transform.position + Random.insideUnitSphere * radius;
            randomPos.y = transform.position.y + Random.Range(0, radius);

            if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, radius * 2, spawnSurface))
            {
                randomPos.y = hit.point.y;
            }
            else
            {
                i--;
                continue;
            }

            if (Physics.CheckSphere(randomPos, 0.1f, avoidMask))
            {
                i--;
                continue;
            }

            Instantiate(zombies[zombieIndex], randomPos, Quaternion.identity);
        }
        Destroy(gameObject);
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

        return weights.Length - 1;
    }
}