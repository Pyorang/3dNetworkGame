using UnityEngine;

public class BearDeadState : BearStateBase
{
    public BearDeadState(BearController controller) : base(controller) { }

    public override void Enter()
    {
        Agent.isStopped = true;
        Agent.velocity = Vector3.zero;
        Animator.SetTrigger(BearController.HashDie);

        if (Controller.HitCollider != null)
            Controller.HitCollider.enabled = false;
    }

    public override void Update() { }
    public override void Exit() { }
}
