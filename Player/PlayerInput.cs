using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player input actions (move, teleport, jump, attack) using Unity's Input System.
/// </summary>
[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private InputAction moveAction;
    public float moveInputX;
    public float moveInputY;

    [Header("Teleport")]
    [SerializeField] private InputAction teleportAction;
    public bool teleportRequested;
    
    [Header("Jump")]
    [SerializeField] private InputAction jumpAction;
    public bool jumpRequested;
    public bool isJumpHeld;
    
    [Header("Attack")]
    [SerializeField] private InputAction attackAction;
    public bool attackRequested;

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        attackAction.Enable();
        teleportAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        attackAction.Disable();
        teleportAction.Disable();
    }
    
    void Update()
    {
        UpdateMoveInput();
        UpdateTeleportInput();
        UpdateJumpInput();
        UpdateAttackInput();
    }
    
    private void UpdateMoveInput()
    {
        moveInputX = Mathf.Round(moveAction.ReadValue<Vector2>().x);
        moveInputY = Mathf.Round(moveAction.ReadValue<Vector2>().y);
    }
    
    private void UpdateTeleportInput()
    {
        if (teleportAction.triggered) teleportRequested = true;
    }
    
    private void UpdateJumpInput()
    {
        if (jumpAction.triggered) jumpRequested = true;
        isJumpHeld = jumpAction.ReadValue<float>() > 0.5f;
    }
    
    private void UpdateAttackInput()
    {
        if (attackAction.triggered) attackRequested = true;
    }
}
