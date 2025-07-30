using UnityEngine;
using DG.Tweening;
using System;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;

public class SecurityPost : Interactable
{
    [SerializeField] private GameObject _barriersParent;

    [SerializeField] float _barrierOpenY;
    float _barrierCloseY;





    [Button("Open Barriers")]
    public void OpenBarriers()
    {
        // Transform barrier = _barriersParent.transform.GetChild(_num);
        // MoveBarrier(barrier, _num, () =>
        // {

        //     if (_num + 1 < _childCount)
        //     {
        //         _num++;
        //         Debug.Log(_num);
        //         barrier = _barriersParent.transform.GetChild(_num);
        //         MoveBarrier(barrier, _num, AnimateBarriersClose);
        //     }
        //     else
        //     {
        //         _num = 0;
        //     }
        // });

        _barriersParent.transform.DOLocalRotate(new(-90f, 0, 0), 1f)
        .OnComplete(() => _barriersParent.transform.DOLocalMoveY(_barrierOpenY, 1f));
    }


}
