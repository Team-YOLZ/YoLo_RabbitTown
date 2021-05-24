using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object //조건 T 는 오브젝트
    {
        return Resources.Load<T>(path); //리소스 로드하면 우리의 리소스 로드문을 써라
    }

    public GameObject Instantiate(string path, Transform parent = null) //  생성문을 쓸거
    {
        //1. original 들고 있으면 바로 사용.
        GameObject original = Load<GameObject>($"Prefabs/{path}"); // 우리가 맵핑한 로드문을 써라  Prefabs폴더 산하의 path
        if (original == null) // 로드하려는것이 없을시
        {
            Debug.Log($"Failed To Load Prefab : {path}"); //디버그문 출력
            return null;
        }
        return Object.Instantiate(original, parent);
    }
}
