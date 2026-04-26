using UnityEngine;

public class GunSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] guns;
    void Start()
    {
        Transform spawnPoint = transform.GetChild(0);
        int gunIndex = Random.Range(0, guns.Length);
        Instantiate(guns[gunIndex], spawnPoint.position, spawnPoint.rotation);
    }
}
