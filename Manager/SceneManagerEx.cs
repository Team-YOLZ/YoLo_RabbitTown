using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneManagerEx
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void LoadScene(Define.Scene type)//Scene을 넘어갈때 현재 씬 정보 Clear해주고 넘어갈거임.
    {
        Managers.Clear();
        SceneManager.LoadScene(GetSceneName(type));
    }
    string GetSceneName(Define.Scene type)
    {
        string Name = System.Enum.GetName(typeof(Define.Scene), type);
        return Name;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}