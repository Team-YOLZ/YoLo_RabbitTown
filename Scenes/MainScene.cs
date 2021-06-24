using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

public class MainScene : BaseScene
{
    [SerializeField] private Text Attack;
    [SerializeField] private Text AttackSpeed;
    [SerializeField] private Text MovingSpeed;
    [SerializeField] private Text Hp;
    [SerializeField] private Text Leadership;
    [SerializeField] private Text Appeal;
    void Start()
    {
        Init();
        InitUserStatTable();
    }
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Main;
    }
    public override void Clear()
    {
        Debug.Log("MainScene Clear");
    }
    public void InitUserStatTable()
    {
        var bro = Backend.GameData.GetMyData("UserStatTable", new Where(), 10);
        string OwnerIndate = "";
        if (bro.IsSuccess() == false)
        {
            // 요청 실패 처리
            Debug.Log(bro);
            return;
        }
        if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
        {
            // 요청이 성공해도 where 조건에 부합하는 데이터가 없을 수 있기 때문에
            // 데이터가 존재하는지 확인
            // 위와 같은 new Where() 조건의 경우 테이블에 row가 하나도 없으면 Count가 0 이하 일 수 있다.
            Debug.Log(bro);
            return;
        }
        //내 테이블의 rowindate 할당.
        for (int i = 0; i < bro.Rows().Count; ++i)
        {
            OwnerIndate = bro.Rows()[i]["updatedAt"]["S"].ToString();
        }

        string[] select = { "Attack", "AttackSpeed", "MovingSpeed","Hp","Leadership","Appeal" };

        // 테이블 내 해당 rowIndate를 지닌 row를 조회
        // select에 존재하는 컬럼만 리턴
        var BRO = Backend.GameData.Get("UserStatTable", OwnerIndate, select); //BackEnd Return 데이터 Get.
        var json = BRO.GetReturnValuetoJSON(); //BackEnd Return 데이ㅌ => JSON 데이터로 변환
        PlayerData MyPlayerData = new PlayerData(json); //PlayerData 클래스에 할당.

        //MainScene Text UI에 값 할당.
        Attack.text = MyPlayerData.Attack.ToString();
        AttackSpeed.text = MyPlayerData.AttackSpeed.ToString();
        MovingSpeed.text = MyPlayerData.MovingSpeed.ToString();
        Hp.text = MyPlayerData.Hp.ToString();
        Leadership.text = MyPlayerData.Leadership.ToString();
        Appeal.text = MyPlayerData.Appeal.ToString();
    }
    private void Update()
    {
        //임시로 씬넘겨주는 코드
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Managers.Scene.LoadScene(Define.Scene.Game);
        }
    }
}
