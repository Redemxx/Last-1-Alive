using UnityEngine;

public class PickupFlashlight : InteractAction
{
    public override bool InvokeAction()
    {
        PlayerController player = Object.FindFirstObjectByType<PlayerController>();

        player.hasFlashlight = true;
        player.PlayPickupSound();
        FPSUI.Instance.ShowMessage("Press F to toggle flashlight");
        Destroy(gameObject);
        return true;
    }
}
