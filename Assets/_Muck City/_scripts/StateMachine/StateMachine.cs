using System;
using System.Collections.Generic;
using UnityEngine;

public class FuncPredicate : IPredicate
{
    readonly Func<bool> func;

    public FuncPredicate(Func<bool> func)
    {
        this.func = func;
    }

    public bool Evaluate() => func.Invoke();
}

public class StateMachine
{

    public StateNode _currentState;
    Dictionary<Type, StateNode> _nodes = new();
    HashSet<ITransition> _anyTransitions = new();

    public void Update()
    {
        var transition = GetTransition();

        if (transition != null)
        {
            ChangeState(transition.To);
        }

        //! UNCOMMENT TO USE UPDATE FUNC WITH STATES
        _currentState.State?.Update();
    }

    public void FixedUpdate()
    {
        _currentState.State?.FixedUpdate();
    }


    public void SetState(IState state)
    {
        _currentState = _nodes[state.GetType()];
        _currentState.State?.OnEnter();
    }

    void ChangeState(IState state)
    {
        if (state == _currentState.State) return;

        var previousState = _currentState.State;
        var nextState = _nodes[state.GetType()].State;

        previousState?.OnExit();
        nextState?.OnEnter();

        _currentState = _nodes[state.GetType()];
    }

    ITransition GetTransition()
    {

        foreach (var transition in _anyTransitions)
        {
            if (transition.Condition.Evaluate())
            {
                return transition;
            }
        }

        foreach (var transition in _currentState.Transitions)
        {
            if (transition.Condition.Evaluate())
            {
                // Debug.Log($"Transition to {transition.To}");
                return transition;
            }
        }

        return null;

    }

    public void AddTransition(IState from, IState to, IPredicate condition)
    {
        // Debug.Log($"Adding transition from {from} to {to}");
        GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
    }

    public void AddAnyTransition(IState to, IPredicate condition)
    {
        _anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
    }

    StateNode GetOrAddNode(IState state)
    {
        var node = _nodes.GetValueOrDefault(state.GetType());

        if (node == null)
        {
            node = new StateNode(state);
            _nodes.Add(state.GetType(), node);
        }


        return node;
    }

    public class StateNode
    {
        public IState State { get; }
        public HashSet<ITransition> Transitions { get; }

        public StateNode(IState state)
        {
            State = state;
            Transitions = new HashSet<ITransition>();
        }

        public void AddTransition(IState to, IPredicate condition)
        {
            Transitions.Add(new Transition(to, condition));
        }
    }

}
