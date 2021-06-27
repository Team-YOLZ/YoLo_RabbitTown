using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component //컴포넌트 없으면 추가.
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static GameObject FindChind(GameObject go, string name = null, bool recursive = false) //게임오브젝트형식이면
    {
        Transform transform = FindChind<Transform>(go, name, recursive);
        if (transform == null)
            return null;
        return transform.gameObject;
    }

    public static T FindChind<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object //(최상위부모, 이름 입력했는지,재귀적 검색을 할것인지(자식검색))
    {
        if (go == null)
            return null;
        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component == null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)//이름이 비어있거나 원하는 이름이면 뱉어ㅎ
                    return component;
            }
        }
        return null;
    }
}
