using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    }

    //public static void BindEvnet(this GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    //{
    //    UI_Base.BindEvnet(go, action, type);
    //}
}
