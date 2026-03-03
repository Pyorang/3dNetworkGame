using UnityEngine;

public class BearChaseState : BearStateBase
{
    public BearChaseState(BearController controller) : base(controller) { }

    public override void Enter()
    {
        Agent.speed = Stat.ChaseSpeed;
        Animator.SetBool(BearController.HashIsChasing, true);
    }

    public override void Update()
    {
        if (Controller.LockedTarget == null)
        {
            Controller.StateMachine.ChangeState(BearStateType.Return);
            return;
        }

        float distToTarget = Vector3.Distance(Transform.position, Controller.LockedTarget.position);
        Animator.SetFloat(BearController.HashMoveSpeed, Agent.velocity.magnitude);
        Agent.SetDestination(Controller.LockedTarget.position);

        if (distToTarget > Stat.DetectRange)
        {
            Controller.ClearTarget();
            Controller.StateMachine.ChangeState(BearStateType.Return);
            return;
        }

        if (distToTarget <= Stat.AttackRange)
        {
            Controller.StateMachine.ChangeState(BearStateType.Attack);
        }
    }

    public override void Exit() { }
}
