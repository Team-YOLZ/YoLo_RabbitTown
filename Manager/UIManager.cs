//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class UIManager
//{
//    //int _order = 10;

//    //Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
//    //UI_Scene _sceneUI = null;

//    public void SetCanvas(GameObject go, bool sort = true) //기존 UI와 우선순위 비교
//    {
//        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
//        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
//        canvas.overrideSorting = true;// 캔비스 안에 캔버스가있을때 상관없이 내가 소트를 가질거야

//        if (sort)
//        {
//            canvas.sortingOrder = _order;
//            _order++;
//        }
//        else
//        {
//            canvas.sortingOrder = 0;
//        }
//    }

//    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
//    {
//        if (string.IsNullOrEmpty(name))
//            name = typeof(T).Name;

//        GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}");

//        if (parent != null)
//            go.transform.SetParent(parent);
//        return Util.GetOrAddComponent<T>(go);
//    }

//    public T ShowSceneUI<T>(string name = null) where T : UI_Scene //Scene UI열
//    {
//        if (string.IsNullOrEmpty(name))
//            name = typeof(T).Name;

//        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");

//        T sceneUI = Util.GetOrAddComponent<T>(go);
//        _sceneUI = sceneUI;

//        GameObject root = GameObject.Find("@UI_Root");
//        if (root == null)
//            root = new GameObject { name = "@UI_Root" };

//        go.transform.SetParent(root.transform);
//        return sceneUI;
//    }

//    public T ShowPopUpUI<T>(string name = null) where T : UI_Popup //팝업열
//    {
//        if (string.IsNullOrEmpty(name))
//            name = typeof(T).Name;

//        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");

//        T popup = Util.GetOrAddComponent<T>(go);
//        _popupStack.Push(popup);

//        GameObject root = GameObject.Find("@UI_Root");
//        if (root == null)
//            root = new GameObject { name = "@UI_Root" };

//        go.transform.SetParent(root.transform);
//        return popup;
//    }

//    public void ClosePopupUI(UI_Popup popup)//만약 순차적으로 팝업을 지우는게 실패했을때 경고,종료.
//    {
//        if (_popupStack.Count == 0) return;

//        if (_popupStack.Peek() != popup)
//        {
//            Debug.Log("Close Popup failed");
//            return;
//        }
//        ClosePopupUI();
//    }


//    public void ClosePopupUI() //팝업 하나씩 삭제 스택에서
//    {
//        if (_popupStack.Count == 0) return;

//        UI_Popup popup = _popupStack.Pop();
//        Managers.Resource.Destroy(popup.gameObject);
//        popup = null;
//        _order--;
//    }

//    public void CloseAllPopupUI() //팝업 다삭제
//    {
//        while (_popupStack.Count > 0)
//        {
//            ClosePopupUI();
//        }
//    }
//    public void Clear()
//    {
//        CloseAllPopupUI();
//        _sceneUI = null;
//    }
//}
