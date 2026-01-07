using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all enemies, handling facing and damaging the player, enemy list management, and death logic.
/// </summary>
public abstract class Enemy : MonoBehaviour 
{
    /// <summary>
    /// A global list of all enemies for efficient searching (used by TeleportManager to find the closest enemy).
    /// </summary>
    public static readonly List<Enemy> AllEnemies = new();

    [Header("Enemy")]
    public SpriteTinter spriteTinter;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private Transform deathEffectPoint;
    protected Animator animator;

    public virtual bool AlwaysFacePlayer => false;
    
    /// <summary>
    /// Adds this enemy to the global enemy list and initializes references to the animator and sprite tinter.
    /// </summary>
    protected virtual void Awake()
    {
        AllEnemies.Add(this);
        animator = GetComponentInChildren<Animator>();
        spriteTinter = GetComponent<SpriteTinter>();
    }
  
    /// <summary>
    /// Mirrors the enemy horizontally to face the player if <c>AlwaysFacePlayer</c> is true.
    /// </summary>
    protected virtual void Update()
    {
        if (AlwaysFacePlayer) FacePlayer();
    }

    /// <summary>
    /// Called when the enemy collides with another collider. Deals damage to the player if touched.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponentInParent<Player>();
        if ((player != null) && (player.state != Player.State.Dead))
        {
            player.TakeDamage(transform.position);
        }
    }

    /// <summary>
    /// Called when the enemy is destroyed. Removes this enemy from the global enemy list.
    /// </summary>
    private void OnDestroy()
    {
        AllEnemies.Remove(this);
    }
        
    /// <summary>
    /// Mirrors the enemy horizontally to face the player.
    /// </summary>
    public void FacePlayer()
    {
        float scaleX = Mathf.Sign(transform.position.x - Player.Instance.transform.position.x);
        transform.localScale = new Vector3(scaleX, 1, 1);
    }

    /// <summary>
    /// Handles the enemy's death by playing a sound, spawning a death effect, and destroying the enemy game object.
    /// </summary>
    public void Die()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.enemyDeath);
        Instantiate(deathEffect, deathEffectPoint.transform.position, transform.rotation, null);
        Destroy(gameObject);
    }
}
