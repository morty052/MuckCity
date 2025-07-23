using UnityEngine;
using DG.Tweening;
using System;
using System.Threading.Tasks;

public static class ABUtils
{

    public static void ScaleIn(Transform transform, Action callBack = null)
    {
        transform.DOScale(Vector3.one, 0.1f)
        .OnComplete(() => callBack?.Invoke());
    }
    public static void ScaleOut(Transform transform, Action callBack = null)
    {
        transform.DOScale(Vector3.zero, 0.1f)
        .OnComplete(() => callBack?.Invoke());

    }


    public static async void DelayedInvoke(float delay, Action action)
    {
        await Task.Delay((int)delay * 1000);
        action?.Invoke();
    }
}
