using UnityEngine;

public class BearIdleState : BearStateBase
{
    private float _idleTimer;
    private float _idleDuration;

    public BearIdleState(BearController controller) : base(controller) { }

    public override void Enter()
    {
        Agent.isStopped = true;
        Agent.velocity = Vector3.zero;
        Animator.SetFloat(BearController.HashMoveSpeed, 0f);
        _idleDuration = Random.Range(Stat.IdleTimeMin, Stat.IdleTimeMax);
        _idleTimer = 0f;
    }

    public override void Update()
    {
        if (Controller.TryDetectPlayer())
        {
            Controller.StateMachine.ChangeState(BearStateType.Chase);
            return;
        }

        _idleTimer += Time.deltaTime;
        if (_idleTimer >= _idleDuration)
        {
            Controller.StateMachine.ChangeState(BearStateType.Patrol);
        }
    }

    public override void Exit()
    {
        Agent.isStopped = false;
    }
}
