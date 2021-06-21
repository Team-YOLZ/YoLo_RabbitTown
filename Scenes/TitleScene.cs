using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : BaseScene
{
    void Start()
    {
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
}
