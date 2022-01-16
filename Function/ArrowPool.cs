
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 화살통개념
// 원거리 유닛에게 스크립트 넣어주기
public class ArrowPool : MonoBehaviour
{
    private Queue<ArrowCtrl> _queue = new Queue<ArrowCtrl>();
    void Start()
    {
        Init(2);
    }
    private ArrowCtrl CreateNewObject() //비 활성으로 arrow 만들어 주기
    {
        var obj = Managers.Resource.Instantiate($"Creature_YJ/Arrow_{(gameObject.name).Substring(0, gameObject.name.Length - 1)}", transform).GetComponent<ArrowCtrl>();
        
        obj.gameObject.SetActive(false);
        return obj;
    }
    private void Init(int count) //count만큼 큐에 arrow 넣기
    {
        for (int i = 0; i < count; i++)
        {
            _queue.Enqueue(CreateNewObject());
        }
    }
    public ArrowCtrl GetObject()
    {
        if (_queue.Count > 0) //큐안에 오브젝트가 남아 있으면
        {
            var obj = _queue.Dequeue();
            obj.transform.SetParent(gameObject.transform);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else //없으면 새로 만들기
        {
            var newObj = CreateNewObject();
            newObj.transform.SetParent(gameObject.transform);
            newObj.gameObject.SetActive(true);
            return newObj;
        }
    }
    public void ReturnObject(ArrowCtrl arrow)
    {
        arrow.gameObject.SetActive(false);
        arrow.transform.SetParent(gameObject.transform);
        _queue.Enqueue(arrow);
    }

}
