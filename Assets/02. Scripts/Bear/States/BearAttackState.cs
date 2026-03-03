using UnityEngine;

public class BearAttackState : BearStateBase
{
    public BearAttackState(BearController controller) : base(controller) { }

    public override void Enter()
    {
        Agent.isStopped = true;
        Agent.velocity = Vector3.zero;
        Animator.SetFloat(BearController.HashMoveSpeed, 0f);
        Animator.SetTrigger(BearController.HashAttack);

        if (Controller.LockedTarget != null)
        {
            Vector3 dir = (Controller.LockedTarget.position - Transform.position).normalized;
            dir.y = 0f;
            if (dir != Vector3.zero)
                Transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    public override void Update()
    {
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
        }
    }

    public override void Exit()
    {
        Agent.isStopped = false;
    }
}
