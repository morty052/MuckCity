using System;
using System.Threading.Tasks;
using UnityEngine;

public class ExtendedMono : MonoBehaviour, ILoadDataOnStart, IFindable
{
    public GameObject GameObject => throw new NotImplementedException();

    public bool IsQuestItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public virtual void AddLoadingTaskToQueue()
    {
        throw new System.NotImplementedException();
    }

    public virtual Task OnLoadTask()
    {
        throw new System.NotImplementedException();
    }

    public void RemoveInteractionListener(Action<string> action)
    {
        throw new NotImplementedException();
    }

    public void SetupInteractionListener(Action<string> action)
    {
        throw new NotImplementedException();
    }
}
