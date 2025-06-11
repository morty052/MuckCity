using Invector.vCharacterController.AI;
using UnityEngine;
using UnityEngine.AI;

public class IdleState : BaseState
{
    protected readonly NpcCharacter _agent;

    public IdleState(Animator animator, NpcCharacter _ai) : base(animator, _ai)
    {
        _agent = _ai;
    }

    public override void OnEnter()
    {
        Debug.Log("entering idle state");
    }
    public override void OnExit()
    {
        Debug.Log("exiting idle state");
    }



}
