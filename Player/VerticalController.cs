using UnityEngine;

/// <summary>
/// Manages player vertical movement, including jumping, jump buffering, coyote time, double jump, landing, and gravity.
/// </summary>
public class VerticalController : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 4.25f;
    [SerializeField] private float jumpTimeToApex = 0.4f;
    [SerializeField] private bool variableJumpHeight = true;
    [Tooltip("Multiplier applied to reduce upward velocity every physics frame when jump is cancelled")]
    [SerializeField] private float jumpCancelMultiplier = 0.7f;
    private float coyoteTimeTimer;
    [SerializeField] private float coyoteTimeDuration = 0.15f;
    private float jumpBufferTimer;
    [SerializeField] private float jumpBufferDuration = 0.15f;
    private int jumpCount;
    [SerializeField] private int maxJumpCount = 2;
    [SerializeField] private float fastestFallSpeed = -30;
    
    private Player player;
    private Rigidbody2D rb2d;
    private PlayerInput input;
    private GroundCheck groundCheck;
    
    private void Start()
    {
        player = GetComponent<Player>();
        rb2d = player.rb2d;
        input = player.input;
        groundCheck = player.groundCheck;
    }

    /// <summary>
    /// Performs per-frame jump logic, updating timers, handling jump requests, air movement, landing, and gravity.
    /// </summary>
    public void UpdateVerticalMovement()
    {
        UpdateJumpTimers();
        if (input.jumpRequested) HandleJumpRequest();
        if (!player.groundCheck.isGrounded) InAirLogic();
        else if (!player.groundCheck.wasGrounded) Land();
        ApplyGravity();
    }

    /// <summary>
    /// Updates both the coyote time and jump buffer timers each physics frame.
    /// Coyote time provides a short grace period for jumping after leaving the ground.
    /// Jump buffer allows jump input to be buffered for a short duration before landing.
    /// </summary>
    private void UpdateJumpTimers()
    {
        if (groundCheck.isGrounded) coyoteTimeTimer = coyoteTimeDuration;
        else coyoteTimeTimer = Mathf.Max(coyoteTimeTimer - Time.fixedDeltaTime, 0f);

        jumpBufferTimer = Mathf.Max(jumpBufferTimer - Time.fixedDeltaTime, 0f);
    }
    
    /// <summary>
    /// Handles a jump request, performing a normal jump, coyote time jump, double jump, or buffering the jump.
    /// </summary>
    private void HandleJumpRequest()
    {
        input.jumpRequested = false;
        if (player.state
            is Player.State.Attack
            or Player.State.JumpAttack
            or Player.State.PreTeleport
            or Player.State.Teleport) return;
        if (groundCheck.isGrounded) Jump(); // normal jump
        else if (coyoteTimeTimer > 0f) Jump(); // coyote time jump
        else if ((jumpCount > 0) && (jumpCount < maxJumpCount)) Jump(); // double jump
        else StoreJumpInBuffer();
    }
    
    /// <summary>
    /// Performs a jump by updating velocity, counters, timers, and triggering jump state and animation.
    /// </summary>
    private void Jump()
    {
        player.state = Player.State.Jump;
        jumpCount++;
        jumpBufferTimer = 0f;
        coyoteTimeTimer = 0f;
        groundCheck.isGrounded = false;

        float jumpVelocity = (2 * jumpHeight) / jumpTimeToApex;
        rb2d.velocity = new Vector2(rb2d.velocity.x, jumpVelocity);

        player.TriggerJumpAnimation();
        SoundManager.Instance.PlaySound(SoundManager.Instance.jump);
    }

    /// <summary>
    /// Stores a jump request in the jump buffer when the player unsuccessfully attempts to jump while in air.
    /// If the player lands while the buffer is active, the jump will be performed automatically.
    /// </summary>
    private void StoreJumpInBuffer()
    {
        jumpBufferTimer = jumpBufferDuration;
    }

    /// <summary>
    /// Handles transition to fall state, and jump cancellation when the jump button is released while moving upward.
    /// </summary>
    private void InAirLogic()
    {
        if ((rb2d.velocity.y < 0) && (player.state != Player.State.JumpAttack))
        {
            player.state = Player.State.Fall;
        }
        
        // If the player released jump button while moving upward, accelerate down faster
        if (variableJumpHeight && !input.isJumpHeld && rb2d.velocity.y > 0)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * jumpCancelMultiplier);
        }
    }
    
    /// <summary>
    /// Handles landing logic and performs a buffered jump if available.
    /// </summary>
    private void Land()
    {
        if (input.moveInputX == 0) player.state = Player.State.Idle;
        else player.state = Player.State.Run;

        jumpCount = 0;
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0f);
        SoundManager.Instance.PlaySound(SoundManager.Instance.land);

        // If the player landed while having a jump buffered, jump
        if (jumpBufferTimer > 0) Jump();
    }
    
    /// <summary>
    /// Applies custom gravity to the player based on jump height and apex time.
    /// </summary>
    public void ApplyGravity()
    {
        float jumpGravity = (2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
        rb2d.velocity -= jumpGravity * Time.fixedDeltaTime * Vector2.up;
        // apply maxFallSpeed
        rb2d.velocity = new Vector2(rb2d.velocity.x, Mathf.Max(rb2d.velocity.y, fastestFallSpeed));
    }
    
    /// <summary>
    /// Applies dead-state physics and plays landing sound if just landed.
    /// </summary>
    public void UpdateDeadVerticalMovement()
    {
        rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        if (!groundCheck.isGrounded) ApplyGravity();
        else if (groundCheck.isGrounded && !groundCheck.wasGrounded) SoundManager.Instance.PlaySound(SoundManager.Instance.land);
    }
}
