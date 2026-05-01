using UnityEngine;

public class PickupGrenade : InteractAction
{
    private PlayerShooting player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerShooting>();
    }

    public override bool InvokeAction()
    {
        player.AddGrenade();
        Destroy(gameObject);
        return true;
    }
}
