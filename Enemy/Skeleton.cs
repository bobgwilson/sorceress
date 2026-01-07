using UnityEngine;

/// <summary>
/// Controls skeleton enemy behavior, including movement, edge and obstacle detection, and turning around.
/// </summary>
public class Skeleton : Enemy
{
    [Header("Skeleton")]    
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private Transform groundAheadCheck;
    [SerializeField] private Transform obstacleAheadCheck;
    [SerializeField] private int speedPixelsPerSecond = 60;
    private Rigidbody2D rb2d;

    protected override void Awake()
    {
        base.Awake();
        rb2d = GetComponent<Rigidbody2D>();
        SetWalkVelocity();
    }
    
    private void FixedUpdate()
    {
        CheckEdgesAndObstacles();
    }

    /// <summary>
    /// Sets the skeleton's horizontal walking velocity based on its facing direction.
    /// </summary>
    private void SetWalkVelocity()
    {
        rb2d.velocity = new Vector2(-transform.localScale.x * speedPixelsPerSecond /  GameConstants.PixelsPerUnit, 0);
    }
    
    /// <summary>
    /// Checks for ground and obstacles ahead of the skeleton and triggers a turn if needed.
    /// </summary>
    private void CheckEdgesAndObstacles()
    {
        bool isGroundAhead = Physics2D.OverlapCircle(groundAheadCheck.position, groundAheadCheck.localScale.x, groundLayerMask);
        bool isObstacleAhead = Physics2D.OverlapCircle(obstacleAheadCheck.position, obstacleAheadCheck.localScale.x, groundLayerMask);
        if (!isGroundAhead || isObstacleAhead) TurnAround();
    }

    /// <summary>
    /// Reverses the skeleton's facing direction and updates its walking velocity.
    /// </summary>
    private void TurnAround()
    {
        transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
        SetWalkVelocity();
    }

    #if UNITY_EDITOR
    /// <summary>
    /// Draws wireframe circles in the Scene view to visualize the ground and obstacle check ranges.
    /// </summary>
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.DrawWireDisc(groundAheadCheck.position, Vector3.forward, groundAheadCheck.localScale.x);
        UnityEditor.Handles.DrawWireDisc(obstacleAheadCheck.position, Vector3.forward, obstacleAheadCheck.localScale.x);
    }
    #endif
}
