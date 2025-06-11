using System.Threading.Tasks;
using UnityEngine;

public interface ILoadDataOnStart
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Task OnLoadTask();

    // Update is called once per frame
    void AddLoadingTaskToQueue();
}
