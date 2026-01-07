using System.Collections;
using UnityEngine;

/// <summary>
/// Manages player teleportation, including finding the closest enemy, and controlling teleport effects.
/// </summary>
public class TeleportManager : MonoBehaviour
{
    public bool isPreTeleportFinished;
    [Tooltip("How many times to swap during teleport. Must be an odd number.")]
    [SerializeField] private int numberOfSwaps = 5;
    [SerializeField] private float swapDuration = 1f / 12;
    [SerializeField] private Enemy closestEnemy;
    [SerializeField] private float teleportRange = 17.5f;

    private Player player;
    private CameraController cameraController;

    public bool IsTeleporting => player.state is Player.State.PreTeleport or Player.State.Teleport;
    
    private void Awake()
    {
        player = GetComponent<Player>();
        cameraController = FindObjectOfType<CameraController>();
    }
    
    /// <summary> 
    /// Handles the per-frame teleportation update logic: finds and tints the closest enemy,
    /// processes teleport requests, and starts the teleport coroutine after pre-teleport animation finishes.
    /// </summary>
    public void UpdateTeleportation()
    {
        if (!IsTeleporting) FindAndTintClosestEnemy();
        if (player.input.teleportRequested) HandleTeleportRequest();
        if (isPreTeleportFinished) StartCoroutine(Teleport());
    }
    
    /// <summary>
    /// Finds the closest enemy, applies partial tint, and removes tint from previous closest enemy.
    /// </summary>
    void FindAndTintClosestEnemy()
    {
        Enemy oldClosestEnemy = closestEnemy;
        closestEnemy = FindClosestEnemy();

        if (closestEnemy != oldClosestEnemy) 
        {
            TintEnemy(closestEnemy, 0.5f);
            TintEnemy(oldClosestEnemy, 0);
        }
    }

    /// <summary>
    /// Searches all enemies and returns the closest one to the player within the teleport range.
    /// </summary>
    /// <returns>The closest enemy within range, or null if none are found.</returns>
    private Enemy FindClosestEnemy()
    {
        float closestDistance = Mathf.Infinity;
        Enemy closestEnemySoFar = null;
        foreach (Enemy enemy in Enemy.AllEnemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemySoFar = enemy;
            }
        }
        if (closestDistance > teleportRange) closestEnemySoFar = null;
        return closestEnemySoFar;
    }

    /// <summary>
    /// Sets the tint blend value for the specified enemy, if the enemy is not null.
    /// </summary>
    /// <param name="enemy">The enemy to tint.</param>
    /// <param name="blendValue">The blend value to apply to the enemy's tint.</param>

    private void TintEnemy(Enemy enemy, float blendValue)
    {
        if (enemy != null) enemy.spriteTinter.BlendValue = blendValue;
    }
    
    /// <summary>
    /// Handles the player's teleport request: resets the request flag and initiates pre-teleport if conditions are met.
    /// </summary>
    private void HandleTeleportRequest()
    {
        player.input.teleportRequested = false;
        if ((!IsTeleporting) && (player.groundCheck.isGrounded) && (closestEnemy != null)) PreTeleport();
    }
    
    /// <summary>
    /// Initiates pre-teleport state (brief meditation before teleporting), fully tints the enemy, and plays a sound.
    /// </summary>
    private void PreTeleport()
    {
        player.state = Player.State.PreTeleport;
        player.rb2d.velocity = Vector2.zero;

        TintEnemy(closestEnemy, 1);
        SoundManager.Instance.PlaySound(SoundManager.Instance.preTeleport);
    }
    
    /// <summary>
    /// Performs the teleportation sequence: plays the teleport sound, pauses time,
    /// swaps player and enemy positions multiple times for effect, and starts camera pan.
    /// </summary>
    private IEnumerator Teleport()
    {
        isPreTeleportFinished = false;
        SoundManager.Instance.PlaySound(SoundManager.Instance.teleport);
        Time.timeScale = 0;
        
        for (int i = 0; i < numberOfSwaps; i++)
        {
            SwapPlayerAndEnemy();
            yield return new WaitForSecondsRealtime(swapDuration);
        }
        cameraController.StartPan();
    }

    /// <summary>
    /// Swaps positions of player and closest enemy, and makes enemy face the player if required to (wizards always do).
    /// </summary>
    private void SwapPlayerAndEnemy()
    {
        (player.transform.position, closestEnemy.transform.position) = (closestEnemy.transform.position, player.transform.position);
        if (closestEnemy.AlwaysFacePlayer) closestEnemy.FacePlayer();
    }
    
    /// <summary>
    /// Restarts time after teleportation, restores swapped enemy's partial tint, and updates the closest enemy selection.
    /// </summary>
    public void FinishTeleport() 
    {
        Time.timeScale = 1;
        TintEnemy(closestEnemy, 0.5f);
        FindAndTintClosestEnemy();
    }
    
    /// <summary>
    /// Removes any tint from the currently selected closest enemy. This is called when the player dies.
    /// </summary>
    public void UntintClosestEnemy()
    {
        TintEnemy(closestEnemy, 0);
    }
}
