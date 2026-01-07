using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Checks if the player is grounded by casting rays downward from multiple points.
/// </summary>
public class GroundCheck : MonoBehaviour
{
    public bool isGrounded = true;
    public bool wasGrounded = true; // Was the player grounded in the previous frame? This is used to detect landing.
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float groundCheckDistance = 0.05f;
    [SerializeField] private float groundCheckHalfWidth = 0.4f;

    /// <summary>
    /// Performs ground detection every physics frame and updates <c>wasGrounded</c> and <c>isGrounded</c>.
    /// </summary>
    public void UpdateGrounded()
    {
        wasGrounded = isGrounded;
        Vector2 groundCheckPosition = transform.position;
        Vector2 left = groundCheckPosition + new Vector2(-groundCheckHalfWidth, 0);
        Vector2 center = groundCheckPosition;
        Vector2 right = groundCheckPosition + new Vector2(groundCheckHalfWidth, 0);

        isGrounded = Physics2D.Raycast(left,   Vector2.down, groundCheckDistance, groundLayerMask) ||
                     Physics2D.Raycast(center, Vector2.down, groundCheckDistance, groundLayerMask) ||
                     Physics2D.Raycast(right,  Vector2.down, groundCheckDistance, groundLayerMask);
    }

    /// <summary>
    /// Draws gizmo lines in the editor to visualize the ground check raycast positions and distances.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 groundCheckPosition = transform.position;
        Vector2 left = groundCheckPosition + new Vector2(-groundCheckHalfWidth, 0);
        Vector2 center = groundCheckPosition;
        Vector2 right = groundCheckPosition + new Vector2(groundCheckHalfWidth, 0);
        Vector2 down = Vector2.down * groundCheckDistance;
        Gizmos.DrawLine(left, left + down);
        Gizmos.DrawLine(center, center + down);
        Gizmos.DrawLine(right, right + down);
    }
}
