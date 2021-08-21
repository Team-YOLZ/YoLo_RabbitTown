using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ArrowCtrl : MonoBehaviour
{
    public void fire(GameObject enemy, GameObject origin)
    {
        //이 스크립트를 가지고 있는 GameObject가 enemy위치로 , @f 시간만큼 이동
        transform.DOMove(enemy.transform.position + Vector3.up, 0.5f).OnComplete(() =>
        {
            //이동이 완료되면 풀링
            origin.GetComponent<ArrowPool>().ReturnObject(this);
        });
    }
}
