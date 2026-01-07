using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton that controls the player character, handling input, movement, state transitions, attacks, and death logic.
/// </summary>
[RequireComponent(typeof(GroundCheck))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(VerticalController))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HorizontalController))]
[RequireComponent(typeof(TeleportManager))]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; } // Singleton instance

    [Header("References")]
    public PlayerInput input;
    public GroundCheck groundCheck;
    public Rigidbody2D rb2d; // make sure to set the Rigidbody2d collision detection to "continuous" in inspector
    public TeleportManager teleportManager;
    public PlayerAttack playerAttack;
    [SerializeField] private GameObject standingColliderObject;
    [SerializeField] private GameObject crouchingColliderObject;
    private Animator animator;
    private VerticalController verticalController;
    private HorizontalController horizontalController;
    
    public enum State
    {
        None,
        Idle,
        Run,
        Crouch,
        Jump,
        Fall,
        Attack,
        JumpAttack,
        CrouchAttack,
        PreTeleport,
        Teleport,
        Dead
    }
    
    private State _state = State.Idle;
    
    /// <summary>
    /// Gets or sets the current state of the player character.
    /// Changing the state updates animation parameters and toggles colliders for crouching.
    /// </summary>
    public State state
    {
        get => _state;
        set
        {
            if (_state != value)
            {
                // Debug.Log($"playerState changed: {_state} → {value}");
                bool wasCrouching = (_state is State.Crouch or State.CrouchAttack);
                bool isCrouching = (value is State.Crouch or State.CrouchAttack);
                if (wasCrouching != isCrouching)
                {
                    crouchingColliderObject.SetActive(isCrouching);
                    standingColliderObject.SetActive(!isCrouching);
                }
                _state = value;
            }
            foreach (State possibleState in Enum.GetValues(typeof(State)))
            {
                animator.SetBool(possibleState.ToString(), state == possibleState);
            }
        }
    }
    
    [SerializeField] private bool isInvincible; // only used for testing, is always false in game
    
    /// <summary>
    /// Initializes component references and enforces the singleton pattern for the Player.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple instances of Player detected! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        rb2d = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();
        groundCheck = GetComponent<GroundCheck>();
        animator = GetComponentInChildren<Animator>();
        verticalController = GetComponent<VerticalController>();
        teleportManager = GetComponent<TeleportManager>();
        horizontalController = GetComponent<HorizontalController>();
        playerAttack = GetComponent<PlayerAttack>();
        Time.timeScale = 1;
    }
    
    /// <summary>
    /// Handles physics-based updates for the player, including movement, jumping, attacking, and teleportation logic.
    /// </summary>
    private void FixedUpdate()
    {
        groundCheck.UpdateGrounded();
        
        if (state == State.Dead)
        {
            verticalController.UpdateDeadVerticalMovement();
            return;
        }
        
        teleportManager.UpdateTeleportation();
        if (teleportManager.IsTeleporting) return; // no other movement during teleport

        verticalController.UpdateVerticalMovement();
        horizontalController.UpdateHorizontalMovement();
        if (input.attackRequested) playerAttack.HandleAttackRequest();
    }
    
    /// <summary>
    /// Handles the player taking damage from an enemy, projectile, or kill zone. Any hit results in instant death.
    /// </summary>
    /// <param name="damageSourcePosition">
    /// The position of the object that damaged the player, used to determine facing direction.
    /// </param>
    public void TakeDamage(Vector2 damageSourcePosition)
    {
        // face player toward the object that caused damage
        transform.localScale = new Vector3(Mathf.Sign(damageSourcePosition.x - transform.position.x), 1, 1);

        if (!isInvincible) StartCoroutine(Die());
    }
    
    /// <summary>
    /// Handles the player's death sequence, including stopping the music, playing death sounds,
    /// setting the state to Dead, and reloading the current scene after a delay.
    /// </summary>
    private IEnumerator Die()
    {
        state = State.Dead;
        Destroy(GameObject.FindGameObjectWithTag("Music"));
        SoundManager.Instance.PlaySound(SoundManager.Instance.playerDeath);
        SoundManager.Instance.PlaySound(SoundManager.Instance.playerDeathGrunt);
        SoundManager.Instance.PlaySound(SoundManager.Instance.playerDeathHit);
        rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        teleportManager.UntintClosestEnemy();

        const float deathReloadDelay = 2.5f;
        yield return new WaitForSeconds(deathReloadDelay);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Handles player reaching the goal: stops music, plays a sound, pauses time, and reloads the scene after a delay.
    /// If I add more levels in the future, this will load the next level instead of reloading the current one.
    /// </summary>
    public IEnumerator ReachedGoal()
    {
        Time.timeScale = 0;
        Destroy(GameObject.FindGameObjectWithTag("Music"));
        SoundManager.Instance.PlaySound(SoundManager.Instance.reachedGoal);
        rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        teleportManager.UntintClosestEnemy();

        const float goalReloadDelay = 2f;
        yield return new WaitForSecondsRealtime(goalReloadDelay);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    
    /// <summary>
    /// Triggers the jump animation for the player character by setting the appropriate trigger on the Animator.
    /// </summary>
    public void TriggerJumpAnimation()
    {
        animator.SetTrigger("JumpTrigger");
    }
}
