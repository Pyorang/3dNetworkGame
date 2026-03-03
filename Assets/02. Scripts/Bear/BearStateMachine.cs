using System.Collections.Generic;

public enum BearStateType
{
    Idle,
    Patrol,
    Detect,
    Chase,
    Attack,
    Return,
    Hit,
    Dead
}

public class BearStateMachine
{
    private Dictionary<BearStateType, BearStateBase> _states = new();
    private BearStateBase _currentState;

    public BearStateType CurrentStateType { get; private set; }
    public BearStateType PreviousStateType { get; private set; }

    public System.Action OnStateChanged;

    public void AddState(BearStateType type, BearStateBase state)
    {
        _states[type] = state;
    }

    public void Initialize(BearStateType startState)
    {
        CurrentStateType = startState;
        _currentState = _states[startState];
        _currentState.Enter();
        OnStateChanged?.Invoke();
    }

    public void ChangeState(BearStateType newState)
    {
        if (_currentState != null)
        {
            PreviousStateType = CurrentStateType;
            _currentState.Exit();
        }

        CurrentStateType = newState;
        _currentState = _states[newState];
        _currentState.Enter();
        OnStateChanged?.Invoke();
    }

    public void Update()
    {
        _currentState?.Update();
    }

    public void RevertToPreviousState()
    {
        ChangeState(PreviousStateType);
    }
}
