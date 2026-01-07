using UnityEngine;

/// <summary>
/// Handles player horizontal movement, crouching, facing, and state updates.
/// </summary>
public class HorizontalController : MonoBehaviour
{
    [SerializeField] private int runSpeedPixelsPerSecond = 120;
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    /// <summary>
    /// Handles player horizontal movement, facing, crouching, and state updates each physics frame.
    /// </summary>
    public void UpdateHorizontalMovement()
    {
        // Prevent horizontal movement and direction change during ground attack or teleport states
        if (player.state is Player.State.Attack
            or Player.State.CrouchAttack
            or Player.State.PreTeleport
            or Player.State.Teleport)
            return;
        
        UpdateFacingDirection();
        if (ProcessCrouchInput()) return;

        if (player.groundCheck.isGrounded)
        {
            if (player.input.moveInputX == 0) player.state = Player.State.Idle;
            else player.state = Player.State.Run;
        }
        SetHorizontalVelocity();
    }
    
    /// <summary>
    /// Updates the player's facing direction based on input or horizontal velocity.
    /// Adjusts the local scale to flip the sprite and handles state changes when changing direction during air attacks.
    /// </summary>
    private void UpdateFacingDirection()
    {
        float facing = 0;
        const float facingVelocityThreshold = 1f; // if horizontal velocity exceeds this, use it to determine facing
        
        if (player.input.moveInputX != 0) facing = Mathf.Sign(player.input.moveInputX);
        else if (Mathf.Abs(player.rb2d.velocity.x) > facingVelocityThreshold) facing = Mathf.Sign(player.rb2d.velocity.x);

        if (facing == 0 || transform.localScale.x == facing) return;

        Vector3 oldLocalScale = transform.localScale;
        transform.localScale = new Vector3(facing, 1, 1);
        
        // if changing direction during air attack, change to Jump or Fall state so magic attack doesn't switch sides
        if (oldLocalScale.x != transform.localScale.x && player.state == Player.State.JumpAttack)
        {
            if (player.rb2d.velocity.y > 0)
            {
                player.state = Player.State.Jump;
                player.TriggerJumpAnimation();
            }
            else player.state = Player.State.Fall;
        }
    }
    
    /// <summary>
    /// Handles crouch input while grounded, setting state and stopping movement if crouching.
    /// </summary>
    /// <returns>True if crouching, otherwise false.</returns>
    private bool ProcessCrouchInput()
    {
        if (player.input.moveInputY < 0 && player.groundCheck.isGrounded)
        {
            player.state = Player.State.Crouch;
            player.rb2d.velocity = new Vector2(0, player.rb2d.velocity.y);
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Applies horizontal velocity to the player based on input.
    /// </summary>
    private void SetHorizontalVelocity()
    {
        float velocityX = player.input.moveInputX * runSpeedPixelsPerSecond / GameConstants.PixelsPerUnit;
        player.rb2d.velocity = new Vector2(velocityX, player.rb2d.velocity.y);
    }
}
