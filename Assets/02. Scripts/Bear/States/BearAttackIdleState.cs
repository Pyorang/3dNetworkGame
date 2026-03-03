using UnityEngine;

public class BearAttackIdleState : BearStateBase
{
    private float _timer;

    public BearAttackIdleState(BearController controller) : base(controller) { }

    public override void Enter()
    {
        Agent.isStopped = true;
        Agent.velocity = Vector3.zero;
        Animator.SetFloat(BearController.HashMoveSpeed, 0f);
        _timer = 0f;
    }

    public override void Update()
    {
        _timer += Time.deltaTime;

        if (Controller.LockedTarget == null)
        {
            Controller.StateMachine.ChangeState(BearStateType.Return);
            return;
        }

        float dist = Vector3.Distance(Transform.position, Controller.LockedTarget.position);

        if (dist > Stat.DetectRange)
        {
            Controller.ClearTarget();
            Controller.StateMachine.ChangeState(BearStateType.Return);
            return;
        }

        if (dist > Stat.AttackRange)
        {
            Controller.StateMachine.ChangeState(BearStateType.Chase);
            return;
        }

        if (_timer >= Stat.AttackCooldown)
        {
            Controller.StateMachine.ChangeState(BearStateType.Attack);
        }
    }

    public override void Exit()
    {
        Agent.isStopped = false;
    }
}
