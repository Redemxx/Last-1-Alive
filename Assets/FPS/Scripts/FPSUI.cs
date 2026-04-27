using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private GameObject playerUI;
    private GameObject winScreen;
    private GameObject deathScreen;

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
        deathScreen = transform.Find("Death").gameObject;
        winScreen = transform.Find("Win").gameObject;
        playerUI = transform.Find("UI").gameObject;
        health = playerUI.transform.Find("Health").GetComponent<TMP_Text>();
        ammo = playerUI.transform.Find("Ammo").GetComponent<TMP_Text>();
        healthKits = playerUI.transform.Find("HealthKits").GetComponent<TMP_Text>();
        messageSpawnPoint = playerUI.transform.Find("MessageSpawn").transform;
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

    public void ChangeMenu(int menu)
    {
        switch (menu)
        {
            case 0:
                playerUI.SetActive(true);
                winScreen.SetActive(false);
                deathScreen.SetActive(false);
                break;
            case 1:
                playerUI.SetActive(false);
                winScreen.SetActive(true);
                deathScreen.SetActive(false);
                break;
            case 2:
                playerUI.SetActive(false);
                winScreen.SetActive(false);
                deathScreen.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void ResumeGame()
    {
        ChangeMenu(0);
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        ChangeMenu(1);
        Time.timeScale = 0f;
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        Destroy(gameObject);
        SceneManager.LoadScene("Main Menu");
    }
}
