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
    private GameObject pauseMenu;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        deathScreen = transform.Find("Death").gameObject;
        winScreen = transform.Find("Win").gameObject;
        playerUI = transform.Find("UI").gameObject;
        pauseMenu = transform.Find("Pause").gameObject;
        health = playerUI.transform.Find("Health").GetComponent<TMP_Text>();
        ammo = playerUI.transform.Find("Ammo").GetComponent<TMP_Text>();
        healthKits = playerUI.transform.Find("HealthKits").GetComponent<TMP_Text>();
        messageSpawnPoint = playerUI.transform.Find("MessageSpawn").transform;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerShooting = player.GetComponent<PlayerShooting>();
        playerController = player.GetComponent<PlayerController>();
        playerHealth = player.GetComponent<Health>();

        Transform resume = pauseMenu.transform.Find("Resume");
        UnityEngine.UI.Button resumeButton = resume.GetComponent<UnityEngine.UI.Button>();
        resumeButton.onClick.AddListener(ResumeGame);

        Debug.Log("Resume: " + resume + " Button: " + resumeButton);

        pauseMenu.transform.Find("Exit").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ExitToMainMenu);
        pauseMenu.transform.Find("Volume").GetComponent<UnityEngine.UI.Slider>().onValueChanged.AddListener(HandleVolume);

        Debug.Log("Finished setting up UI");
        ChangeMenu(0);
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
                pauseMenu.SetActive(false);
                deathScreen.SetActive(false);
                winScreen.SetActive(false);
                break;
            case 1:
                playerUI.SetActive(false);
                pauseMenu.SetActive(true);
                deathScreen.SetActive(false);
                winScreen.SetActive(false);
                break;
            case 2:
                playerUI.SetActive(false);
                pauseMenu.SetActive(false);
                deathScreen.SetActive(true);
                winScreen.SetActive(false);
                break;
            case 3:
                playerUI.SetActive(false);
                pauseMenu.SetActive(false);
                deathScreen.SetActive(false);
                winScreen.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void ResumeGame()
    {
        Debug.Log("Resuming game...");
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        ChangeMenu(0);
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        ChangeMenu(1);
        Time.timeScale = 0f;
    }

    public void HandleVolume(float volume) => AudioListener.volume = volume * volume;

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Destroy(gameObject);
        SceneManager.LoadScene("Main Menu");
    }
}
