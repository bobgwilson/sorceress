using UnityEngine;
using System.Collections;

public class Wizard : Enemy
{
    [Header("Wizard")]
    [SerializeField] private float attackRange = 17.5f;
    private bool attackRoutineActive;
    [SerializeField] private float delayBeforeFirstAttack = 0.5f;
    [SerializeField] private float attackInterval = 2;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform throwPoint;
    
    public enum State { Idle, Attack }
    private State _state = State.Idle;
    public State state
    {
        get => _state;
        set
        {
            _state = value;
            animator.SetBool("Idle", _state == State.Idle);
            animator.SetBool("Attack", _state == State.Attack);
        }
    }

    public override bool AlwaysFacePlayer => true;
    
    private bool IsPlayerInRange => Mathf.Abs(transform.position.x - Player.Instance.transform.position.x) <= attackRange;
    
    /// <summary>
    /// Calls the base class <c>Update()</c> and checks if the player is in range to start the attack routine.
    /// </summary>
    protected override void Update()
    {
        base.Update();
        if (!attackRoutineActive && IsPlayerInRange) StartCoroutine(AttackRoutine());
    }

    /// <summary>
    /// Coroutine that periodically throws a projectile if the player is in range.
    /// </summary>
    IEnumerator AttackRoutine()
    {
        attackRoutineActive = true;
        yield return new WaitForSeconds(delayBeforeFirstAttack);
        while (true)
        {
            if (!IsPlayerInRange) break;
            state = State.Attack;
            yield return new WaitForSeconds(attackInterval);
        }
        attackRoutineActive = false;
    }
    
    /// <summary>
    /// Throws a projectile and plays a sound when the attack animation reaches the throw frame.
    /// </summary>
    public void OnThrowProjectileFrame()
    {
        GameObject projectile = Instantiate(projectilePrefab, throwPoint.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
        SoundManager.Instance.PlaySound(SoundManager.Instance.fireball);
    }
}
