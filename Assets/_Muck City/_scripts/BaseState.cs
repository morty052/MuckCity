using UnityEngine;
using UnityEngine.AI;

public abstract class BaseState : IState
{

    protected readonly Animator _animator;
    public readonly NpcCharacter _AI;

    protected readonly int BITE_ANIM = Animator.StringToHash("Bite");
    protected readonly int TACKLE_ANIM = Animator.StringToHash("Bash");
    protected readonly int RUN_ANIM = Animator.StringToHash("Running");
    protected readonly int MID_RANGE_ATTACK_ANIM = Animator.StringToHash("Mid Range");
    protected readonly int LEAP_ANIM = Animator.StringToHash("Leap");





    protected const float _crossFadeDuration = 0.1f;


    protected BaseState(Animator animator, NpcCharacter ai)
    {
        _animator = animator;
        _AI = ai;
    }
    protected BaseState(Animator animator)
    {
        _animator = animator;
    }
    public virtual void FixedUpdate()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnExit()
    {
        // Debug.Log("exiting state");
    }

    public virtual void Update()
    {
        // Debug.Log("updating state");
    }
}
