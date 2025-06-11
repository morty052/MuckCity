using UnityEngine;

public class GuardNPC : NpcCharacter
{
    GuardNpcSO _guardNpcSO;
    protected override void SetupData()
    {
        base.SetupData();
        _guardNpcSO = _npcSO as GuardNpcSO;
        _aiController.waypointArea = _guardNpcSO._defaultPatrolPoints;
    }
}
