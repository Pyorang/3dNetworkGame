using UnityEngine;

public class BearHitState : BearStateBase
{
    private bool _animFinished;

    public BearHitState(BearController controller) : base(controller) { }

    public override void Enter()
    {
        Agent.isStopped = true;
        Agent.velocity = Vector3.zero;
        Animator.SetTrigger(BearController.HashHit);
        _animFinished = false;
    }

    public override void Update()
    {
        AnimatorStateInfo info = Animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("Hit") && info.normalizedTime >= 0.9f)
        {
            _animFinished = true;
        }

        if (_animFinished)
        {
            if (Controller.LockedTarget == null)
            {
                float distToSpawn = Vector3.Distance(Transform.position, Controller.SpawnPosition);
                if (distToSpawn > 1.5f)
                    Controller.StateMachine.ChangeState(BearStateType.Return);
                else
                    Controller.StateMachine.ChangeState(BearStateType.Idle);
            }
            else
            {
                float distToTarget = Vector3.Distance(Transform.position, Controller.LockedTarget.position);

                if (distToTarget > Stat.DetectRange)
                {
                    Controller.ClearTarget();
                    Controller.StateMachine.ChangeState(BearStateType.Return);
                }
                else if (distToTarget <= Stat.AttackRange)
                {
                    Controller.StateMachine.ChangeState(BearStateType.Attack);
                }
                else
                {
                    Controller.StateMachine.ChangeState(BearStateType.Chase);
                }
            }
        }
    }

    public override void Exit()
    {
        Agent.isStopped = false;
    }
}
