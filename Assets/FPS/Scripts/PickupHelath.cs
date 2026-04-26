using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Rendering;

public class PickupHealth : InteractAction
{
    public override bool InvokeAction()
    {
        PlayerController player = Object.FindFirstObjectByType<PlayerController>();

        if (player.healthPacks < 2)
        {
            player.healthPacks++;
            player.PlayPickupSound();
            Destroy(gameObject);
            return true;
        }
        else
        {
            return false;
        }
    }
}
