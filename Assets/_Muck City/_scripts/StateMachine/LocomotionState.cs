using Invector.vCharacterController.AI;
using UnityEngine;
using UnityEngine.AI;

public class LocomotionState : BaseState
{
    protected readonly NpcCharacter _agent;

    public bool patrolInStrafe;

    public bool updateRotationInStrafe = true;

    public vAIMovementSpeed patrolSpeed = vAIMovementSpeed.Walking;
    public LocomotionState(Animator animator, NpcCharacter _ai) : base(animator, _ai)
    {
        _agent = _ai;
    }


    public override void OnEnter()
    {
        Debug.Log("entering locomotion state");
        DoPatrolWaypoints();
    }
    public override void OnExit()
    {
        Debug.Log("exiting locomotion state");
    }


    void DoPatrolWaypoints()
    {

        if (_agent._aiController.isDead) return;

        if (_agent._aiController.waypointArea != null && _agent._aiController.waypointArea.waypoints.Count > 0)
        {
            if (_agent._aiController.targetWaypoint == null || !_agent._aiController.targetWaypoint.isValid)
            {
                _agent._aiController.NextWayPoint();
            }
            else
            {
                if (Vector3.Distance(_agent._aiController.transform.position, _agent._aiController.targetWaypoint.position) <
                    _agent._aiController.stopingDistance + _agent._aiController.targetWaypoint.areaRadius + _agent._aiController.changeWaypointDistance &&
                    _agent._aiController.targetWaypoint.CanEnter(_agent._aiController.transform) &&
                    !_agent._aiController.targetWaypoint.IsOnWay(_agent._aiController.transform))
                {
                    _agent._aiController.targetWaypoint.Enter(_agent._aiController.transform);

                }
                else if (Vector3.Distance(_agent._aiController.transform.position, _agent._aiController.targetWaypoint.position) <
                    _agent._aiController.stopingDistance + _agent._aiController.targetWaypoint.areaRadius &&
                    (!_agent._aiController.targetWaypoint.CanEnter(_agent._aiController.transform) ||
                    !_agent._aiController.targetWaypoint.isValid))
                {
                    _agent._aiController.NextWayPoint();
                }

                if (_agent._aiController.targetWaypoint != null &&
                    _agent._aiController.targetWaypoint.IsOnWay(_agent._aiController.transform) &&
                    Vector3.Distance(_agent._aiController.transform.position, _agent._aiController.targetWaypoint.position) <=
                    _agent._aiController.targetWaypoint.areaRadius + _agent._aiController.changeWaypointDistance)
                {
                    if (_agent._aiController.remainingDistance <= (_agent._aiController.stopingDistance + _agent._aiController.changeWaypointDistance) || _agent._aiController.isInDestination)
                    {
                        var timer = _agent._fsmController.GetTimer("Patrol");
                        if (timer >= _agent._aiController.targetWaypoint.timeToStay || !_agent._aiController.targetWaypoint.isValid)
                        {
                            _agent._aiController.targetWaypoint.Exit(_agent._aiController.transform);
                            _agent._aiController.visitedWaypoints.Clear();
                            _agent._aiController.NextWayPoint();
                            // if (debugMode) Debug.Log("Sort new Waypoint");
                            _agent._aiController.Stop();
                            _agent._fsmController.SetTimer("Patrol", 0);
                        }
                        else if (timer < _agent._aiController.targetWaypoint.timeToStay)
                        {
                            // if (debugMode) Debug.Log("Stay");
                            if (_agent._aiController.targetWaypoint.rotateTo)
                            {
                                _agent._aiController.Stop();
                                _agent._aiController.RotateTo(_agent._aiController.targetWaypoint.transform.forward);
                            }
                            else
                                _agent._aiController.Stop();

                            _agent._fsmController.SetTimer("Patrol", timer + Time.deltaTime);
                        }
                    }
                }
                else
                {

                    if (patrolInStrafe)
                    {
                        if (updateRotationInStrafe) _agent._aiController.StrafeMoveTo(_agent._aiController.targetWaypoint.position, _agent._aiController.desiredVelocity, patrolSpeed);
                        else _agent._aiController.StrafeMoveTo(_agent._aiController.targetWaypoint.position, patrolSpeed);
                    }

                    else
                        _agent._aiController.MoveTo(_agent._aiController.targetWaypoint.position, patrolSpeed);
                    // if (debugMode) Debug.Log("Go to new Waypoint");
                }
            }
        }
        else if (_agent._aiController.selfStartingPoint)
        {
            if (patrolInStrafe)
            {
                if (updateRotationInStrafe) _agent._aiController.StrafeMoveTo(_agent._aiController.selfStartPosition, _agent._aiController.desiredVelocity, patrolSpeed);
                else _agent._aiController.StrafeMoveTo(_agent._aiController.selfStartPosition, patrolSpeed);
            }
            else
                _agent._aiController.MoveTo(_agent._aiController.selfStartPosition, patrolSpeed);
        }
        else if (_agent._aiController.customStartPoint)
        {
            // if (_agent.debugMode)
            //     _agent.SendDebug("MoveTo CustomStartPosition", this);
            if (patrolInStrafe)
            {
                if (updateRotationInStrafe) _agent._aiController.StrafeMoveTo(_agent._aiController.customStartPosition, _agent._aiController.desiredVelocity, patrolSpeed);
                else _agent._aiController.StrafeMoveTo(_agent._aiController.customStartPosition, patrolSpeed);
            }
            else
                _agent._aiController.MoveTo(_agent._aiController.customStartPosition, patrolSpeed);
        }
        else
        {
            // if (_agent.debugMode)
            //     _agent.SendDebug("Stop Patrolling", this);
            _agent._aiController.Stop();
        }
    }
}
