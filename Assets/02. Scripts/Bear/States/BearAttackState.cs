using UnityEngine;

public class BearAttackState : BearStateBase
{
    private bool _attackFinished;

    public BearAttackState(BearController controller) : base(controller) { }

    public override void Enter()
    {
        Agent.isStopped = true;
        Agent.velocity = Vector3.zero;
        Animator.SetFloat(BearController.HashMoveSpeed, 0f);
        _attackFinished = false;
    }

    public override void Update()
    {
        if (!_attackFinished) return;

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

        // 아직 공격 범위 안 → Attack 유지, 애니메이터가 Attack Idle → Attack 자동 순환
        _attackFinished = false;
    }

    public void OnAttackAnimFinished()
    {
        _attackFinished = true;
    }

    public override void Exit()
    {
        Agent.isStopped = false;
    }
}
