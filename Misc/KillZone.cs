using UnityEngine;

/// <summary>
/// Damages the player if it enters the trigger. Used when player falls into the water.
/// </summary>
public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponentInParent<Player>();
        if ((player != null) && (player.state != Player.State.Dead)) player.TakeDamage(transform.position);
    }
}
