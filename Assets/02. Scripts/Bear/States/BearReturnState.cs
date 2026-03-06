using UnityEngine;

public class BearReturnState : BearStateBase
{
    private float _healTimer;
    private float _healInterval;
    private int _healTicksRemaining;
    private float _healPerTick;

    public BearReturnState(BearController controller) : base(controller) { }

    public override void Enter()
    {
        Agent.speed = Stat.ReturnSpeed;
        Agent.SetDestination(Controller.SpawnPosition);

        _healInterval = Stat.RecoveryDuration / Stat.RecoveryTicks;
        _healTicksRemaining = Stat.RecoveryTicks;
        _healPerTick = (Stat.MaxHp - Stat.HP) / Mathf.Max(Stat.RecoveryTicks, 1);
        _healTimer = 0f;
    }

    public override void Update()
    {
        Animator.SetFloat(BearController.HashMoveSpeed, Agent.velocity.magnitude);

        if (_healTicksRemaining > 0)
        {
            _healTimer += Time.deltaTime;
            if (_healTimer >= _healInterval)
            {
                _healTimer -= _healInterval;
                Stat.HP += _healPerTick;
                _healTicksRemaining--;
            }
        }

        if (!Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance + 0.1f)
        {
            Stat.HP = Stat.MaxHp;
            Controller.StateMachine.ChangeState(BearStateType.Idle);
        }
    }

    public override void Exit() { }
}
