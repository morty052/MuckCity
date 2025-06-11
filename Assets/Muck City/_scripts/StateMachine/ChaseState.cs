using ImprovedTimers;
using UnityEngine;

public class ChaseState : BaseState
{
    public ChaseState(Animator animator, NpcCharacter _ai) : base(animator, _ai) { }

    readonly float _repositionCountdown = 1f;
    CountdownTimer _timer;

    public override void OnEnter()
    {
        Debug.Log("entering chase state");
        SetupTimers();
        // _AI._agent.isStopped = false;
        // _AI.HandleChase();
    }



    void SetupTimers()
    {
        _timer = new CountdownTimer(_repositionCountdown);
        _timer.OnTimerStop += () =>
        {
            // _AI.HandleChase();
            _timer.Start();
        };
        _timer.Start();
    }

    public override void OnExit()
    {
        _animator.SetBool(RUN_ANIM, false);
        _timer.Stop();
        _timer.Dispose();
        Debug.Log("exiting chase state");
    }


}
