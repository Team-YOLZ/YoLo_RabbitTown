using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : BaseScene
{
    [Header("Data날리기")]
    public bool delete;
    void Start()
    {
        DeleteData();
        Init();
    }
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Title;
    }
    public override void Clear()
    {
        //날려줄 정보들
        Debug.Log("TitleScene Clear");
    }

    public void DeleteData()
    {
        if (delete == true)
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
