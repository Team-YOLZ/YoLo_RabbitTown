using UnityEngine;
using BackEnd;

public class BackEndGetTable : MonoBehaviour
{
    static BackEndGetTable s_instance; //유일성이 보장된다.
    static BackEndGetTable Instance { get { Init(); return s_instance; } }

    public PlayerStatData playerStat;
    public PlayerAssetData playerAsset;
    public PlayerKillData playerKillData;

    private string StatTableKey;
    private string AssetTableKey;
    private string CountTableKey;

    private MainScene MainPage;

    private void Awake()
    {
        Init(); //싱글톤 보장.


        InitUserStatTable(); //스탯 테이블 입력.
        InitUserAssetTable(); //자산 테이블 입력.
        InitCaptureCountTable(); //킬카운트 테이블 입력.

        MainPage = GameObject.Find("@MainScene").GetComponent<MainScene>();//메인 페이지 갱신위한 클래스 할당.

        Debug.Log(StatTableKey);
        BackendReturnObject bro = Backend.BMember.GetUserInfo();
        playerStat.OwnerIndate = bro.GetReturnValuetoJSON()["row"]["inDate"].ToString();
    }

    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("UserTableInformation");
            if (go == null)
            {
                go = new GameObject { name = "UserTableInformation" };
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<BackEndGetTable>();
        }
    }

    //유저 스탯 테이블 init.
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
            OwnerIndate = bro.Rows()[i]["inDate"]["S"].ToString();
        }
        string[] select = { "Attack", "AttackSpeed", "MovingSpeed", "Hp", "Leadership", "Appeal" };

        // 테이블 내 해당 rowIndate를 지닌 row를 조회
        // select에 존재하는 컬럼만 리턴
        var BRO = Backend.GameData.Get("UserStatTable", OwnerIndate, select); //BackEnd Return 데이터 Get.
        var json = BRO.GetReturnValuetoJSON(); //BackEnd Return 데이터 => JSON 데이터로 변환
        playerStat = new PlayerStatData(json); //PlayerData 클래스에 할당.
        StatTableKey = OwnerIndate;
    }

    //유저 자산 테이블 init.
    public void InitUserAssetTable()
    {
        var bro = Backend.GameData.GetMyData("UserAssetTable", new Where(), 10);
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
            OwnerIndate = bro.Rows()[i]["inDate"]["S"].ToString();
        }
        string[] select = { "Coin", "Spoil1", "Spoil2", "Spoil3", "Spoil4" };

        // 테이블 내 해당 rowIndate를 지닌 row를 조회
        // select에 존재하는 컬럼만 리턴
        var BRO = Backend.GameData.Get("UserAssetTable", OwnerIndate, select); //BackEnd Return 데이터 Get.
        var json = BRO.GetReturnValuetoJSON(); //BackEnd Return 데이터 => JSON 데이터로 변환
        playerAsset = new PlayerAssetData(json); //PlayerData 클래스에 할당.
        AssetTableKey = OwnerIndate;
    }

    //유저 킬카운트 테이블 init.
    public void InitCaptureCountTable()
    {
        var bro = Backend.GameData.GetMyData("UnitCaptureCount", new Where(), 10);
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
            OwnerIndate = bro.Rows()[i]["inDate"]["S"].ToString();
        }
        string[] select = { "Chicken1", "Chicken2", "Chicken3", "Cow1", "Cow2", "Cow3", "Goat1", "Goat2", "Horse1", "Horse2", "Horse3" };

        // 테이블 내 해당 rowIndate를 지닌 row를 조회
        // select에 존재하는 컬럼만 리턴
        var BRO = Backend.GameData.Get("UnitCaptureCount", OwnerIndate, select); //BackEnd Return 데이터 Get.
        var json = BRO.GetReturnValuetoJSON(); //BackEnd Return 데이터 => JSON 데이터로 변환
        playerKillData = new PlayerKillData(json); //PlayerData 클래스에 할당.
        CountTableKey = OwnerIndate;
    }

    //포획 유닛 정보 Init
    //public void InitCaptureUnitTable()
    //{
    //    var bro = Backend.GameData.GetMyData("OwnUnitTable", new Where(), 10);
    //    string OwnerIndate = "";
    //    if (bro.IsSuccess() == false)
    //    {
    //        // 요청 실패 처리
    //        Debug.Log(bro);
    //        return;
    //    }
    //    if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
    //    {
    //        // 요청이 성공해도 where 조건에 부합하는 데이터가 없을 수 있기 때문에
    //        // 데이터가 존재하는지 확인
    //        // 위와 같은 new Where() 조건의 경우 테이블에 row가 하나도 없으면 Count가 0 이하 일 수 있다.
    //        Debug.Log(bro);
    //        return;
    //    }
    //    //내 테이블의 rowindate 할당.
    //    for (int i = 0; i < bro.Rows().Count; ++i)
    //    {
    //        OwnerIndate = bro.Rows()[i]["inDate"]["S"].ToString();
    //    }
    //    string[] select = {"Name"};

    //    // 테이블 내 해당 rowIndate를 지닌 row를 조회
    //    // select에 존재하는 컬럼만 리턴
    //    var BRO = Backend.GameData.Get("OwnUnitTable", OwnerIndate, select); //BackEnd Return 데이터 Get.
    //    var json = BRO.GetReturnValuetoJSON(); //BackEnd Return 데이터 => JSON 데이터로 변환
    //    playerKillData = new PlayerKillData(json); //PlayerData 클래스에 할당.
    //    CountTableKey = OwnerIndate;
    //}

    //(임시 양식) 공격력 증가 버튼.
    public void UpAttackButton()
    {
        // 1단계 DB 수정.
        UpAttackStat();    // BackEnd UserTable 스탯 증가.
        ConsumptionCoin(); // BackEnd UserAssetTable 코인 감소.

        // 2단계 수정된 DB 정보 다시 PlayerData 클래스(싱글톤 객체)에 할당.
        InitUserStatTable(); //수정된 UserTable 정보 갱신.
        InitUserAssetTable(); //수정된 UserAssetTable 정보 갱신.

        //3단계 수정된 PlayerData 정보 스텟 페이지에 재 할당.
        MainPage.EditPage(); //스탯 페이지 갱신.
    }

    //임시 공격력 증가 함수.
    private void UpAttackStat()
    {
        Param updateParam = new Param();
        updateParam.AddCalculation("Attack", GameInfoOperator.addition, 10); // 기존 데이터 Attack에서 10만큼 더 추가

        Backend.GameData.UpdateWithCalculation("UserStatTable", new Where(), updateParam); //백엔드 UserTable에 적용.
    }

    //임시 스탯 증가 비용 지불.
    private void ConsumptionCoin()
    {
        Param updateParam = new Param();
        updateParam.AddCalculation("Coin", GameInfoOperator.subtraction, 10); // 기존 데이터 Attack에서 10만큼 더 추가

        Backend.GameData.UpdateWithCalculation("UserAssetTable", new Where(), updateParam); //백엔드 UserTable에 적용.
    }

    public void AssetUpdate()
    {
        Param updateParam = new Param();
        updateParam.Add("Coin", playerAsset.Coin);
        updateParam.Add("Spoil1", playerAsset.Spoil1);
        updateParam.Add("Spoil2", playerAsset.Spoil2);
        updateParam.Add("Spoil3", playerAsset.Spoil3);
        updateParam.Add("Spoil4", playerAsset.Spoil4);

        Backend.GameData.Update("UserAssetTable", AssetTableKey, updateParam);
    }

    public void KillCountUpdate()
    {
        Param updateParam = new Param();
        updateParam.Add("Chicken1", playerKillData.Chicken1);
        updateParam.Add("Chicken2", playerKillData.Chicken2);
        updateParam.Add("Chicken3", playerKillData.Chicken3);
        updateParam.Add("Cow1", playerKillData.Cow1);
        updateParam.Add("Cow2", playerKillData.Cow2);
        updateParam.Add("Cow3", playerKillData.Cow1);
        updateParam.Add("Goat1", playerKillData.Goat1);
        updateParam.Add("Goat2", playerKillData.Goat2);
        updateParam.Add("Horse1", playerKillData.Horse1);
        updateParam.Add("Horse2", playerKillData.Horse2);
        updateParam.Add("Horse3", playerKillData.Horse3);


        Backend.GameData.Update("UnitCaptureCount", CountTableKey, updateParam);
    }
}
