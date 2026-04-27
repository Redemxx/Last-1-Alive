using UnityEngine;

public class Win : MonoBehaviour
{
    private bool triggered = false;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            triggered = true;
            FPSUI.Instance.PauseGame();
        }
    }

    void OnMenu()
    {
        if (!triggered) return;
        FPSUI.Instance.ExitToMainMenu();
    }
}
