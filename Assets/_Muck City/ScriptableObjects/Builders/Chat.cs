using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chat", menuName = "ScriptableObjects/Phone/Chat")]
public class Chat : ScriptableObject
{
    public List<InstantMessage> _dialogue;
}
