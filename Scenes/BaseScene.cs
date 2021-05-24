using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour //Scene Type 최상위 부모.
{
    //Get 타입은 Public으로 열어두고 Set타입은 protected로 막아두기위해
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;
    protected virtual void Init()
    {
        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
    }
    public abstract void Clear();
}
