using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    List<EnemyCtrl> list1 = new List<EnemyCtrl>();
    List<EnemyCtrl> list2 = new List<EnemyCtrl>();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            list1.Add(EnemyPool.GetObject("Goat1"));
        if (Input.GetKeyDown(KeyCode.S))
            foreach (EnemyCtrl obj in list1)
                EnemyPool.ReturnObject(obj);

        if (Input.GetKeyDown(KeyCode.Q))
            list2.Add(EnemyPool.GetObject("Chicken1"));
        if (Input.GetKeyDown(KeyCode.W))
            foreach (EnemyCtrl obj in list2)
                EnemyPool.ReturnObject(obj);

        if (Input.GetKeyDown(KeyCode.D))
            EnemyPool.Clear();

    }
}
