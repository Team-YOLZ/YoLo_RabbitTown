using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    void Start()
    {
        Init();
    }
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Game;

        //GameObject go = new GameObject { name = "WorldObj" };
        //WorldObjectGenerator pool = go.GetOrAddComponent<WorldObjectGenerator>();
        //pool.SetTreeCount(5);
    }
    public override void Clear()
    {

    }
}
