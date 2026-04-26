using System.Collections;
using TMPro;
using UnityEngine;

public class FPSUI : MonoBehaviour
{
    public static FPSUI Instance;

    private TMP_Text health;
    private TMP_Text ammo;
    private TMP_Text healthKits;
    private Transform messageSpawnPoint;
    [SerializeField] private GameObject messagePrefab;

    private PlayerShooting playerShooting;
    private Health playerHealth;
    private PlayerController playerController;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        health = transform.Find("Health").GetComponent<TMP_Text>();
        ammo = transform.Find("Ammo").GetComponent<TMP_Text>();
        healthKits = transform.Find("HealthKits").GetComponent<TMP_Text>();
        messageSpawnPoint = transform.Find("MessageSpawn").transform;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerShooting = player.GetComponent<PlayerShooting>();
        playerController = player.GetComponent<PlayerController>();
        playerHealth = player.GetComponent<Health>();

        StartCoroutine(DelayMessage("Press F to toggle flashlight", 18f));
    }

    void Update()
    {
        health.text = "" + playerHealth.GetHealth();

        GunController gun = playerShooting.gun;
        if (gun != null) { 
            GunState gunState = gun.gunState;
            ammo.text = gunState.currentMag + " / " + gunState.currentAmmo;
        }else{
            ammo.text = "0/0";
        }

        healthKits.text = "Health Kits: " + playerController.healthPacks;
    }

    public void ShowMessage(string message, float duration = 10f)
    {
        GameObject messageObj = Instantiate(messagePrefab, messageSpawnPoint);
        messageObj.transform.localPosition = Vector3.zero;
        messageObj.transform.localRotation = Quaternion.identity;
        TMP_Text textComponent = messageObj.GetComponent<TMP_Text>();
        textComponent.text = message;
        Destroy(messageObj, duration);
    }

    public IEnumerator DelayMessage(string message, float delay, float duration = 10f)
    {
        yield return new WaitForSeconds(delay);
        ShowMessage(message, duration);
    }
}
