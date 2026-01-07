using UnityEngine;

/// <summary>
/// Handles the behavior of enemy projectiles, including movement, collision detection,
/// and triggering impact effects upon hitting the player or the environment.
/// </summary>
public class Projectile : MonoBehaviour
{
    [SerializeField] private int speedPixelsPerSecond = 180;
    [SerializeField] private GameObject impactPrefab;
    [SerializeField] private Transform impactPoint;
    [SerializeField] private float impactSoundRange = 19f; // used to only play impact sound when impacting on-screen
    private Rigidbody2D rb2d;
    
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Handles collision events for the projectile. 
    /// If it hits the player, it damages the player and triggers an impact effect.
    /// If it hits the environment, it triggers an impact effect.
    /// </summary>
    /// <param name="other">The collider that the projectile has entered.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player != null && player.state != Player.State.Dead)
        {
            player.TakeDamage(transform.position);
            Impact();
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Environment")) Impact();
    }

    /// <summary>
    /// Sets the projectile's direction and velocity.
    /// </summary>
    /// <param name="direction">
    /// The direction multiplier for the projectile's movement. (1 for left, -1 for right.)
    /// </param>
    public void SetDirection(float direction)
    {
        rb2d.velocity = Vector2.left * direction * speedPixelsPerSecond / GameConstants.PixelsPerUnit;
        transform.localScale = new Vector3(
            transform.localScale.x * direction,
            transform.localScale.y,
            transform.localScale.z);
    }

    /// <summary>
    /// Triggers the projectile's impact effect, plays a sound, and destroys the projectile.
    /// </summary>
    private void Impact()
    {
        if (impactPrefab != null && impactPoint != null)
        {
            Instantiate(impactPrefab, impactPoint.position, impactPoint.rotation);
        }
        
        float distanceToPlayer = Vector3.Distance(Player.Instance.transform.position, transform.position);
        if (distanceToPlayer <= impactSoundRange)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.fireballImpact);
        }

        Destroy(gameObject);
    }
}
