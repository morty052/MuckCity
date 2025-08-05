using UnityEngine;
using DG.Tweening;
using System;
using System.Threading.Tasks;
using Invector.vItemManager;

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

    public static bool IsAhead(Transform reference, Transform target)
    {
        Vector3 toTarget = (target.position - reference.position).normalized;
        float dot = Vector3.Dot(reference.forward, toTarget);

        // If dot > 0, target is in front; if dot < 0, it's behind
        return dot > 0f;
    }

    public static async void StartLerp(Transform floater, Transform target, float duration, float delay = 0, AnimationCurve animationCurve = null, Action OnComplete = null)
    {
        await LerpToTarget(floater, target.position, duration, delay, OnComplete, animationCurve);
    }

    private static async Task LerpToTarget(Transform obj, Vector3 destination, float duration, float delay = 0, Action OnComplete = null, AnimationCurve animationCurve = null)
    {
        await Task.Delay((int)(delay * 1000));
        Vector3 startPos = obj.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            if (animationCurve != null)
            {
                t = animationCurve.Evaluate(elapsed / duration);
            }
            obj.position = Vector3.Lerp(startPos, destination, t);
            await Task.Yield(); // Wait for next frame
        }

        obj.position = destination; // Snap to final position
        OnComplete?.Invoke();
    }
}
