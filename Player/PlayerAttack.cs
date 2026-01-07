using UnityEngine;

/// <summary>
/// Handles player attack logic, including processing attack requests and applying melee attack effects.
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    [Header("Melee Attack")]
    [SerializeField] private Transform attackCheck;

    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }
    
    /// <summary>
    /// Handles the player's attack request and updates state accordingly.
    /// </summary>
    public void HandleAttackRequest()
    {
        player.input.attackRequested = false;

        switch (player.state)
        {
            case Player.State.Jump or Player.State.Fall:
                player.state = Player.State.JumpAttack;
                break;
            case Player.State.Run or Player.State.Idle:
                player.state = Player.State.Attack;
                player.rb2d.velocity = new Vector2(0, player.rb2d.velocity.y);
                break;
            case Player.State.Crouch:
                player.state = Player.State.CrouchAttack;
                player.rb2d.velocity = new Vector2(0, player.rb2d.velocity.y);
                break;
        }
    }

    /// <summary>
    /// When the melee attack animation reaches the attack frame, applies damage and plays a sound.
    /// </summary>
    public void OnMeleeAttackFrame()
    {
        int enemyLayerMask = LayerMask.GetMask("Enemies");
        Collider2D[] enemyColliders =
            Physics2D.OverlapCircleAll(attackCheck.position, attackCheck.localScale.x, enemyLayerMask);
        
        foreach (Collider2D enemyCollider in enemyColliders)
        {
            enemyCollider.GetComponent<Enemy>().Die();
        }

        SoundManager.Instance.PlaySound(SoundManager.Instance.attack);
    }
    
    #if UNITY_EDITOR
    /// <summary>
    /// Draws the attack range circle in the Scene view for easy adjustment.
    /// </summary>
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.DrawWireDisc(attackCheck.position, Vector3.forward, attackCheck.localScale.x);
    }
    #endif
}
