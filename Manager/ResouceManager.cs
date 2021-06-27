using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object //조건 T 는 오브젝트
    {
        if(typeof(T) == typeof(GameObject)) //이러한 경우 프리팹일 경우가 크다.
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);

            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
                return go as T;
        }
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

        //2. 혹시 폴링된 애가 있나?
        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;

        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        //풀링된 애라면 푸쉬
        Poolable poolable = go.GetComponent<Poolable>();
        if(poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go);
    }
}
