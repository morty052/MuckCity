using DialogueEditor;
using UnityEngine;
using Invector.vCharacterController.AI;



[CreateAssetMenu(fileName = "Npc", menuName = "ScriptableObjects/NewNpc/GuardNpc", order = 1)]
public class GuardNpcSO : NpcSO
{

    public vWaypointArea _defaultPatrolPoints;
    public Vector3 _idlePosition;

    private void OnValidate()
    {
#if UNITY_EDITOR
        _name = this.name;
        if (!_roles.Contains(Role.GUARD))
        {
            _roles.Add(Role.GUARD);
        }
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }


}
