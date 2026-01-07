using UnityEngine;

/// <summary>
/// Triggers the player's goal-reaching behavior when the player enters the goal area.
/// </summary>
public class Goal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponentInParent<Player>();
        if ((player != null) && (player.state != Player.State.Dead)) StartCoroutine(player.ReachedGoal());
    }
}
