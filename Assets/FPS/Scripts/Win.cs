using UnityEngine;

public class Win : MonoBehaviour
{
    private bool triggered = false;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            triggered = true;
            Time.timeScale = 0f;
            FPSUI.Instance.ChangeMenu(3);
        }
    }

    void OnMenu()
    {
        if (!triggered) return;
        FPSUI.Instance.ExitToMainMenu();
    }
}
