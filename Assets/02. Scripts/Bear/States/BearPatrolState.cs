using UnityEngine;

public class BearPatrolState : BearStateBase
{
    public BearPatrolState(BearController controller) : base(controller) { }

    public override void Enter()
    {
        Agent.speed = Stat.WalkSpeed;

        Vector3 randomPoint = Controller.GetRandomPatrolPoint();
        Agent.SetDestination(randomPoint);
    }

    public override void Update()
    {
        Animator.SetFloat(BearController.HashMoveSpeed, Agent.velocity.magnitude);

        if (Controller.TryDetectPlayer())
        {
            Controller.StateMachine.ChangeState(BearStateType.Chase);
            return;
        }

        if (!Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance + 0.1f)
        {
            Controller.StateMachine.ChangeState(BearStateType.Idle);
        }
    }

    public override void Exit() { }
}
