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
        if (info.IsName("Hit") && info.normalizedTime >= 1f)
        {
            _animFinished = true;
        }

        if (_animFinished)
        {
            Controller.StateMachine.RevertToPreviousState();
        }
    }

    public override void Exit()
    {
        Agent.isStopped = false;
    }
}
