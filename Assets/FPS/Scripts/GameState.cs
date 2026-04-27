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
}
