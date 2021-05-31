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

    private void Update()
    {
        //임시로 씬넘겨주는 코드
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Managers.Scene.LoadScene(Define.Scene.Main);
        }
    }
}
