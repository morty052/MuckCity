using UnityEngine;

[CreateAssetMenu(fileName = "DelveBuddyFunctionContainer", menuName = "ScriptableObjects/DelveBuddy/DelveBuddyFunctionContainer", order = 1)]
public class DelveBuddyFunctionContainer : ScriptableObject
{
    [SerializeReference] public DelveBuddyFunction function;
}
