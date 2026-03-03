using UnityEngine;

public class BearDetectState : BearStateBase
{
    public BearDetectState(BearController controller) : base(controller) { }

    public override void Enter()
    {
        Agent.isStopped = true;
        Agent.velocity = Vector3.zero;
        Animator.SetFloat(BearController.HashMoveSpeed, 0f);
        Animator.SetTrigger(BearController.HashDetect);

        if (Controller.LockedTarget != null)
        {
            Vector3 dir = (Controller.LockedTarget.position - Transform.position).normalized;
            dir.y = 0f;
            if (dir != Vector3.zero)
                Transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    public override void Update() { }

    public override void Exit()
    {
        Agent.isStopped = false;
    }
}
