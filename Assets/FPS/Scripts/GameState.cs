using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int gamemode = 1;
    public float[] gameModifiers = {-0.35f, 0f, 0.25f};

    public float GameModifier() { return gameModifiers[gamemode]; }

    public float gameVolume = 0.5f;
    public float mouseSensitivity = 0.5f;

    public void SetMouseSensitivity(float sensitivity)
    {
        mouseSensitivity = sensitivity;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController == null) return;

        playerController.SetMouseSensitivity(sensitivity);
    }

    public void HandleVolume(float volume)
    {
        AudioListener.volume = volume * volume;
        gameVolume = volume * volume;
    }

    public void HandleSensitivity(float sensitivity)
    {
        Debug.Log("Sensitivity: " + sensitivity);
        SetMouseSensitivity(sensitivity * sensitivity);
    }
}
