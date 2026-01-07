using UnityEngine;

public class PreTeleportStateBehaviour : StateMachineBehaviour
{
    /// <summary>
    /// Called when exiting the PreTeleport state. Marks pre-teleport as finished and transitions to Teleport.
    /// </summary>
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player player = animator.GetComponentInParent<Player>();
        if (player.state == Player.State.PreTeleport)
        {
            player.teleportManager.isPreTeleportFinished = true;
            player.state = Player.State.Teleport;
        }
    }
}
