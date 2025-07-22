using UnityEngine;
using DG.Tweening;
using System;

public static class ExtendedUIMono
{

    public static void ScaleIn(Transform transform, Action callBack = null)
    {
        transform.DOScale(Vector3.one, 0.3f)
        .OnComplete(() => callBack?.Invoke());
    }
    public static void ScaleOut(Transform transform, Action callBack = null)
    {
        transform.DOScale(Vector3.zero, 0.3f)
        .OnComplete(() => callBack?.Invoke());

    }
}
