using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ArrowCtrl : MonoBehaviour
{
    //나중에 풀링방식으로 변경시켜야함.
    //dotween에서도 풀링시키는게 있는것같은데 알아보는중..
    public void fire(GameObject enemy)
    {
        transform.DOMove(enemy.transform.position, 0.5f).OnComplete(() =>
        {
            Destroy(gameObject); 
        });
    }
}
