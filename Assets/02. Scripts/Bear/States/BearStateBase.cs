using UnityEngine;
using UnityEngine.AI;

public abstract class BearStateBase
{
    protected BearController Controller;
    protected BearStat Stat;
    protected Animator Animator;
    protected NavMeshAgent Agent;
    protected Transform Transform;

    public BearStateBase(BearController controller)
    {
        Controller = controller;
        Stat = controller.Stat;
        Animator = controller.Animator;
        Agent = controller.Agent;
        Transform = controller.transform;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
