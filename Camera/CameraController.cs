using UnityEngine;

/// <summary>
/// Manages camera movement to follow the player and pan smoothly to the player's destination when teleporting.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = false;

    [Header("Teleport Pan Settings")]
    [Tooltip("Duration of the camera pan during teleportation in seconds.")]
    [SerializeField] private float panDuration = 0.5f;
    
    private bool isPanning;
    private float panStartTime;
    private Vector3 panStartPosition;
    private Player player;

    private void Start()
    {
        player = Player.Instance;
    }
    
    /// <summary>
    /// Updates the camera position to follow the player, or pan during teleportation.
    /// </summary>
    private void LateUpdate()
    {
        float cameraX = transform.position.x;
        float cameraY = transform.position.y;
        float cameraZ = transform.position.z;
        float playerX = player.transform.position.x;
        float playerY = player.transform.position.y;
        
        if (Time.timeScale != 0)
        {
            if (followX) cameraX = playerX;
            if (followY) cameraY = playerY;
        }
        else if (isPanning)
        {
            float t = Mathf.Clamp01((Time.unscaledTime - panStartTime) / panDuration);
            if (followX) cameraX = Mathf.SmoothStep(panStartPosition.x, playerX, t);
            if (followY) cameraY = Mathf.SmoothStep(panStartPosition.y, playerY, t);
    
            if (t >= 1)
            {
                isPanning = false;
                player.teleportManager.FinishTeleport();
            }
        }
        
        transform.position = new Vector3(cameraX, cameraY, cameraZ);
    }

    /// <summary>
    /// Initiates a camera pan to the player's new position during teleportation.
    /// </summary>
    public void StartPan()
    {
        panStartPosition = gameObject.transform.position;
        panStartTime = Time.unscaledTime;
        isPanning = true;
    }
}
